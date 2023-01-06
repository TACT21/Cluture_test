using Bloom.Server.Filer.Utility;
using Bloom.Server.Utility;
using Bloom.Server.Utility.Format;
using System.Text.Json;
using Bloom.Shared;
using System.Text;

namespace Bloom.Server.Filer.Handler
{
    public static class CompanyHandler
    {
        public async static Task<CompanyExpression> RetrieveGroupEX(string id)
        {
            var result = new CompanyExpression() { id = "hoge"};
            var file = new OptimismFileHelper<CompanyExpression>();
            try
            {
                await file.OpenAsync(DirectoryManeger.GetAbsotoblePath("/data/groups/" + id + ".json"));
                foreach (var item in file.values)
                {
                    if (item.recent)
                    {
                        if (item.data != null)
                        {
                            result = item.data;
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
            if(result.id == "hoge")
            {
                throw new EntryPointNotFoundException();
            }
            return result;
        }
        public async static Task<Company> RetrieveGroup(string id,bool convert = false)
        {
            var gex = new CompanyExpression();
            try
            {
                gex = await RetrieveGroupEX(id);
            }
            catch
            {
                throw;
            }
            return await gex.ConvertToCompany(convert);
        }
        public async static Task<string> BuryGroup(Company group)
        {
            var id = new Guid().ToString("N");
            await WriteGroup(id, group);
            return id;
        }
        public async static Task ReplaceGroup(string id, Company group)
        {
            if (File.Exists(DirectoryManeger.GetAbsotoblePath("/data/groups/" + id + ".json")))
            {
                throw new ArgumentException();
            }
            await WriteGroup(id, group);    
        }
        private async static Task WriteGroup(string id, Company company)
        {
            var ver = 0;
            if (File.Exists(DirectoryManeger.GetAbsotoblePath("/data/groups/" + id + ".json")))
            {
                File.Create(DirectoryManeger.GetAbsotoblePath("/data/groups/" + id + ".json"));
                var pre = await RetrieveGroupEX(id);
                ver = Int32.Parse(pre.version);
            }
            var young = new CompanyExpression();
            young.ConvertFromCompany(company);
            young.version = (ver + 1).ToString();
            using (var sr = new StreamWriter(DirectoryManeger.GetAbsotoblePath("/data/groups/" + id + ".json"), false, Encoding.UTF8))
            {
                await sr.WriteLineAsync(JsonSerializer.Serialize(young));
            }
        }
    }
}