"use strict";

var userListConnection;
var usersNode;

var partyId = location.pathname.split('/')[2];

export function Initialize(connection)
{
    userListConnection = connection;
    usersNode = document.getElementById("usersListId");
    userListConnection.on("UsersUpdated", function (users) {
        UpdateUsersList(users);
    });
}

function UpdateUsersList(users) {
    var userList = users.split("|");
     
    while (usersNode.firstChild)
        usersNode.removeChild(usersNode.lastChild);

    for (var user of userList) {
        var prop = user.split(";");

        var userId = location.pathname.split('/')[3];
        if (prop[0] != userId && "gm" != userId) {
            CreateUserRow(usersNode, prop[0], prop[1], prop[3], prop[2], prop[4], false);
        }
        else {
            CreateUserRow(usersNode, prop[0], prop[1], prop[3], prop[2], prop[4], true);
        }
    }
}


function CreateUserRow(node, id, name, currentHp, hpMax, url, editable) {

    var userDiv = document.createElement("div")
    userDiv.id = "player_" + id;
    userDiv.style = "margin:5px; border:solid; border-width:thin; border-color:lightblue; padding:5px";
    node.appendChild(userDiv);

    var divImage = document.createElement("div");
    divImage.style = "float:left; max-width:40px";
    var image = document.createElement("IMG");
    image.src = url;
    image.style = "max-width:50px";
    divImage.appendChild(image);
    userDiv.appendChild(divImage);

    var divName = document.createElement("div");
    divImage.style = "float:left;";
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
        input.oninput = function () { UpdateHp(input.value) }; 
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

function UpdateHp(value) {

    var user = location.pathname.split('/')[3];
    userListConnection.invoke("UpdateHp",partyId, user, value);
}
