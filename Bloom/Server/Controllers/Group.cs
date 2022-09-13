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

namespace Bloom.Server.Hubs
{
    public class Company : Hub
    {
        public async Task ClaimGroup(string str)
        {
#if DEBUG
            await SendGroupDev();
#endif
#if !DEBUG
                await SendGroup();
#endif
        }

        private async Task SendGroup()
        {
            string result = string.Empty;
            List<string[]> files = new List<string[]>();
            var filePath = DirectoryManeger.GetAbsotoblePath("/map/img_list.csv");
            if (File.Exists(filePath))
            {
                StreamReader sr = new StreamReader(filePath, Encoding.UTF8);
                while (!sr.EndOfStream)
                {
                    try
                    {
                        files.Add(sr.ReadLine().Split(","));
                    }
                    catch { }
                }
                sr.Close();
                foreach (var item in files)
                {
                    var a = File.Open(item[1], FileMode.OpenOrCreate);
                    using (var stream = new MemoryStream())
                    {
                        a.CopyTo(stream);
                        result += (item[0] + "\a data:image/jpeg;base64," + Convert.ToBase64String(stream.ToArray()) + "\n");
                    }
                    a.Close();
                }
                await Clients.Caller.SendAsync("ReceiveImages", result);
            }
            else
            {
                File.Create(filePath).Close();
                await SendGroupDev();
            }
        }
        private async Task SendGroupDev()
        {
            Console.WriteLine("SendGroupDev");
            var result = new Group ();
            result.name = "団体名";
            result.enname = "Company name";
            result.comment = "Here is coment";
            result.location = "<h1>よくわからん</h1>,0";
            Console.WriteLine("SerializeGroup!");
            using (MemoryStream ms = new MemoryStream())
            {
                XmlSerializer xs = new XmlSerializer(typeof(Group));
                xs.Serialize(ms, result);
                await Clients.Caller.SendAsync("ReceiveGroup", Encoding.UTF8.GetString(ms.ToArray()));
            }
        }

  
    }

}
