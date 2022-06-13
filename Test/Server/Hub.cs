using Microsoft.AspNetCore.SignalR;
namespace Test.Server.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.Caller.SendAsync("ReceiveMessage", user,"Sending..." );
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
