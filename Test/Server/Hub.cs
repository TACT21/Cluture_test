using Microsoft.AspNetCore.SignalR;
using Microsoft.Graph;
using Azure.Identity;
using System.Diagnostics;
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
        public async Task MicrosoftLogin(string userName, string password)
        {
            var scopes = new[] { "User.Read" };

            // Multi-tenant apps can use "common",
            // single-tenant apps must use the tenant ID from the Azure portal
            var tenantId = "common";

            // Value from app registration
            var clientId = "YOUR_CLIENT_ID";

            // using Azure.Identity;
            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };

            // https://docs.microsoft.com/dotnet/api/azure.identity.usernamepasswordcredential
            var userNamePasswordCredential = new UsernamePasswordCredential(
                userName, password, tenantId, clientId, options);

            var graphClient = new GraphServiceClient(userNamePasswordCredential, scopes);
        }
        public async Task GoogleLogin(string message)
        {
            Guid myUUId = Guid.NewGuid();
            await Clients.Client(message.Split(",")[2]).SendAsync("LoginCorect", message + "," + Guid.NewGuid().ToString());
            await Clients.Caller.SendAsync("LoginIsSended");
        }
        
        public async Task ConectionHelp()
        {
            await Clients.Caller.SendAsync("ConectionOK");
        }
    }
}
