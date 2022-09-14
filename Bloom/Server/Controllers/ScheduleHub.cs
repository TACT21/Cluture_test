using Microsoft.AspNetCore.SignalR;
using System.IO;
using Bloom.Server;
using Bloom.Server.Controllers;
using System.Text;
using Bloom.Shared;
using System.Xml.Linq;
using System.Text.Json;
using SixLabors.ImageSharp;
using System.Xml.Serialization;
using System.IO;

namespace Bloom.Server.Controllers
{
    public class ScheduleHub : Hub
    {
        public async Task Claim()
        {
#if DEBUG
            await SendMapImageDev();
#endif
#if !DEBUG
                await SendMapImage();
#endif
        }

        private async Task SendMapImage()
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
                await SendMapImageDev();
            }
        }
        private async Task SendMapImageDev()
        {
            await Clients.Caller.SendAsync("ReceiveImages", "");
        }
    }
}
