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
    public class FloorHandler
    {
        public static async Task BullyFloor(Floor floor)
        {
            var gex = new FloorExpression();
            gex.ConvertFromFloor(floor);
            await BullyFloor(gex);
        }
        public static async Task BullyFloor(FloorExpression gex)
        {
            var file = new OptimismFileHelper<FloorExpression>();
            try
            {
                await file.OpenAsync(DirectoryManeger.GetAbsotoblePath("/data/floors/" + gex.id + ".json"));
                await file.ChengeAsync(file.values.Count, gex);
            }
            catch
            {
                throw;
            }
        }
        public static async Task ReplaceFloor(Floor floor, int index, bool overWrite = false)
        {
            var gex = new FloorExpression();
            gex.ConvertFromFloor(floor);
            await ReplaceFloor(gex, index, overWrite);
        }
        public async static Task ReplaceFloor(FloorExpression gex, int index, bool overWrite = false)
        {
            if (File.Exists(DirectoryManeger.GetAbsotoblePath("/data/floors/" + gex.id + ".json")))
            {
                throw new ArgumentException();
            }
            var file = new OptimismFileHelper<FloorExpression>();
            try
            {
                await file.OpenAsync(DirectoryManeger.GetAbsotoblePath("/data/floors/" + gex.id + ".json"));
                await ReplaceFloor(gex, index, overWrite);
            }
            catch
            {
                throw;
            }
        }
        public static async Task<List<DataExpression<FloorExpression>>> RetriveFloorExDev(string id)
        {
            List<DataExpression<FloorExpression>> result = new List<DataExpression<FloorExpression>>();
            result.Add(new DataExpression<FloorExpression>()
            {
                data = new FloorExpression() { id = "NULL" }
            });
            var file = new OptimismFileHelper<FloorExpression>();
            try
            {
                await file.OpenAsync(DirectoryManeger.GetAbsotoblePath("/data/floors/" + id + ".json"));
            }
            catch
            {
                throw;
            }
            result = file.values;
            return result;
        }
        public static async Task<List<DataExpression<Floor>>> RetriveFloorDev(string id, bool convert = false)
        {
            List<DataExpression<Floor>> result = new List<DataExpression<Floor>>();
            result.Add(new DataExpression<Floor>()
            {
                data = new Floor() { floorTitle = "NULL" }
            });
            var file = new OptimismFileHelper<FloorExpression>();
            try
            {
                var a = await RetriveFloorExDev(id);
                foreach (var item in a)
                {
                    var floor = new Floor();
                    if (item.data != null)
                    {
                        floor.fllor = item.data.froor;
                        floor.floorTitle = item.data.floorTitle;
                        var task = new List<Task<Company>>();
                        foreach (var group in item.data.companys)
                        {
                            task.Add(CompanyHandler.RetrieveGroup(group, convert));
                        }
                        var re = await Task.WhenAll(task);
                        floor.companys = re.ToArray();
                    }
                }
            }
            catch
            {
                throw;
            }
            return result;
        }
        public static async Task<Floor> RetriveFloor(string id, bool convert = false)
        {
            Floor result = new Floor() { id = "hoge" };
            var Floor = new List<DataExpression<Floor>>();
            try
            {
                Floor = await RetriveFloorDev(id, convert);
            }
            catch
            {
                throw;
            }
            foreach (var item in Floor)
            {
                if (item.recent && item.data != null)
                {
                    result = item.data;
                    break;
                }
            }
            if (result.id == "hoge")
            {
                throw new EntryPointNotFoundException();
            }
            return result;
        }
        public static async Task<List<Floor>> RetriveFloorList(bool convert = false)
        {
            List<Floor> result = new List<Floor>();
            var files = Directory.GetFiles(DirectoryManeger.GetAbsotoblePath("/data/floors/"));
            foreach (var item in files)
            {
                if (Path.GetExtension(item) == ".json")
                {
                    var id = DirectoryManeger.ConvertPath2Id(item);
                    try
                    {
                        result.Add(await RetriveFloor(id, convert));
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            return result;
        }
        public static async Task<List<BuildingExpression>> RetriveBuildingExpression()
        {
            List<BuildingExpression> result = new List<BuildingExpression>(0);
            var file = new OptimismFileHelper<List<BuildingExpression>>();
            try
            {
                await file.OpenAsync(DirectoryManeger.GetAbsotoblePath("/floor_indexer.json\r\n"));
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
                    break;
                }
            }
            if(result.Count == 0)
            {
                throw new EntryPointNotFoundException();
            }
            return result;
        }
        public static async Task<List<BuildingExpression>> ReplaceFloor(List<BuildingExpression> buildings)
        {
            List<BuildingExpression> result = new List<BuildingExpression>(0);
            DataExpression<List<BuildingExpression>> data = new DataExpression<List<BuildingExpression>>() { data = buildings};
            var file = new OptimismFileHelper<List<BuildingExpression>>();
            try
            {
                await file.OpenAsync(DirectoryManeger.GetAbsotoblePath("/floor_indexer.json\r\n"));
                await file.ChengeAsync(file.values.Count + 1, data,true);
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
                    break;
                }
            }
            if (result.Count == 0)
            {
                throw new EntryPointNotFoundException();
            }
            return result;
        }
    }
}
