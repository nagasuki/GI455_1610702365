var websocket = require('ws');

var websocketServer = new websocket.Server({port:8080}, ()=>{
    console.log("Mano Chat Server is running");
});

var wsList = [];
var usernameList = [];

websocketServer.on("connection", (ws, rq)=>{
    console.log('client connected');

    usernameList.push(ws);
    wsList.push(ws);

    ws.on("message", (data)=>{
        console.log("semd from client : "+data);
        Boardcast(data);
    });

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