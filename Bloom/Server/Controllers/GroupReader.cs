using Bloom.Shared;
using System.Text.Json;

namespace Bloom.Server.Controllers
{
    public class GroupReader
    {
        public async Task<Group> Read(string path)
        {
            var result = new Group();
            using (StreamReader sr = new StreamReader(path))
            {
                var content = await sr.ReadToEndAsync();
                GroupJson json = JsonSerializer.Deserialize<GroupJson>(content);
                result = json.Convert();
            }
            return result;
        }
    }
}
