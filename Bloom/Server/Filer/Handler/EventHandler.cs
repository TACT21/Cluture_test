using Bloom.Server.Utility;
using Bloom.Server.Filer.Utility;
using Bloom.Server.Utility.Format;
using Bloom.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bloom.Server.Filer.Handler
{
    internal static class EventHandler
    {
        public async static Task<List<Event>> RetrieveEventList()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var result = new List<Event>();
            DirectoryInfo di = new DirectoryInfo(DirectoryManeger.GetAbsotoblePath("/data/events"));
            string errorContent = "";
            int errors = 0;
            var tasks = new List<Task<Event>>();
            // ディレクトリ直下のすべてのファイル一覧を取得する
            FileInfo[] fiAlls = di.GetFiles();
            foreach (var f in fiAlls)
            {
                var task =( async () => { 
                    var file = new Event();
                    try
                    {
                        file = await GainEvent(f.FullName);
                    }
                    catch(Exception e)
                    {
                        errorContent += ("===== Error happen from Event Handler @"+DateTime.Now.ToString() + " =====" + JsonSerializer.Serialize(e, options));
                        errors++;
                    }
                    return file;
                });
                tasks.Add(task.Invoke());
            }
            var events = await Task.WhenAll(tasks);
            if(errors > 0)
            {
                var path = DirectoryManeger.GetAbsotoblePath("/logs/" + DateTime.Now.ToString() + "_EventHandler.txt");
                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.WriteLine(errorContent);
                }
                if(tasks.Count == 1)
                {
                    Console.WriteLine("Exception is happened \nLog exist @ " + path);
                }
                else
                {
                    Console.WriteLine(errors.ToString() + " exceptions is happened \nLog exist @ " + path);
                }
            }
            foreach (var item in events)
            {
                result.Add(item);
            }
            return result;
        }
        public async static Task<Event> RetrieveEvent(string id)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            Event result = new Event();
            var errorContent = "";
            try
            {
                result = await GainEvent(DirectoryManeger.GetAbsotoblePath("/data/events/" + id + ".json"));
            }catch(EntryPointNotFoundException e)
            {
                errorContent += ("===== Error happen from Event Handler @" + DateTime.Now.ToString() + " ===== \n{\n\t\"Error\":\"Not found current data\"\n\t\"id\":\""+id.ToString()+"\"}");
            }catch(Exception e)
            {
                errorContent += ("===== Error happen from Event Handler @" + DateTime.Now.ToString() + " =====" + JsonSerializer.Serialize(e, options));
            }
            if(errorContent != "")
            {
                var path = DirectoryManeger.GetAbsotoblePath("/logs/" + DateTime.Now.ToString() + "_EventHandler.txt");
                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.WriteLine(errorContent);
                    Console.WriteLine("Exception is happened \nLog exist @ " + path);
                }
            }
            return result;
        }
        private async static Task<Event> GainEvent(string path)
        {
            Event result = new Event() { id = "NULL"};
            var file = new OptimismFileHelper<Event>();
            try
            {
                await file.OpenAsync(path);
            }
            catch
            {
                throw;
            }
            foreach (var item in file.values)
            {
                if (item.recent && item.data != null)
                {
                    result = item.data;
                }
            }
            if(result.id == "NULL")
            {
                throw new EntryPointNotFoundException();
            }
            return result;
        }
        public async static Task BuryEvent(Event group,bool overWrite = false)
        {
            group.id = new Guid().ToString("N");
            var options = new JsonSerializerOptions { WriteIndented = true };
            var errorContent = "";
            var file = new OptimismFileHelper<Event>();
            try
            {
                var path = DirectoryManeger.GetAbsotoblePath("/data/events/" + group.id + ".json");
                await file.OpenAsync(path);
                await file.ChengeAsync(file.values.Count, group, path, overWrite);
            }
            catch (InvalidOperationException e)
            {
                errorContent += ("===== Error happen from Event Handler @" + DateTime.Now.ToString() + " ===== \n{\n\t\"Error\":\"Not found current data\"\n\t\"id\":\"" + group.id.ToString() + "\"}");
                throw;
            }
            catch (Exception e)
            {
                errorContent += ("===== Error happen from Event Handler @" + DateTime.Now.ToString() + " =====" + JsonSerializer.Serialize(e, options));
            }
            if (errorContent != "")
            {
                var path = DirectoryManeger.GetAbsotoblePath("/logs/" + DateTime.Now.ToString() + "_EventHandler.txt");
                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.WriteLine(errorContent);
                    Console.WriteLine("Exception is happened \nLog exist @ " + path);
                }
            }
        }
        public async static Task ReplaceEvent(Event group, bool overWrite = false)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var errorContent = "";
            var file = new OptimismFileHelper<Event>();
            try
            {
                var path = DirectoryManeger.GetAbsotoblePath("/data/events/" + group.id + ".json");
                await file.OpenAsync(path);
                var current = new DataExpression<Event>();
                var currentIndex = 0;
                var index = 0;
                var exist = false;
                for (int i = 0; i < file.values.Count; i++)
                {
                    var item = file.values[i];
                    if(item.data != null && item.data.id == group.id)
                    {
                        exist = true;
                        index = i;
                    }else if(item.data != null && item.recent)
                    {
                        current = item;
                        current.recent = false;
                        currentIndex = i;
                    }
                }
                if (exist)
                {
                    await file.ChengeAsync(currentIndex, current, path, overWrite);
                    await file.ChengeAsync(index, new DataExpression<Event>() { data = group,recent = true}, path, overWrite);
                }
                else
                {
                    throw new EntryPointNotFoundException();
                }
                await file.ChengeAsync(file.values.Count, group, path, overWrite);
            }
            catch (InvalidOperationException e)
            {
                errorContent += ("===== Error happen from Event Handler @" + DateTime.Now.ToString() + " ===== \n{\n\t\"Error\":\"Not found current data\"\n\t\"id\":\"" + group.id.ToString() + "\"}");
                throw;
            }
            catch (Exception e)
            {
                errorContent += ("===== Error happen from Event Handler @" + DateTime.Now.ToString() + " =====" + JsonSerializer.Serialize(e, options));
            }
            if (errorContent != "")
            {
                var path = DirectoryManeger.GetAbsotoblePath("/logs/" + DateTime.Now.ToString() + "_EventHandler.txt");
                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.WriteLine(errorContent);
                    Console.WriteLine("Exception is happened \nLog exist @ " + path);
                }
            }
        }
    }
}
