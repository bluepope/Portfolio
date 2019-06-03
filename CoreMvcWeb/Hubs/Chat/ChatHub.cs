using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreMvcWeb.Extentions;

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

        /// <summary>
        /// 인증이 안된 사용자에게 보내는 메시지
        /// </summary>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        private async Task SendMessageToIsNotAuthenticated(string sendMessage)
        {
            await Clients.All.SendAsync("NoAuth", sendMessage);
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
            if (!Context.User.Identity.IsAuthenticated)
            {
                await SendMessageToIsNotAuthenticated("비로그인님 채팅방에 입장 못합니다.");
            }
            else
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
                await base.OnConnectedAsync();
                await Clients.All.SendAsync("ReceiveMessage", "System", $"{Context.User.Identity.Name}님 입장 알림");
            }

        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
            await Clients.All.SendAsync("ReceiveMessage", "System", $"{Context.User.Identity.Name}님 퇴장 알림");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
