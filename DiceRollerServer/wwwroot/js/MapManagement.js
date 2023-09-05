"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var signalR = require("@microsoft/signalr");
require("");
var connection = new signalR.HubConnectionBuilder().withUrl("/rollHub").build();
var movedElement;
var currentbackground;
currentbackground = document.getElementById("backgroundListId").nodeValue;
dragElement(document.getElementById("charac_1_id"));
function dragElement(elmnt) {
    var pos1 = 0, pos2 = 0, pos3 = 0, pos4 = 0;
    movedElement = elmnt;
    if (document.getElementById(elmnt.id)) {
        // if present, the header is where you move the DIV from:
        document.getElementById(elmnt.id).onmousedown = dragMouseDown;
    }
    else {
        // otherwise, move the DIV from anywhere inside the DIV:
        elmnt.onmousedown = dragMouseDown;
    }
    function dragMouseDown(e) {
        e = e || window.MouseEvent;
        e.preventDefault();
        // get the mouse cursor position at startup:
        pos3 = e.clientX;
        pos4 = e.clientY;
        document.onmouseup = closeDragElement;
        // call a function whenever the cursor moves:
        document.onmousemove = elementDrag;
    }
    function elementDrag(e) {
        e = e || window.DragEvent;
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
        var path = document.getElementById("backgroundListId").nodeValue;
        if (path != currentbackground) {
            currentbackground = path;
            connection.invoke("LoadBackground", path);
        }
    });
    connection.on("UpdatedBackground", function (background, tileNumber) {
        var map = document.getElementById("background_id");
        map.src = background;
        for (var index = 0; index < map.parentNode.children.length; index++) {
            var child = map.parentNode.childNodes[index];
            if (child instanceof HTMLImageElement && child.id.startsWith("charac")) {
                var token = child;
                token.style.width = String(map.width / tileNumber);
            }
        }
    });
}
