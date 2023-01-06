using Bloom.Server.Filer.Handler;
using Bloom.Server.Filer.Utility;
using Bloom.Server.Utility;
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
    public static class GreadHandler
    {
        public static async Task BullyGread(Floor floor)
        {
            var gex = new GreadExpression();
            gex.ConvertFromFloor(floor);
            await BullyGread(gex);
        }
        public static async Task BullyGread(GreadExpression gex)
        {
            var file = new OptimismFileHelper<GreadExpression>();
            try
            {
                await file.OpenAsync(DirectoryManeger.GetAbsotoblePath("/data/greads/" + gex.id + ".json"));
                await file.ChengeAsync(file.values.Count,gex);
            }
            catch
            {
                throw;
            }
        }
        public static async Task ReplaceGread(Floor floor, int index, bool overWrite = false)
        {
            var gex = new GreadExpression();
            gex.ConvertFromFloor(floor);
            await ReplaceGread(gex, index, overWrite);
        }
        public async static Task ReplaceGread(GreadExpression gex, int index, bool overWrite = false)
        {
            if (File.Exists(DirectoryManeger.GetAbsotoblePath("/data/greads/" + gex.id + ".json")))
            {
                throw new ArgumentException();
            }
            var file = new OptimismFileHelper<GreadExpression>();
            try
            {
                await file.OpenAsync(DirectoryManeger.GetAbsotoblePath("/data/greads/" + gex.id + ".json"));
                await ReplaceGread(gex, index, overWrite);
            }
            catch
            {
                throw;
            }
        }
        public static async Task<List<DataExpression<GreadExpression>>> RetriveGreadExDev(string id)
        {
            List<DataExpression<GreadExpression>> result = new List<DataExpression<GreadExpression>>();
            result.Add(new DataExpression<GreadExpression>()
            {
                data = new GreadExpression() { id = "NULL" }
            });
            var file = new OptimismFileHelper<GreadExpression>();
            try
            {
                await file.OpenAsync(DirectoryManeger.GetAbsotoblePath("/data/greads/" + id+ ".json"));
            }
            catch
            {
                throw;
            }
            result = file.values;
            return result;
        }
        public static async Task<List<DataExpression<Floor>>> RetriveGreadDev(string id,bool convert = false)
        {
            List<DataExpression<Floor>> result = new List<DataExpression<Floor>>();
            result.Add(new DataExpression<Floor>()
            {
                data = new Floor() { floorTitle = "NULL" }
            });
            var file = new OptimismFileHelper<GreadExpression>();
            try
            {
                var a = await RetriveGreadExDev(id);
                foreach (var item in a)
                {
                    var floor = new Floor();
                    if (item.data != null)
                    {
                        floor.fllor = item.data.gread;
                        floor.floorTitle = item.data.name;
                        var task = new List<Task<Company>>();
                        foreach (var group in item.data.groups)
                        {
                            task.Add(CompanyHandler.RetrieveGroup(group, convert));
                        }
                        floor.groups.AddRange(await Task.WhenAll(task));
                    }
                }
            }
            catch
            {
                throw;
            }
            return result;
        }
        public static async Task<Floor> RetriveGread(string id, bool convert = false)
        {
            Floor result = new Floor() { id = "hoge"};
            var gread = new List<DataExpression<Floor>>();
            try
            {
               gread = await RetriveGreadDev(id, convert);
            }
            catch
            {
                throw;
            }
            foreach (var item in gread)
            {
                if (item.recent && item.data != null)
                {
                    result = item.data;
                    break;
                }
            }
            if(result.id == "hoge")
            {
                throw new EntryPointNotFoundException();
            }
            return result;
        }
        public static async Task<List<Floor>> RetriveGreadList(bool convert = false)
        {
            List<Floor> result = new List<Floor>();
            var files = Directory.GetFiles(DirectoryManeger.GetAbsotoblePath("/data/greads/"));
            foreach (var item in files)
            {
                if(Path.GetExtension(item) == ".json")
                {
                    var id = DirectoryManeger.ConvertPath2Id(item);
                    try
                    {
                        result.Add(await RetriveGread(id, convert));
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            return result;
        }
    }
}
