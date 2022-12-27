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
        /// <summary>
        /// ローカルストレージのデータ取得関数
        /// </summary>
        /// <typeparam name="T">格納データのタイプ</typeparam>
        /// <param name="key">格納データのキー</param>
        /// <param name="options">シリアライズオプション</param>
        /// <param name="cancellationToken">キャンセルトークン</param>
        public static async Task<T> GetLocalData<T>(string key,JsonSerializerOptions options = null ,CancellationToken cancellationToken = default)
        {
            var data = "";
            var result = new Data<T>(); 
            using (var reader = new MemoryStream())
            {
                reader.Write(Encoding.UTF8.GetBytes(data));
                result = await JsonSerializer.DeserializeAsync<Data<T>>(reader, options ,cancellationToken);
            }
            if (result == null || result.DataObject == null)
            {
                return default(T);
            }
            else
            {
                try
                {
                    return (T)result.DataObject;
                }
                catch
                {
                    throw;
                }
            }
        }
        /// <summary>
        /// ローカルストレージのデータ型取得関数
        /// </summary>
        /// <param name="key">データキー</param>
        /// <param name="options">オプション</param>
        /// <param name="cancellationToken">キャンセルトークン</param>
        /// <returns>データタイプ。NULLの場合、データは存在しません。</returns>
        public static async Task<Type?> GetLocalDataType(string key, JsonSerializerOptions? options = default, CancellationToken cancellationToken = default)
        {
            var data = "";
            var result = new Data<byte[]>();
            using (var reader = new MemoryStream())
            {
                reader.Write(Encoding.UTF8.GetBytes(data));
                result = await JsonSerializer.DeserializeAsync<Data<byte[]>>(reader, options, cancellationToken);
            }
            if (result == null || result.DataType == null)
            {
                return null;
            }
            else
            {
                return result.DataType;
            }
        }
        /// <summary>
        /// ローカルストレージへのデータ格納用関数
        /// </summary>
        /// <typeparam name="T">格納データのタイプ</typeparam>
        /// <param name="key">格納データのキー</param>
        /// <param name="body">格納データ本体</param>
        /// <param name="options">シリアライズオプション</param>
        /// <param name="cancellationToken">キャンセルトークン</param>
        public static async Task SetLocalData<T>(string key,T body, JsonSerializerOptions? options = default, CancellationToken cancellationToken = default)
        {
            var data = new Data<T>();
            data.DataObject = body;
            data.DataType = typeof(T);
            string json = "";
            using (var ms = new MemoryStream())
            {
                await JsonSerializer.SerializeAsync<Data<T>>(ms, data,options,cancellationToken);
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
