"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/rollHub").build();

document.getElementById("CreateSessionButton").disabled = true;

document.getElementById("CreateSessionButton").addEventListener("click", function (event) {
    var name = document.getElementById("partyNameInput").value;
    var password = document.getElementById("partyPasswordInput").value;
    connection.invoke("CreateSession", name, password);
    event.preventDefault();
});

connection.start().then(function () {
    document.getElementById("CreateSessionButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("FailedToCreateSession", function () {
    alert("Session could not be created");
});

connection.on("SessionCreated", function (partyId) {
    window.location.href = "party/" + partyId+"/gm";
});