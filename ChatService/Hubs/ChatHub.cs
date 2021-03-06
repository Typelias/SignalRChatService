using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatService.Hubs
{
    public class ChatHub : Hub
    {
        private readonly string _botUser;
        private readonly IDictionary<string, UserConnection> _connections;

        public ChatHub(IDictionary<string, UserConnection> connections)
        {
            _botUser = "MyChat Bot";
            _connections = connections;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if(_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                _connections.Remove(Context.ConnectionId);
                Clients.Group(userConnection.Room).SendAsync("ReceivedMessage", _botUser, $"{userConnection.User} has left");
                SendConnectedUsers(userConnection.Room);
            }
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string message)
        {
            if(_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                await Clients.Group(userConnection.Room).SendAsync("ReceivedMessage", userConnection.User, message);
            }
        }

        public async Task SendWebRTC(String user, NodeDssMessage message)
        {
            

            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                System.Diagnostics.Debug.WriteLine("HEJ");
                System.Diagnostics.Debug.WriteLine(userConnection.User);
                System.Diagnostics.Debug.WriteLine(message.MessageType);
                await Clients.Group(userConnection.Room).SendAsync("WebRTC", user, message);
            }
        }

        public async Task JoinRoom(UserConnection userConnection)
        {
            System.Diagnostics.Debug.WriteLine(userConnection.ToString());
            await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.Room);
            _connections[Context.ConnectionId] = userConnection;
            await Clients.Group(userConnection.Room).SendAsync("ReceivedMessage", _botUser, $"{userConnection.User} has joined");
            await SendConnectedUsers(userConnection.Room);
        }

        public Task SendConnectedUsers(string room)
        {
            var users = _connections.Values.Where(c => c.Room == room).Select(c => c.User);
            return Clients.Group(room).SendAsync("UsersInRoom", users);
        }
    }
}
