import * as mapManager from './MapManagement.js'; 
import * as chatRoom from './ChatRoom.js';
import * as userListManager from './UserListManagement.js';

var connection = new signalR.HubConnectionBuilder().withUrl("/rollHub").build();

chatRoom.Initialize(connection);
mapManager.Initialize(connection);
userListManager.Initialize(connection);

connection.start().then(function () {
    chatRoom.Enable();
    mapManager.Enable();

    JoinSession();
}).catch(function (err) {
    return console.error(err.toString());
});

function JoinSession() {
    var partyId = location.pathname.split('/')[2];
    var userId = location.pathname.split('/')[3];
    if (userId == 'gm')
        connection.invoke("JoinGm", partyId);
    else
        connection.invoke("JoinPlayer", partyId, userId);
}


