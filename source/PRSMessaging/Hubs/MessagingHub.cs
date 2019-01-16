using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRSMessaging.Hubs
{
    public class MessagingHub : Hub
    {
        private static List<MessagingClient> MessagingClients = new List<MessagingClient>();

        private bool IsConnectionIdExist(string ConnectionId)
        {
            if (MessagingClients.Where(f => f.ConnectionId == ConnectionId).Any())
                return true;

            return false;
        }

        public async Task JoinRoom(string room, string subject)
        {
            if (!IsConnectionIdExist(Context.ConnectionId))
            {
                MessagingClients.Add(new MessagingClient
                {
                    ConnectionId = Context.ConnectionId,
                    Room = room,
                    Subject = subject
                });
            }

            List<string> listNames = MessagingClients.Where(f => f.Room == room).Select(m => m.Subject).ToList();
            var messEvent = new MessagingEvent
            {
                Event = MessagingEvents.Joined,
                Data = listNames
            };

            await Groups.AddToGroupAsync(Context.ConnectionId, room);
            await Clients.Group(room).SendAsync("onJoinRoom", messEvent);
        }

        public Task LeaveRoom(string room)
        {
            MessagingClient client = MessagingClients.Where(f => f.ConnectionId == Context.ConnectionId).First();
            MessagingClients.Remove(client);

            List<string> listNames = MessagingClients.Where(f => f.Room == room).Select(m => m.Subject).ToList();
            var messEvent = new MessagingEvent
            {
                Event = MessagingEvents.Joined,
                Data = listNames
            };

            Clients.Group(room).SendAsync("onLeaveRoom", messEvent);
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, room);
        }

        public async Task SendData(dynamic data)
        {
            List<string> rooms = MessagingClients.Where(f => f.ConnectionId == Context.ConnectionId).Select(m => m.Room).ToList();

            var messEvent = new MessagingEvent
            {
                Event = MessagingEvents.Data,
                Data = data
            };

            await Clients.Groups(rooms).SendAsync("onData", messEvent);
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            string room = MessagingClients.Where(f => f.ConnectionId == Context.ConnectionId).Select(m => m.Room).First();
            MessagingClient client = MessagingClients.Where(p => p.ConnectionId == Context.ConnectionId).First();

            MessagingClients.Remove(client);

            List<string> listNames = MessagingClients.Where(f => f.Room == room).Select(m => m.Subject).ToList();
            var messEvent = new MessagingEvent
            {
                Event = MessagingEvents.Joined,
                Data = listNames
            };

            Clients.Group(room).SendAsync("onLeaveRoom", messEvent);
            Clients.Group(room).SendAsync("onDisconnectRoom", messEvent);

            return base.OnDisconnectedAsync(exception);
        }
    }


    public class MessagingClient
    {
        public string ConnectionId { get; set; }
        public string Room { get; set; }
        public string Subject { get; set; }
    }

    public class MessagingEvent
    {
        public MessagingEvent() { }
        public string Event { get; set; }
        public dynamic Data { get; set; }
    }

    public static class MessagingEvents
    {
        public static readonly string Joined = "joined";
        public static readonly string Disconnected = "disconnected";
        public static readonly string Data = "data";
    }

    public class MessagingData
    {
        public string @Type { get; set; }
        public dynamic Data { get; set; }
    }
}
