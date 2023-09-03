"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/rollHub").build();
var movedElement
var currentbackground = document.getElementById("backgroundListId").value;

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
    JoinSession();
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("ReceiveMessage", function (user, message) {
    AddMessage(`${user} says ${message}`);
});

connection.on("UsersUpdated", function (users) {
    UpdateUsersList(users);
});

function UpdateUsersList(users) {
    var userList = users.split("|");

    var usersNode = document.getElementById("usersListId");
    while (usersNode.firstChild)
        usersNode.removeChild(usersNode.lastChild);

    for (var user of userList) {
        var prop = user.split(";");

        var userId = location.pathname.split('/')[3];
        if (prop[0] != userId && "gm" != userId) {
            CreateUserRow(usersNode, prop[0], prop[1], prop[3], prop[2], false);
        }
        else {
            CreateUserRow(usersNode, prop[0], prop[1], prop[3], prop[2], true);
        }
    }
}

function CreateUserRow(node, id, name, currentHp, hpMax, editable) {

    var userDiv = document.createElement("div")
    userDiv.id = "player_" + id;
    userDiv.style = "margin:5px; border:solid; border-width:thin; border-color:lightblue; padding:5px";
    node.appendChild(userDiv);

    var divName = document.createElement("div");
    var labelName = document.createElement("label");
    labelName.textContent = name + ": ";
    divName.appendChild(labelName);
    userDiv.appendChild(divName);

    var divHp = document.createElement("div")

    if (editable) {
        var input = document.createElement("input");
        input.type = "text";
        input.id = "hpInput";
        input.size = 3;
        input.value = currentHp;
        divHp.appendChild(input);
    }
    else {
        var labelCurrentHp = document.createElement("label");
        labelCurrentHp.textContent = currentHp;
        divHp.appendChild(labelCurrentHp);
    }
    var labelHp = document.createElement("label");
    labelHp.textContent = " / " + hpMax + " HP";
    
    divHp.appendChild(labelHp);
    userDiv.appendChild(divHp);
}

connection.on("JoinedUser", function (partyId, name) {
    AddMessage(`${name} joined the room`);
    AutoScrollDown();
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
    AddMessage(`${user} rolls ${message} on D${die}`);
});

function AddMessage(message) {
    var textArea = document.getElementById("messagesList");
    textArea.value += `${message}\r\n`;
    textArea.scrollTop = textArea.scrollHeight - textArea.clientHeight;
}

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

dragElement(document.getElementById("charac_1_id"));

function dragElement(elmnt) {
    var pos1 = 0, pos2 = 0, pos3 = 0, pos4 = 0;
    movedElement = elmnt;

    if (document.getElementById(elmnt.id)) {
        // if present, the header is where you move the DIV from:
        document.getElementById(elmnt.id).onmousedown = dragMouseDown;
    } else {
        // otherwise, move the DIV from anywhere inside the DIV:
        elmnt.onmousedown = dragMouseDown;
    }

    function dragMouseDown(e) {
        e = e || window.event;
        e.preventDefault();
        // get the mouse cursor position at startup:
        pos3 = e.clientX;
        pos4 = e.clientY;
        document.onmouseup = closeDragElement;
        // call a function whenever the cursor moves:
        document.onmousemove = elementDrag;
    }

    function elementDrag(e) {
        e = e || window.event;
        e.preventDefault();
        // calculate the new cursor position:
        pos1 = pos3 - e.clientX;
        pos2 = pos4 - e.clientY;
        pos3 = e.clientX;
        pos4 = e.clientY;
        // set the element's new position:
        elmnt.style.top = (elmnt.offsetTop - pos2) + "px";
        elmnt.style.left = (elmnt.offsetLeft - pos1) + "px";
    }

    function closeDragElement() {
        // stop moving when mouse button is released:
        document.onmouseup = null;
        document.onmousemove = null;
        connection.invoke("MoveImage", movedElement.id, movedElement.style.left, movedElement.style.top);
    }

    connection.on("ImageMoved", function (elementId, x, y) {
        var element = document.getElementById(elementId);
        if (element == undefined)
            return;
        element.style.left = x;
        element.style.top = y;
    });

    document.getElementById("backgroundListId").addEventListener("click", function () {
        var path = document.getElementById("backgroundListId").value;
        if (path != currentbackground) {
            currentbackground = path;
            connection.invoke("LoadBackground", path);
        }
    });

    connection.on("UpdatedBackground", function (background, tileNumber) {
        var map = document.getElementById("background_id");
        map.src = background;

        for(var child of map.parentNode.children) {
            if (child.id.startsWith("charac"))
                child.style = "position:absolute; left:0px; top:0px;width:"+(map.width / tileNumber);
        }
    });
}