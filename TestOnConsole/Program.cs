using Microsoft.Graph;
using Azure.Identity;
// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
var adress = Console.ReadLine();
var password = Console.ReadLine();
var scopes = new[] { "User.Read" };

// Multi-tenant apps can use "common",
// single-tenant apps must use the tenant ID from the Azure portal
var tenantId = "ce01eebb-6ef2-4f72-b4e4-42ec69a668c2";

// Value from app registration
var clientId = "453fa4c2-fc1a-45be-ba70-7c7dd5070f7a";
Console.WriteLine("Login start with" + adress + "," + password);
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
    Console.WriteLine("error," + ex.Message + "@Userlogin");
}
try
{
    IHttpProvider httpProvider
    var graphClient = new GraphServiceClient(userNamePasswordCredential, scopes,);
    //mailaddress
    var avate = await graphClient.Me.Request().Select("mail,displayName").GetAsync();
    Console.WriteLine(avate.Mail.ToString());
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
}
catch (Exception ex)
{
    Console.WriteLine("error," + ex.ToString());
}