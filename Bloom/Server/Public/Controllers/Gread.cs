using Microsoft.AspNetCore.SignalR;
using System.IO;
using Bloom.Server;
using Bloom.Server.Controllers;
using Bloom.Server.Utility;
using Bloom.Server.Utility.Format;
using System.Text;
using Bloom.Shared;
using System.Xml.Linq;
using System.Text.Json;
using SixLabors.ImageSharp;
using System.Xml.Serialization;
using System.IO;

namespace Bloom.Server.Hubs
{
    public class Gread :Hub
    {
        public async Task<Floor> ClaimGread(int gread)
        {
            var result = new Floor();
#if DEBUG
            result = await RetriveGreadDev();
#endif
#if !DEBUG
            try
            {
                result = await RetriveGread(gread);
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
        private async Task<Floor> RetriveGread(int greadNumber)
        {
            var greads = new GreadExpression[0];
            var result = new Floor();
            using (var sr = new FileStream(DirectoryManeger.GetAbsotoblePath("/gread_indexer.json"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                greads = await JsonSerializer.DeserializeAsync<GreadExpression[]>(sr);
            }
            if(greads == null)
            {
                return RetriveGreadDev();
            }
            foreach (var item in greads)
            {
                if(item.gread == greadNumber)
                {
                    result.id = item.id;
                    result.floorTitle = item.name;
                    var tasks = new List<Task<Group>>();
                    foreach (var group in item.groups)
                    {
                        tasks.Add(Company.RetrieveGroupShoten(group));
                    }
                    var groups = await Task.WhenAll(tasks);
                    result.groups = new List<Group>(groups);
                }
            }
            return result;
        }
        private Floor RetriveGreadDev()
        {
            var floor = new Floor();
            floor.floorTitle = "Dev";
            return floor;
        }
        public async Task<Dictionary<int,string>> ClaimGreads()
        {
            var result = new Dictionary<int, string>();
#if DEBUG
            result.add(0,"Dev")
#endif
#if !DEBUG
            try
            {
                var greads = new GreadExpression[0];
                using (var sr = new FileStream(DirectoryManeger.GetAbsotoblePath("/gread_indexer.json"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    greads = await JsonSerializer.DeserializeAsync<GreadExpression[]>(sr);
                }
                foreach (var item in greads)
                {
                    result.Add(item.gread, item.name);
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

            return result;
        }
    }
}
