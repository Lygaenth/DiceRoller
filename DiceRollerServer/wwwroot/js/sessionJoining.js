"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/rollHub").build();

document.getElementById("connectPlayerButton").disabled = true;

document.getElementById("connectPlayerButton").addEventListener("click", function (event) {
    var user = document.getElementById("userNameInput").value;
    var partyId = document.getElementById("partyIdInput").value;
    var password = document.getElementById("partyPasswordInput").value;
    connection.invoke("ConnectPlayer", user, partyId, password);
    event.preventDefault();
});

connection.start().then(function () {
    document.getElementById("connectPlayerButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("FailedToJoinSession", function () {
    alert("session does not exist or password is invalid");
});

connection.on("CanJoinSessionAsPlayer", function (partyId, userId, name) {
    window.location.href = `party/${partyId}/${userId}`;
});
