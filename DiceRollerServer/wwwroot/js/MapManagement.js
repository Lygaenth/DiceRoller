"use strict";

var backgroundList;
var mapConnection;
var currentBackground;
var mapViewNode;
var tileWidth = 40;
var partyId = location.pathname.split('/')[2];
var userId = location.pathname.split('/')[3];

export function Initialize(connection) {
    mapConnection = connection;
    backgroundList = document.getElementById("backgroundListId");
    mapViewNode = document.getElementById("mapViewId");

    if (backgroundList != null) {
        currentBackground = backgroundList.value;
        backgroundList.addEventListener("click", function () {
            if (backgroundList.value != currentBackground) {
                currentBackground = backgroundList.value;
                mapConnection.invoke("LoadBackground", partyId, currentBackground);
            }
        });
    }

    mapConnection.on("ImageMoved", function (elementId, x, y) {
        var element = document.getElementById("charac_"+elementId+"_id");
        if (element == undefined)
            return;
        element.style = "position:absolute; left:"+x+"px; top:"+y+"px;max-width:" + tileWidth + "px"
    });

    mapConnection.on("UpdatedBackground", function (background, tileNumber) {
        var map = document.getElementById("background_id");
        map.src = background;
        tileWidth = (map.width / tileNumber);
        for (var child of map.parentNode.children) {
            if (child.id.startsWith("charac"))
                child.style = "position:absolute; left:0px; top:0px;max-width:" + tileWidth + "px";
        }
    });

    mapConnection.on("UsersUpdated", function (users) {
        RefreshUsersPositions(users);
    });

    Disable();
}

function RefreshUsersPositions(users) {
    var userList = users.split("|");

    var mapChildren = mapViewNode.querySelectorAll("*");
    for (var node of mapChildren) {
        if (node.id.startsWith("charac"))
            mapViewNode.removeChild(node);
    }

    for (var user of userList) {
        var prop = user.split(";");

        var element = document.createElement("IMG");
        element.id = "charac_" + prop[0] + "_id";
        element.style = "position: absolute; left: " + prop[5] + "px; top: " + prop[6] + "px; max-width: " + tileWidth+"px";
        element.src = prop[4];
        element.draggable = true;
        mapViewNode.appendChild(element);
        if (userId == String(prop[0]) || userId == "gm")
            DragElement(element);
    }
}



export function Enable() {
    if (backgroundList != null) {
        backgroundList.disabled = false;
        currentBackground = backgroundList.value;
    }
    var element = document.getElementById("charac_" + userId + "_id"); 
    if (element != null)
        DragElement(element, mapConnection);
}

function Disable() {
    if (backgroundList != null)
        backgroundList.Disabled = true;

}

function DragElement(elmnt) {
    var movedElement;

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
    };

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
    };

    function closeDragElement() {

        // stop moving when mouse button is released:
        document.onmouseup = null;
        document.onmousemove = null;
        mapConnection.invoke("MoveImage", partyId, GetCharacterId(movedElement.id), movedElement.style.left.substring(0, movedElement.style.left.length - 2), movedElement.style.top.substring(0, movedElement.style.top.length - 2));
    };

    function GetCharacterId(id) {
        if (id.startsWith("charac"))
            return id.split('_')[1]; 
    }
};


