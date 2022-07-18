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
        public async Task MicrosoftLogin(string adress, string password)
        {
            var scopes = new[] { "User.Read" };
            
            // Multi-tenant apps can use "common",
            // single-tenant apps must use the tenant ID from the Azure portal
            var tenantId = "ce01eebb-6ef2-4f72-b4e4-42ec69a668c2";

            // Value from app registration
            var clientId = "453fa4c2-fc1a-45be-ba70-7c7dd5070f7a";
            await Sendmessage("Login start with" + adress + "," + password);
            string avater = String.Empty;
            string name = String.Empty;
            UsernamePasswordCredential? userNamePasswordCredential = null;
            try
            {
                var options = new TokenCredentialOptions
                {
                    AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
                };
                // https://docs.microsoft.com/dotnet/api/azure.identity.usernamepasswordcredential
                userNamePasswordCredential = new UsernamePasswordCredential(
                    adress, password, tenantId, clientId, options);
            }
            catch (Exception ex)
            {
                await Sendmessage("error," + ex.Message+"@Userlogin");
            }
            try { 
                var graphClient = new GraphServiceClient(userNamePasswordCredential, scopes);
                //mailaddress
                var avate = await graphClient.Me.Request().Select("mail,displayName").GetAsync();
                await Sendmessage(avate.Mail.ToString());
                //name
                name = avate.DisplayName;
                //photo
                var photo = await graphClient.Me.Photo.Content.Request().GetAsync();
                using (photo)
                {
                    byte[] buf = new byte[32768]; // 一時バッファ
                    using (MemoryStream ms = new MemoryStream())
                    {
                        while (true)
                        {
                            int read = photo.Read(buf, 0, buf.Length);
                            if (read > 0)
                            {
                                ms.Write(buf, 0, read);
                            }
                            else
                            {
                                break;
                            }
                        }
                        // メモリ・ストリームの内容をUTF-8で文字列に格納
                        avater = System.Text.Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }catch (Exception ex)
            {
                if (ex.ToString().Contains("AADSTS50126"))
                {
                    Console.WriteLine("Inviled Password OR Addres.");
                }
                else
                {
                    await Sendmessage("error," + ex.ToString());
                }
            }
            Guid myUUId = Guid.NewGuid();
            var integretion = new IntegrationProvider.Provider();
            await integretion.Register(myUUId.ToString(), adress);
            await Clients.Caller.SendAsync("LoginCorect", adress + "," + name + ","+avater+"," + myUUId.ToString());
        }
        public async Task GoogleLogin(string message)
        {
            Guid myUUId = Guid.NewGuid();
            await Clients.Client(message.Split(",")[2]).SendAsync("LoginCorect", message[0]+","+message[1] + ",," + myUUId.ToString());
            var integretion = new IntegrationProvider.Provider();
            await integretion.Register(myUUId.ToString(), message[0].ToString());
            await Clients.Caller.SendAsync("LoginIsSended");
        }
        
        public async Task ConectionHelp()
        {
            await Clients.Caller.SendAsync("ConectionOK");
        }

        public async Task Sendmessage(string mess, string id = "")
        {
            Console.WriteLine(mess);
            if (id != "")
            {
                await Clients.Client(id).SendAsync("DebugLog", mess);
            }
            else
            {
                await Clients.All.SendAsync("DebugLog", mess);
            }
        }
    }
}
