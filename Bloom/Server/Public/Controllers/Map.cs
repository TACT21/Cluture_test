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
using Bloom.Server.Utility;
using Bloom.Server.Utility.Format;

namespace Bloom.Server.Hubs
{
    public class Map : Hub
    {
        /// <summary>
        /// フロアID 取得関数
        /// </summary>
        public async Task<Floor> ClaimFloorID(Building building,int floor)
        {
            var result = new Floor();
#if DEBUG
            result = await RetrieveFloorIDDev();
#endif
#if !DEBUG
            try
            {
                result = await RetrieveFloorID(building,floor);
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
                await Clients.Caller.SendAsync("ReceiveFloorID", await result.ConvertToJson());
            }
            catch(Exception ex)
            {
                var path = DirectoryManeger.GetAbsotoblePath("/logs/" + DateTime.UtcNow.ToString());
                Console.WriteLine("Warning! " + ex.Message + " *Log is at"+path);
                File.WriteAllText(path, ex.ToString(),Encoding.UTF8);
            }

            return result;
        }
        public async Task<Floor> RetrieveFloorIDDev()
        {
            var result = new Floor();
            result.id = "0";
            return result;
        }
        public async Task<Floor> RetrieveFloorID(Building building, int floor)
        {
            var result = await RetrieveFloorIDDev();
            try
            {
                var indexer = await Filer.GetFileText(DirectoryManeger.GetAbsotoblePath(await GetFroor(building,floor)));
                var path = "/tmp/"+ Guid.NewGuid().ToString()+".txt";
                //Close時に削除する一時ファイルを作成
                using (FileStream sr = File.Create(
                    DirectoryManeger.GetAbsotoblePath(path),
                    Encoding.UTF8.GetBytes(indexer).Length,
                    FileOptions.DeleteOnClose))
                {
                    await sr.WriteAsync(Encoding.UTF8.GetBytes(indexer));
                    Floor? fl = await JsonSerializer.DeserializeAsync<Floor>(sr);
                    if(fl != null)
                    {
                        result.id = fl.id;
                    }
                } 
            }
            catch
            {
                throw;
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
                result = await RetrieveFloor(building,floor);
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
        public async Task<Floor> RetrieveFloor(Building building, int floor)
        {
            var result = new Floor();
            result.id = "0";
            try
            {
                var indexer = await GetFroor(building,floor);
                FloorExpression? expression = new();
                using (var  sr = new FileStream(indexer, FileMode.Open,FileAccess.Read,FileShare.ReadWrite))
                {
                    expression = await JsonSerializer.DeserializeAsync<FloorExpression>(sr);  
                }
                if (expression != null)
                {
                    result.id = expression.id;
                    result.building = expression.building;
                    result.floorTitle = expression.floorTitle;
                    result.fllor = expression.froor;
                    var tasks = new List<Task<Company>>();
                    foreach (var item in expression.groups)
                    {
                        tasks.Add(CompanyHub.RetrieveGroupShoten(item));
                    }
                    var list = await Task.WhenAll<Company>(tasks);
                    result.groups = new List<Company>(list);
                }
            }
            catch
            {
                throw;
            }
            return result;
        }
        /// <summary>
        /// フロアの定義ファイル取得関数
        /// </summary>
        public async Task<string> GetFroor(Building building, int floor)
        {
            var result = "";
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
                            if (item.building == building)
                            {
                                result = item.paths[floor];
                            }
                        }
                    }
                }
            }
            catch
            {
                throw;
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
