using Bloom.Server.Utility;
using Bloom.Server.Utility.Format;
using System.Text.Json;
using Bloom.Shared;

namespace Bloom.Server.Handler
{
    public class Company
    {
        public async static Task<GroupExpression> RetrieveGroup(string id)
        {
            var result = new GroupExpression();
            using (var sr = new FileStream(DirectoryManeger.GetAbsotoblePath("/data/groups/" + id + ".json"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                result = await JsonSerializer.DeserializeAsync<GroupExpression>(sr);
            }
            return result;
        }

        public async static Task<string> BuryGroup(Group group)
        {
            var id = new Guid().ToString();

        }

        public async static Task ReplaceGroup(string id)
        {

        }
    }
}