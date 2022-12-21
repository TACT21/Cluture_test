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

namespace Bloom.Server.Hubs
{
    public class Map : Hub
    {
        public async Task<Floor> FloorID(Building building,int floor)
        {
            var result = new Floor();
            #if DEBUG
                result = await SendFloorIDDev(building,floor);
            #endif
            #if !DEBUG
                    result = await SendFloorID();
            #endif
            return result;
        }
        public async Task<Floor> SendFloorIDDev(Building building, int floor)
        {
            var result = new Floor();
            result.id = "0";
            return result;
        }
    }
}
