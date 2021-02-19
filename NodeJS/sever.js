var websocket = require('ws');

var websocketServer = new websocket.Server({port:8080}, ()=>{
    console.log("Mano Chat Server is running");
});

var wsList = [];
var roomList = [];

websocketServer.on("connection", (ws)=>{
    
    //Lobby
    console.log("client connected.");
    //Reception
    ws.on("message", (data)=>{
        console.log("send from client :"+ data);

        //========== Convert jsonStr into jsonObj =======

        //toJsonObj = JSON.parse(data);

        // I change to line below for prevent confusion
        var toJsonObj = { 
            roomName:"",
            data:""
        }
        toJsonObj = JSON.parse(data);
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

                //ws.send("CreateRoomFail"); 

                //I will change to json string like a client side. Please see below
                var callbackMsg = {
                    eventName:"CreateRoom",
                    data:"fail"
                }
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

                //ws.send("CreateRoomSuccess");

                //I need to send roomName into client too. I will change to json string like a client side. Please see below
                var callbackMsg = {
                    eventName:"CreateRoom",
                    data:toJsonObj.data
                }
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

                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);

                    console.log("client join room success.");
                    console.log("client request JoinRoom ["+toJsonObj.data+"]");
                }
                else
                {
                    var callbackMsg = {
                        eventName:"JoinRoom",
                        data:"fail",
                    }

                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);

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

                //ws.send("LeaveRoomSuccess");

                //I will change to json string like a client side. Please see below
                var callbackMsg = {
                    eventName:"LeaveRoom",
                    data:"success"
                }
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
                var toJsonStr = JSON.stringify(callbackMsg);
                ws.send(toJsonStr);
                //=======================================================

                console.log("leave room fail");
            }
        }
    });
wsList.push(ws);

    /*ws.on("message", (data)=>{
        console.log("send from client : "+ data);
        Boardcast(data);
    });*/

    ws.on("close", ()=>{
        wsList = ArrayRemove(wsList, ws);
        console.log("Client Disconnected");
    });
});

function ArrayRemove(arr, value){
    return arr.filter((element)=>{
        return element != value;
    })
}

function Boardcast(data){
    for(var i = 0; i < wsList.length; i++){
        wsList[i].send(data);
    }
}