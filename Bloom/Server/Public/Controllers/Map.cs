using Microsoft.AspNetCore.SignalR;
using System.IO;
using Bloom.Server;
using Bloom.Server.Filer.Handler;
using Bloom.Server.Controllers;
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
    public class Map : Hub
    {
        /// <summary>
        /// フロアID 取得関数
        /// </summary>
        public async Task<string> ClaimFloorID(Building building,int floor)
        {
            var result = "Dev";
#if !DEBUG
            try
            {
                var floors = await FloorHandler.RetriveFloorList();
                foreach (var item in floors)
                {
                    if(item.building == building && item.fllor == floor)
                    {
                        result = item.id; 
                        break;
                    }
                }
            }
            catch(Exception ex)
            {
                var path = DirectoryManeger.GetAbsotoblePath("/logs/" + DateTime.UtcNow.ToString());
                Console.WriteLine("Warning! " + ex.Message + " *Log is at"+path);
                File.WriteAllText(path, ex.ToString());
            }
#endif
            try
            {
                await Clients.Caller.SendAsync("ReceiveFloorID", result);
            }
            catch(Exception ex)
            {
                var path = DirectoryManeger.GetAbsotoblePath("/logs/" + DateTime.UtcNow.ToString());
                Console.WriteLine("Warning! " + ex.Message + " *Log is at"+path);
                File.WriteAllText(path, ex.ToString(),Encoding.UTF8);
            }

            return result;
        }
        
        /// <summary>
        /// フロア本体取得関数
        /// </summary>
        public async Task<Floor> ClaimFloor(Building building, int floor)
        {
            var result = new Floor();
#if DEBUG
            result.id = "0";
#endif
#if !DEBUG
            try
            {
                var floors = await FloorHandler.RetriveFloorList();
                foreach (var item in floors)
                {
                    if (item.building == building && item.fllor == floor)
                    {
                        result = item;
                        break;
                    }
                }
            }
            catch(Exception ex)
            {
                var path = DirectoryManeger.GetAbsotoblePath("/logs/" + DateTime.UtcNow.ToString());
                Console.WriteLine("Warning! " + ex.Message + " *Log is at"+path);
                File.WriteAllText(path, ex.ToString());
            }
                    
#endif
            try
            {
                await Clients.Caller.SendAsync("ReceiveFloor", await result.ConvertToJson());
            }
            catch (Exception ex)
            {
                var path = DirectoryManeger.GetAbsotoblePath("/logs/" + DateTime.UtcNow.ToString());
                Console.WriteLine("Warning! " + ex.Message + " *Log is at" + path);
                File.WriteAllText(path, ex.ToString(), Encoding.UTF8);
            }

            return result;
        }        
        public async Task<Dictionary<Building, string[]>> ClaimBuildings()
        {
            var result = new Dictionary<Building, string[]>();
#if DEBUG
            result.Add(Building.Gernal, new string[0]);
            result.Add(Building.Central, new string[2] {"3","中央棟"});
#endif
#if !DEBUG
            try
            {
                var indexer = await Filer.GetFileText("/data/FloorIndexer.json");
                var path = "/tmp/" + Guid.NewGuid().ToString() + ".txt";
                await File.WriteAllTextAsync(DirectoryManeger.GetAbsotoblePath(path), indexer);//検索結果をいったんファイルに
                //Close時に削除する一時ファイルを作成
                using (FileStream sr = File.Create(
                    DirectoryManeger.GetAbsotoblePath(path),
                    Encoding.UTF8.GetBytes(indexer).Length,
                    FileOptions.DeleteOnClose))
                {
                    BuildingExpression?[] dic = await JsonSerializer.DeserializeAsync<BuildingExpression[]>(sr);
                    if (dic != null || dic.Length != 0)
                    {
                        foreach (var item in dic)
                        {
                            result.Add(item.building, new string[2] { item.paths.Values.Count.ToString(), item.name });
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                var path = DirectoryManeger.GetAbsotoblePath("/logs/" + DateTime.UtcNow.ToString());
                Console.WriteLine("Warning! " + ex.Message + " *Log is at"+path);
                File.WriteAllText(path, ex.ToString());
            }
                    
#endif
            try
            {
                var path = "/tmp/" + Guid.NewGuid().ToString() + ".txt";
                //Close時に削除する一時ファイルを作成
                using (FileStream fs = File.Create(DirectoryManeger.GetAbsotoblePath(path)))
                {
                    await JsonSerializer.SerializeAsync<Dictionary<Building, string[]>>(fs, result);
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        await Clients.Caller.SendAsync("ReceiveBuildings", sr.ReadToEnd());
                    }
                }
                File.Delete(DirectoryManeger.GetAbsotoblePath(path));
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
