using System.Text.Json;
using System.Text.Json.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using System.Text;
using System.Runtime.InteropServices;

namespace Bloom.Shared
{
    internal static class Default
    {
        internal static JsonSerializerOptions options1 = new JsonSerializerOptions();
    }
    /// <summary>
    /// 出展団体の管理用クラス
    /// </summary>
    public class Group
    {
        public string id { get; set; }
        public string name { get; set; }
        public string enname { get; set; }
        public List<Media>? posterUrl { get; set; }
        public string? comment { get; set; }
        public List<Media>? videoUrl { get; set; }
        public List<Media>? cmUrl { get; set; }
        public string? location { get; set; }
        public string? contentUrl { get; set; }
        /// <summary>
        /// Jsonの内容をこの変数に代入します。
        /// </summary>
        /// <param name="json">変換対象のJson</param>
        /// <param name="options">変換オプション。Nullの場合、Bloom.Shared.Default.options1に準じます。</param>
        public async Task ConvertFromJson(Stream json,JsonSerializerOptions options = null)
        {
            //シリアライズオプションの設定
            if(options== null)
            {
                options = Default.options1;
            }
            var deta = await JsonSerializer.DeserializeAsync<Json>(json,options);
            this.id = deta.id;
            this.name = deta.name;
            this.enname = deta.enName;
            this.comment = deta.comment;
            this.location = deta.location;
            this.contentUrl = deta.contentUrl;
            //リスト化されたオブジェクトの順応
            foreach (var item in deta.videoUrl)
            {
                videoUrl.Add(item.Value);
            }
            foreach (var item in deta.cmUrl)
            {
                cmUrl.Add(item.Value);
            }
            foreach (var item in deta.posterUrl)
            {
                posterUrl.Add(item.Value);
            }
        }
        /// <summary>
        /// Jsonの内容をこの変数に代入します。
        /// </summary>
        /// <param name="json">変換対象のJson</param>
        /// <param name="options">変換オプション。Nullの場合、Bloom.Shared.Default.options1に準じます。</param>
        public async Task ConvertFromJson(string json, JsonSerializerOptions options = null)
        {
            var memory = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var task = ConvertFromJson(memory,options);
            await task;
        }
        public async Task<string> ConvertToJson(JsonSerializerOptions options = null)
        {
            return JsonSerializer.Serialize(this, typeof(Bloom.Shared.Group), options);
        }
        public class Json
        {
            [JsonPropertyName("Id")] public string id { get; set; }
            [JsonPropertyName("Name")] public string name { get; set; }
            [JsonPropertyName("EnName")] public string enName { get; set; }
            [JsonPropertyName("PosterURL")] public Dictionary<string,Media>? posterUrl { get; set; }
            [JsonPropertyName("Comment")] public string? comment { get; set; }
            [JsonPropertyName("VideoLink")] public Dictionary<string, Media>? videoUrl { get; set; }
            [JsonPropertyName("CMLink")] public Dictionary<string, Media>? cmUrl { get; set; }
            [JsonPropertyName("Location")] public string? location { get; set; }
            [JsonPropertyName("ContentLink")] public string? contentUrl { get; set; }
        }
    }
    /// <summary>
    /// ポスターやCM等電子データの保持クラス。
    /// </summary>
    public class Media
    {        
        public bool isfream { get; set; }//互換性維持のために残置。Typeを使用すること
        public string type { get; set; }
        public string Url { set; get; }
        /// <summary>
        /// Jsonの内容をこの変数に代入します。
        /// </summary>
        /// <param name="json">変換対象のJson</param>
        /// <param name="options">変換オプション。Nullの場合、Bloom.Shared.Default.options1に準じます。</param>
        public async Task ConvertFromJson(Stream json, JsonSerializerOptions options = null)
        {
            //シリアライズオプションの設定
            if (options == null)
            {
                options = Default.options1;
            }
            var deta = await JsonSerializer.DeserializeAsync<Json>(json, options);
            this.isfream = deta.type == "ifream" ? true : false;
            this.type = deta.type;
            this.Url = deta.url;
        }
        /// <summary>
        /// Jsonの内容をこの変数に代入します。
        /// </summary>
        /// <param name="json">変換対象のJson</param>
        /// <param name="options">変換オプション。Nullの場合、Bloom.Shared.Default.options1に準じます。</param>
        public async Task ConvertFromJson(string json, JsonSerializerOptions options = null)
        {
            var memory = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var task = ConvertFromJson(memory, options);
            await task;
        }
        /// <summary>
        /// この変数の内容をJsonに変換します。
        /// </summary>
        /// <param name="options">変換オプション。Nullの場合、Bloom.Shared.Default.options1に準じます。</param>
        /// <returns>Json変換後の文字列</returns>
        public async Task<string> ConvertToJson(JsonSerializerOptions options = null)
        {
            if (options == null)
            {
                options = Bloom.Shared.Default.options1;
            }
            return JsonSerializer.Serialize(this, typeof(Bloom.Shared.Media), options);
        }

        class Json
        {
            [JsonPropertyName("type")] public string type { get; set; }
            [JsonPropertyName("Url")] public string url { get; set; }
        }
    }

    public class GroupMap
    {
        public string id { get; set; }
        public string name { get; set; }
        public string? comment { get; set; }
        public string? posterUrl { get; set; }
        public bool isWide { get; set; }= false;
        public string? location { get; set; }
    }

    public class Floor
    {
        public string floorTitle { get; set; } = string.Empty;
        public List<GroupMap> groups = new List<GroupMap>();
        public async Task ConvertFromJson(string json)
        {
            var deta = JsonSerializer.Deserialize<Json>(json);
            this.floorTitle = deta.floorTitle;
            this.groups = JsonSerializer.Deserialize<List<GroupMap>>(deta.groups);
        }

        public async Task<string> ConvertToJson(JsonSerializerOptions options = null)
        {
            if (options == null)
            {
                options = Bloom.Shared.Default.options1;
            }
            return JsonSerializer.Serialize(this, typeof(Bloom.Shared.Floor), options);
        }

        private class Json
        {
            [JsonPropertyName("floorTitle")] public string floorTitle { get; set; }
            [JsonPropertyName("groups")] public string groups { get; set; }
        }
    }

    public class Events
    {
        public string title { get; set; } = string.Empty;
        public string url { get; set; } = string.Empty;
        public string sumbnaill { get; set; } = string.Empty;
        public async Task ConvertFromJson(string json)
        {
            var deta = JsonSerializer.Deserialize<Json>(json);
            this.title = deta.title;
            this.url = deta.url;
            this.sumbnaill= deta.sumbnaill;
        }

        public async Task<string> ConvertToJson(JsonSerializerOptions options = null)
        {
            if(options == null)
            {
                options = Bloom.Shared.Default.options1;
            }
            return JsonSerializer.Serialize(this, typeof(Bloom.Shared.Events), options);
        }

        private class Json
        {
            [JsonPropertyName("title")] public string title { get; set; }
            [JsonPropertyName("url")] public string url { get; set; }
            [JsonPropertyName("sumbnaill")] public string sumbnaill { get; set; }
        }
    }

}
