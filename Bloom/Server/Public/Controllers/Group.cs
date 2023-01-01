using Microsoft.AspNetCore.SignalR;
using System.IO;
using Bloom.Server;
using System.Text;
using Bloom.Shared;
using System.Xml.Linq;
using System.Text.Json;
using SixLabors.ImageSharp;
using System.Xml.Serialization;
using System.IO;
using Bloom.Server.Utility;
using Bloom.Server.Utility.Format;

namespace Bloom.Server.Hubs
{
    public class Company : Hub
    {
        public async Task ClaimGroup(string id)
        {
#if DEBUG
            await RetrieveGroupDev();
#endif
#if !DEBUG
            await RetrieveGroup(id);
#endif
        }
        public async static Task<Group> RetrieveGroup(string id)
        {
            var result = new Group();
            using (var sr = new FileStream(DirectoryManeger.GetAbsotoblePath("/data/groups/" + id + ".json"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var org = await JsonSerializer.DeserializeAsync<GroupExpression>(sr);
                if(org != null)
                {
                    result = await org.ConvertToGroup(false);
                }
            }
            return result;
        }
        public async static Task<Group> RetrieveGroupShoten(string id)
        {
            var result = new Group();
            using (var sr = new FileStream(DirectoryManeger.GetAbsotoblePath("/data/group/" + id), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var org = await JsonSerializer.DeserializeAsync<GroupExpression>(sr);
                if (org != null)
                {
                    var task = org.ConvertToGroup(true);
                    Media? poster = new();
                    foreach (var url in org.posterUrl)
                    {
                        using (var psr = new FileStream(url, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            poster = await JsonSerializer.DeserializeAsync<Media>(psr);
                        }
                        if (poster != null)
                        {
                            break;
                        }
                    }
                    result = await task;
                    result.posterUrl.Add(poster);
                }
            }
            return result;
        }
        private async Task<Group> RetrieveGroupDev()
        {
            var result = new Group ();
            result.name = "団体名";
            result.enname = "Company name";
            result.comment = "Here is coment";
            result.location = "<h1>よくわからん</h1>,0";
            Console.WriteLine("SerializeGroup!");
            return result;
        }
    }
}
