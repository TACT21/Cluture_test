using Microsoft.AspNetCore.SignalR;
using System.IO;
using Bloom.Server;
using Bloom.Server.Controllers;
using Bloom.Server.Utility;
using Bloom.Server.Utility.Format;
using Bloom.Server.Filer.Handler;
using System.Text;
using Bloom.Shared;
using System.Xml.Linq;
using System.Text.Json;
using SixLabors.ImageSharp;
using System.Xml.Serialization;
using System.IO;

namespace Bloom.Server.Hubs
{
    public class GreadHub :Hub
    {
        public async Task<Floor> ClaimGread(int gread)
        {
            var result = new Floor();
#if DEBUG
            result = new Floor()
            {
                floorTitle = "Dev"
            };
#endif
#if !DEBUG
            try
            {
                var greads = await GreadHandler.RetriveGreadList();
                foreach (var item in greads)
                {
                    if(item.fllor == gread)
                    {
                        result = item;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                var path = DirectoryManeger.GetAbsotoblePath("/logs/" + DateTime.UtcNow.ToString());
                Console.WriteLine("Warning! " + ex.Message + " *Log is at" + path);
                File.WriteAllText(path, ex.ToString());
            }
#endif
            try
            {
                await Clients.Caller.SendAsync("ReceiveFloorID", await result.ConvertToJson());
            }
            catch (Exception ex)
            {
                var path = DirectoryManeger.GetAbsotoblePath("/logs/" + DateTime.UtcNow.ToString());
                Console.WriteLine("Warning! " + ex.Message + " *Log is at" + path);
                File.WriteAllText(path, ex.ToString(), Encoding.UTF8);
            }

            return result;
        }

        public async Task<Floor[]> ClaimGreads()
        {
            var result = new List<Floor>();
#if DEBUG
            result.Add(0, "Dev");
#endif
#if !DEBUG
            try
            {
                var greads = await GreadHandler.RetriveGreadList();
                foreach (var item in greads)
                {
                    result.Add(item);
                }
            }
            catch (Exception ex)
            {
                var path = DirectoryManeger.GetAbsotoblePath("/logs/" + DateTime.UtcNow.ToString());
                Console.WriteLine("Warning! " + ex.Message + " *Log is at" + path);
                File.WriteAllText(path, ex.ToString());
            }
#endif
            try
            {
                await Clients.Caller.SendAsync("ReceiveFloorID", result);
            }
            catch (Exception ex)
            {
                var path = DirectoryManeger.GetAbsotoblePath("/logs/" + DateTime.UtcNow.ToString());
                Console.WriteLine("Warning! " + ex.Message + " *Log is at" + path);
                File.WriteAllText(path, ex.ToString(), Encoding.UTF8);
            }

            return result.ToArray();
        }
    }
}
