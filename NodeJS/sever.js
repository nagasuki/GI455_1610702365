var websocket = require('ws');
const sqlite = require('sqlite3').verbose();

var websocketServer = new websocket.Server({port:4624}, ()=>{
    console.log("Mano Chat Server is running");
});

var wsList = [];
var roomList = [];

var database = new sqlite.Database('./database/dbchat.db', sqlite.OPEN_CREATE | sqlite.OPEN_READWRITE, (err)=>{
    if(err) throw err;
    
    console.log("Connect to database");

    /*var userID = "test03"
    var password = "111111"

    var sqlSelect = "SELECT * FROM userData WHERE UserID='"+userID+"' AND UserPassword='"+password+"'"// Login

    database.all(sqlSelect, (err, rows) =>{
        if(err){
            console.log(err);
        }
        else{
            console.log("Login Sumled");
        }
    })*/

    websocketServer.on("connection", (ws)=>{
    
        //Lobby
        console.log("client connected.");

        wsList.push(ws);

        //Reception 
        // ตรงนี้แยกกับระบบแชท ต้องสร้าง ws.on ใหม่่ถึงแชทได้
        ws.on("message", (data)=>{
            console.log("send from client :"+ data);
    
            //========== Convert jsonStr into jsonObj =======
    
            //toJsonObj = JSON.parse(message);
    
            // I change to line below for prevent confusion
            var toJsonObj = {
                eventName:"",
                data:"test:111111",
                username:""
            }
            //===============================================
            toJsonObj = JSON.parse(data);
            //===============================================

            // SaveData
            var splitStr = toJsonObj.data.split(':');

            var userID = splitStr[0];
            var password = splitStr[1];
            var name = splitStr[2];
            var money = parseInt(splitStr[3]);

            var sqlSelect = "SELECT * FROM userData WHERE UserID='"+userID+"' AND UserPassword='"+password+"'"// Login
            var sqlInsert =  `INSERT INTO userData (UserID, UserPassword, UserName) VALUES ('${userID}', '${password}', '${name}')`;// Register
            //var sqlUpdate = "UPDATE userData SET Money='500' WHERE UserID='"+(userID)+"'";
            //var sqlAddmoney = "UPDATE userData SET Money='"+currentMoney+"' WHERE UserID='"+userID+"'";
            //var sqlCheckMoney = "SELECT Money FROM userData WHERE UserID='"+userID+"'";

            //===============================================
            
            if(toJsonObj.eventName == "CreateRoom")//CreateRoom
            {
                //============= Find room with roomName from Client =========
                var isFoundRoom = false;
                for(var i = 0; i < roomList.length; i++)
                {
                    if(roomList[i].roomName == toJsonObj.data)
                    {
                        isFoundRoom = true;
                        console.log(roomList);
                        break;
                    }
                }
                //===========================================================
    
                if(isFoundRoom == true)// Found room
                {
                    //Can't create room because roomName is exist.
                    //========== Send callback message to Client ============
    
                    ws.send("CreateRoomFail");
    
                    //I will change to json string like a client side. Please see below
                    var callbackMsg = {
                        eventName:"CreateRoom",
                        data:"fail"
                    }
                    //===============================================
                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);
                    //=======================================================
    
                    console.log("client create room fail.");
                }
                else
                {
                    //============ Create room and Add to roomList ==========
                    var newRoom = {
                        roomName: toJsonObj.data,
                        wsList: []
                    }
    
                    newRoom.wsList.push(ws);
    
                    roomList.push(newRoom);
                    //=======================================================
    
                    //========== Send callback message to Client ============
                    //===============================================
                    ws.send("CreateRoomSuccess");
                    //===============================================
                    //I need to send roomName into client too. I will change to json string like a client side. Please see below
                    var callbackMsg = {
                        eventName:"CreateRoom",
                        data:toJsonObj.data
                    }
                    //===============================================
                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);
                    //=======================================================
                    console.log("client create room success.");
                }
    
                //console.log("client request CreateRoom ["+toJsonObj.data+"]");
                
            }
            else if(toJsonObj.eventName == "JoinRoom")//JoinRoom
            {
                //============= Home work ================
                // Implementation JoinRoom event when have request from client.
    
                var isJoinRoom = false;
    
                for(var i = 0; i < roomList.length; i++)
                {
                    if(roomList[i].roomName == toJsonObj.data)
                    {
                        if(roomList[i].roomName == toJsonObj.data)
                        {
                            isJoinRoom = true;
                            roomList[i].wsList.push(ws);
                            break;
                        }
                    }
                }
    
                if (isJoinRoom == true)
                    {
                        var callbackMsg = {
                            eventName:"JoinRoom",
                            data:"success",
                        }
                        //===============================================
                        var toJsonStr = JSON.stringify(callbackMsg);
                        ws.send(toJsonStr);
                        //=======================================================

                        console.log("client join room success.");
                        console.log("client request JoinRoom ["+toJsonObj.data+"]");
                    }
                    else
                    {
                        var callbackMsg = {
                            eventName:"JoinRoom",
                            data:"fail",
                        }
                        //===============================================
                        var toJsonStr = JSON.stringify(callbackMsg);
                        ws.send(toJsonStr);
                        //===============================================
                        console.log("client join room fail.");
                    }
                
                //================= Hint =================
                //roomList[i].wsList.push(ws);
    
                console.log("client request JoinRoom");
                //========================================
            }
            else if(toJsonObj.eventName == "LeaveRoom")//LeaveRoom
            {
                //============ Find client in room for remove client out of room ================
                var isLeaveSuccess = false;//Set false to default.
                for(var i = 0; i < roomList.length; i++)//Loop in roomList
                {
                    for(var j = 0; j < roomList[i].wsList.length; j++)//Loop in wsList in roomList
                    {
                        if(ws == roomList[i].wsList[j])//If founded client.
                        {
                            roomList[i].wsList.splice(j, 1);//Remove at index one time. When found client.
    
                            if(roomList[i].wsList.length <= 0)//If no one left in room remove this room now.
                            {
                                roomList.splice(i, 1);//Remove at index one time. When room is no one left.
                            }
                            isLeaveSuccess = true;
                            break;
                        }
                    }
                }
                //===============================================================================
    
                if(isLeaveSuccess)
                {
                    //========== Send callback message to Client ============
                    //===============================================
                    ws.send("LeaveRoomSuccess");
                    //===============================================
                    //I will change to json string like a client side. Please see below
                    var callbackMsg = {
                        eventName:"LeaveRoom",
                        data:"success"
                    }
                    //===============================================
                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);
                    //=======================================================
    
                    console.log("leave room success");
                }
                else
                {
                    //========== Send callback message to Client ============
    
                    //ws.send("LeaveRoomFail");
    
                    //I will change to json string like a client side. Please see below
                    var callbackMsg = {
                        eventName:"LeaveRoom",
                        data:"fail"
                    }
                    //===============================================
                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);
                    //=======================================================
    
                    console.log("leave room fail");
                }
            }
            else if(toJsonObj.eventName == "Register"){
                database.all(sqlInsert, (err, rows)=>{
                    if(err){
                        console.log(err);

                        var callbackMsg = {
                            eventName:"Register",
                            data:"fail"
                        }
                        //===============================================
                        var toJsonStr = JSON.stringify(callbackMsg);
                        ws.send(toJsonStr);
                        //=======================================================
                        console.log("[0]" +toJsonStr);
                    }
                    else{
                        var callbackMsg = {
                            eventName:"Register",
                            data:"success"
                        }
                        //===============================================
                        var toJsonObj = JSON.stringify(callbackMsg);
                        ws.send(toJsonObj);
                        //=======================================================
                        console.log("[1]" +toJsonStr);
                    }
                })
            }
            else if(toJsonObj.eventName == "Login"){
                database.all(sqlSelect, (err, rows)=>{
                    if(err){
                        console.log(err);
                    }
                    else{
                        if(rows.length > 0){
                            var callbackMsg = {
                                eventName:"Login",
                                data:"success"
                            }
                            //===============================================
                            var toJsonStr = JSON.stringify(callbackMsg);
                            ws.send(toJsonStr);
                            //===============================================
                            console.log("[2]" +toJsonStr);
                            //===============================================
                            /*database.all("SELECT UserName FROM userData WHERE UserID='"+userID+"'", ()=>{
                                var UserSendBackToClient = {
                                    eventName:"ShowUserName",
                                    data:"" + rows[0].UserName
                                }
                                //===============================================
                                var toJsonUser = JSON.stringify(UserSendBackToClient);
                                ws.send(toJsonUser);
                                //===============================================
                                console.log("[3]" +toJsonUser);
                                //===============================================
                            });*/
                        }
                        else{
                            var callbackMsg = {
                                eventName:"Login",
                                data:"fail"
                            }
                            //===============================================
                            var toJsonStr = JSON.stringify(callbackMsg);
                            ws.send(toJsonStr);
                            //=======================================================
                            console.log("[2]" +toJsonStr);
                        }
                    }
                })
            }
            else if (toJsonObj.eventName == "SendMessage"){
                Boardcast(ws, data);
                console.log("Send From Client : " + data);
            }
        });        
    
        ws.on("close", ()=>{
            wsList = ArrayRemove(wsList, ws);
            console.log("Client Disconnected");
        });
    });
});

function ArrayRemove(arr, value){
    return arr.filter((element)=>{
        return element != value;
    })
}

function Boardcast(ws, message)
{
    var selectRoomIndex = -1;

    for (var i = 0; i < roomList.length; i++)
    {
        for (var j = 0; j < roomList[i].wsList.length; j++)
        {
            if (ws == roomList[i].wsList[j])
            {
                selectRoomIndex = i;
                console.log(selectRoomIndex);
                break;
            }
        }
    }

    for (var i = 0; i < roomList[selectRoomIndex].wsList.length; i++)
    {
        roomList[selectRoomIndex].wsList[i].send(message);
    }
}