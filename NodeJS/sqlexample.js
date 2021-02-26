const sqlite = require('sqlite3').verbose();

var database = new sqlite.Database('./database/dbchat.db', sqlite.OPEN_CREATE | sqlite.OPEN_READWRITE, (err)=>{

    if(err) throw err;

    console.log("Connect to database");

    var userID = "test888";
    var password = "888888";
    var name = "test8";

    var sqlSelect = "SELECT * FROM userData WHERE UserID='test' AND UserPassword='111111'"// Login
    var sqlInsert =  `INSERT INTO userData (UserID, UserPassword, UserName) VALUES ('${userID}', '${password}', '${name}')`;// Register
    var sqlUpdate = "UPDATE userData SET Money=500 WHERE UserID='"+(userID)+"'";

    //var sqlAddMoney = "UPDATE userData SET Money='"+currentMoney+"' WHERE UserID='"+userID+"'";

    database.all(sqlSelect, (err, rows)=>{
        if(err){
            console.log(err)
        }
        else{
            if(rows.length > 0){
                console.log("Log in Success")

                /*database.all(sqlSelect, (err, rows)=>{
                    if(err){
                        console.log("Add Money Fail");
                    }
                    else{
                        var result ={
                            eventName: "Add Money",
                            data: currentMoney
                        }

                        console.log(JSON.stringify(result));
                    }
                })*/
            }
            else{
                //console.log("Log in Fail")
                console.log("UserID Not Found!")
            }
            console.log(rows);
        }
    });
});