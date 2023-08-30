"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/rollHub").build();

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
    JoinSession()
}).catch(function (err) {
    return console.error(err.toString());
});


connection.on("ReceiveMessage", function (user, message) {
    document.getElementById("messagesList").value += `${user} says ${message}\r\n`;
});

connection.on("UsersUpdated", function (users) {
    var userList = users.split("|");
    for (var user of userList) {
        var prop = user.split(";");
        var li = document.createElement("li");
        document.getElementById("usersList").appendChild(li);
        li.textContent = prop[1] + ": " + prop[2] + " HP";
    }
});

connection.on("JoinedUser", function (user) {
    document.getElementById("messagesList").value += `${user} joined\r\n`;
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var partyId = location.pathname.split('/')[2];
    var userId = location.pathname.split('/')[3];
    var message = document.getElementById("messageInput").value;

    connection.invoke("SendMessage", partyId, userId, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

document.getElementById("rollDie4").addEventListener("click", function (event) {
    RollDie(4);
    event.preventDefault();
});

document.getElementById("rollDie6").addEventListener("click", function (event) {
    RollDie(6);
    event.preventDefault();
});

document.getElementById("rollDie8").addEventListener("click", function (event) {
    RollDie(8);
    event.preventDefault();
});

document.getElementById("rollDie10").addEventListener("click", function (event) {
    RollDie(10);
    event.preventDefault();
});

document.getElementById("rollDie12").addEventListener("click", function (event) {
    RollDie(12);
    event.preventDefault();
});

document.getElementById("rollDie20").addEventListener("click", function (event) {
    RollDie(20);
    event.preventDefault();
});

connection.on("ReceiveRollResult", function (user, message, die) {
    document.getElementById("messagesList").value += `${user} rolls ${message} on D${die}\r\n`;
});

function RollDie(die) {
    var partyId = location.pathname.split('/')[2];
    var userId = location.pathname.split('/')[3];

    connection.invoke("RollDie", partyId, userId, die);
}

function JoinSession() {
    var partyId = location.pathname.split('/')[2];
    var userId = location.pathname.split('/')[3];
    connection.invoke("JoinPlayer", partyId, userId);
}