using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreMvcWeb.Hubs.Chat
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendMessageToCaller(string message)
        {
            await Clients.Caller.SendAsync("ReceiveMessage", "System", message);
        }

        public async Task SendMessageToGroups(string message)
        {
            List<string> groups = new List<string>() { "SignalR Users" };
            await Clients.Groups(groups).SendAsync("ReceiveMessage", message);
        }

        public override async Task OnConnectedAsync()
        {
            /*
            if (Context.User.Identity.IsAuthenticated == false)
            {
                await SendMessageToCaller("접근 권한 제어 - 너 차단");
                await Clients.Caller.SendAsync("NoAuth");
                Context.Abort();
            }
            else
            {

            }
            */
            await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnConnectedAsync();
            await Clients.All.SendAsync("ReceiveMessage", "System", "입장 알림");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
            await Clients.All.SendAsync("ReceiveMessage", "System", "퇴장 알림");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
