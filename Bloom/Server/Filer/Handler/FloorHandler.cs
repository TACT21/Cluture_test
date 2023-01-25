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
        public static async Task<string> BullyFloor(Floor floor)
        {
            var gex = new FloorExpression();
            gex.ConvertFromFloor(floor);
            return await BullyFloor(gex);
        }
        public static async Task<string> BullyFloor(FloorExpression gex)
        {
            var id = Guid.NewGuid().ToString("N");
            var path = DirectoryManeger.GetAbsotoblePath("/data/floors" + id + ".json");
            File.Create(path).Dispose();
            try
            {
                var tasks = new List<Task>();
                var floor = new OptimismFileHelper<FloorExpression>();
                await floor.OpenAsync(path);
                tasks.Add(floor.ChengeAsync(floor.values.Count, gex, true));
                var indexer = new OptimismFileHelper<BuildingExpression>();
                await indexer.OpenAsync(DirectoryManeger.GetAbsotoblePath("/data/floor_indexer.json"));
                bool exist = false;
                var newData = new DataExpression<BuildingExpression>();
                for (int i = 0; i < indexer.values.Count; i++)
                {
                    if (indexer.values[i] != null && indexer.values[i].data.building == gex.building)
                    {
                        newData = indexer.values[i];
                        exist = true;
                        newData.data.paths.Add(gex.froor, path);
                        tasks.Add(indexer.ChengeAsync(i, newData, true));
                        break;
                    }
                }
                if(!exist)
                {

                }
            }
            catch
            {
                throw;
            }
        }
        public static async Task ReplaceFloor(Floor floor,  bool overWrite = false)
        {
            var gex = new FloorExpression();
            gex.ConvertFromFloor(floor);
            await ReplaceFloor(gex,  overWrite);
        }
        public async static Task ReplaceFloor(FloorExpression gex, bool overWrite = false)
        {
            bool exist = false;
            var indexer = new OptimismFileHelper<BuildingExpression>();
            try
            {
                await indexer.OpenAsync(DirectoryManeger.GetAbsotoblePath("/data/floor_indexer.json"));
            }
            catch
            {
                throw;
            }
            for (int i = 0; i < indexer.values.Count; i++)
            {
                if (indexer.values[i].data != null && indexer.values[i].data.building == gex.building)
                {
                    try
                    {
                        var floor = new OptimismFileHelper<FloorExpression>();
                        await floor.OpenAsync(indexer.values[i].data.paths[gex.froor]);
                        await floor.ChengeAsync(floor.values.Count, gex, true);
                    }
                    catch
                    {
                        throw;
                    }
                    exist = true;
                }
            }
            if (!exist)
            {
                throw new EntryPointNotFoundException();
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
                await file.OpenAsync(DirectoryManeger.GetAbsotoblePath("/data/floor_indexer.json\r\n"));
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
        public static async Task RenewFloor(Floor floor)
        {
            var gex = new FloorExpression();
            gex.ConvertFromFloor(floor);
            await RenewFloor(gex);
        }
        public static async Task RenewFloor(FloorExpression gex)
        {
            bool exist = false;
            var indexer = new OptimismFileHelper<BuildingExpression>();
            try
            {
                await indexer.OpenAsync(DirectoryManeger.GetAbsotoblePath("/data/floor_indexer.json\r\n"));
            }
            catch
            {
                throw;
            }
            for (int i = 0; i < indexer.values.Count; i++)
            {
                if (indexer.values[i].data != null && indexer.values[i].data.building == gex.building)
                {
                    try
                    {
                        var floor = new OptimismFileHelper<FloorExpression>();
                        var a = 0;
                        await floor.OpenAsync(indexer.values[i].data.paths[gex.froor]);
                        for (int i = 0; i < floor.values.Count; i++)
                        {
                            if (floor.values[i] != null && floor.values[i].data.Equals(gex))
                            {
                                floor.values[i].recent = true;
                                exist = true;
                            } else if (floor.values[i] != null && floor.values[i].recent)
                            {
                                a = i;//あとでrecentを消す用
                            }
                        }
                        if(exist)
                        {
                            floor.values[a].recent = false;
                        }
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            if (!exist)
            {
                throw new EntryPointNotFoundException();
            }
        }
    }
}
