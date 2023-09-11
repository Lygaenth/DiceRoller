"use strict";

var chatConnection;
var sendButton;
var roll4;
var roll6;
var roll8;
var roll10;
var roll12;
var roll20;

function PostMessage() {
    var partyId = location.pathname.split('/')[2];
    var userId = location.pathname.split('/')[3];
    var message = document.getElementById("messageInput").value;

    chatConnection.invoke("SendMessage", partyId, userId, message).catch(function (err) {
        return console.error(err.toString());
    });
}

function AddMessage(message) {
    var textArea = document.getElementById("messagesList");
    textArea.value += `${message}\r\n`;
    textArea.scrollTop = textArea.scrollHeight - textArea.clientHeight;
};

export function Initialize(connection) {
    chatConnection = connection;
    sendButton = document.getElementById("sendButton")

    roll4 = document.getElementById("rollDie4");
    roll6 = document.getElementById("rollDie6");
    roll8 = document.getElementById("rollDie8");
    roll10 = document.getElementById("rollDie10");
    roll12 = document.getElementById("rollDie12");
    roll20 = document.getElementById("rollDie20");

    sendButton.addEventListener("click", function (event) {
        PostMessage(connection);
        event.preventDefault();
    });

    SubscribeToRoll(roll4, 4);
    SubscribeToRoll(roll6, 6);
    SubscribeToRoll(roll8, 8);
    SubscribeToRoll(roll10, 10);
    SubscribeToRoll(roll12, 12);
    SubscribeToRoll(roll20, 20);

    Disable();

    chatConnection.on("ReceiveMessage", function (user, message) {
        AddMessage(`${user} says: ${message}`);
    });

    chatConnection.on("JoinedUser", function (partyId, name) {
        AddMessage(`${name} joined the room`);        
    });

    chatConnection.on("ReceiveRollResult", function (user, message, die) {
        AddMessage(`${user} rolls ${message} on D${die}`);
    });
}

function SubscribeToRoll(die, value) {
    die.addEventListener("click", function (event) {
        RollDie(value);
        event.preventDefault();
    });
}

function RollDie(die) {
    var partyId = location.pathname.split('/')[2];
    var userId = location.pathname.split('/')[3];

    chatConnection.invoke("RollDie", partyId, userId, die);
}

function Disable() {
    sendButton.disabled = true;

    roll4.disabled = true;
    roll6.disabled = true;
    roll8.disabled = true;
    roll10.disabled = true;
    roll12.disabled = true;
    roll20.disabled = true;
}

export function Enable() {
    sendButton.disabled = false;

    roll4.disabled = false;
    roll6.disabled = false;
    roll8.disabled = false;
    roll10.disabled = false;
    roll12.disabled = false;
    roll20.disabled = false;
}