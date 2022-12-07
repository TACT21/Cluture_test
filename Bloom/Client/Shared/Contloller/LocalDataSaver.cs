using System.Text.Json;
using System.Text;
namespace Bloom.Client.Shared.Contloller
{
    public static class LocalDataSaver
    {
        public static bool UseLocalSaver()
        {
            return false;
        }

        public static async Task<T> GetLocalData<T>(string key)
        {
            
        }
        public static async Task SetLocalData<T>(string key,T body)
        {
            var data = new Data<T>();
            data.DataObject = body;
            data.DataType = typeof(T);
            string json = "";
            using (var ms = new MemoryStream())
            {
                await JsonSerializer.SerializeAsync<Data<T>>(ms, data);
                var datas = new byte[ms.Length];
                await ms.ReadAsync(datas, 0, datas.Length);
                json = Encoding.UTF8.GetString(datas);
            }
        }
        class Data<T>
        {
            public Type? DataType { get; set; }
            public T? DataObject { get; set; }
        }
    }
}
