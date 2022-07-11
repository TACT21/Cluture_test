using Microsoft.AspNetCore.SignalR;
namespace Test.Server.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.Caller.SendAsync("ReceiveMessage", user, "Sending...");
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }

    public class LoginHub : Hub
    {
        public async Task MicrosoftLogin(string user, string message)
        {

        }
        public async Task GoogleLogin(string message)
        {
            await Clients.Client(message.Split(",")[2]).SendAsync("LoginCorect", message);
            Console.WriteLine(message);
            await Clients.Caller.SendAsync("LoginIsSended");
        }
        
        public async Task ConectionHelp()
        {
            await Clients.Caller.SendAsync("ConectionOK");
        }
    }
}
