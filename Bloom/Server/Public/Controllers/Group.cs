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
using Bloom.Server.Filer.Handler;

namespace Bloom.Server.Hubs
{
    public class CompanyHub : Hub
    {
        public async Task<Company> ClaimCompany(string id)
        {
            var result = new Company();
#if DEBUG
            await RetrieveGroupDev();
#endif
#if !DEBUG
            result = await CompanyHandler.RetrieveGroup(id);
#endif
            return result;
        }
        public async static Task<Company> RetrieveGroupShoten(string id)
        {
            var result = new Company();
            var company = await CompanyHandler.RetrieveGroup(id);
            result = company;
            result.videoUrl = null;
            result.contentUrl = null;
            result.cmUrl = null;
            result.enname = null;
            result.location = null;
            return result;
        }
        private async Task<Company> RetrieveGroupDev()
        {
            var result = new Company ();
            result.name = "団体名";
            result.enname = "Company name";
            result.comment = "Here is coment";
            result.location = "<h1>よくわからん</h1>,0";
            Console.WriteLine("SerializeGroup!");
            return result;
        }
    }
}
