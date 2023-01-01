using Microsoft.AspNetCore.SignalR;
using System.IO;
using Bloom.Server;
using Bloom.Server.Controllers;
using Bloom.Server.Utility;
using Bloom.Server.Utility.Format;
using System.Text;
using Bloom.Shared;
using System.Text.Json;
using SixLabors.ImageSharp;
using System.IO;

namespace Bloom.Server.Controllers
{
    public class ScheduleHub : Hub
    {
        public async Task<List<Event>> ClaimSchedules()
        {
            var result = new List<Event>();
#if DEBUG
            result.Add(new Event() { id = "0",title="Dev",url="/Live/Demo",type=EventType.Live});
#endif
#if !DEBUG
            try
            {
                result = await RetrieveSchedules();
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
                var path = "/tmp/" + Guid.NewGuid().ToString() + ".txt";
                //Close時に削除する一時ファイルを作成
                using (FileStream fs = File.Create(DirectoryManeger.GetAbsotoblePath(path)))
                {
                    await JsonSerializer.SerializeAsync(fs, result);
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        await Clients.Caller.SendAsync("ReceiveSchedules", sr.ReadToEnd());
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

        private async Task<List<Event>> RetrieveSchedules()
        {
            var result = new List<Event>();
            try
            {
                var indexer = await Filer.GetFileText(DirectoryManeger.GetAbsotoblePath("/data/events.json"));
                var path = "/tmp/" + Guid.NewGuid().ToString() + ".json";
                //Close時に削除する一時ファイルを作成
                using (FileStream sr = File.Create(
                    DirectoryManeger.GetAbsotoblePath(path),
                    Encoding.UTF8.GetBytes(indexer).Length,
                    FileOptions.DeleteOnClose))
                {
                    await sr.WriteAsync(Encoding.UTF8.GetBytes(indexer));
                    result = await JsonSerializer.DeserializeAsync<List<Event>>(sr);
                }
            }
            catch
            {
                throw;
            }
            return result;
        }
    }
}
