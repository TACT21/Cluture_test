using System.Text.Json;
using System.Text.Json.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using System.Text;
using System.Runtime.InteropServices;
using Bloom.Shared.Setting;

namespace Bloom.Shared
{
    /// <summary>
    /// 出展団体の管理用クラス
    /// </summary>
    public class Group
    {
        public string id { get; set; }
        public string name { get; set; }
        public string enname { get; set; }
        public List<Media> posterUrl { get; set; } = new List<Media>();
        public string? comment { get; set; }
        public List<Media> videoUrl { get; set; } = new List<Media>();
        public List<Media> cmUrl { get; set; } = new List<Media>();
        public string? location { get; set; }
        public string? contentUrl { get; set; }
        /// <summary>
        /// Jsonの内容をこの変数に代入します。
        /// </summary>
        /// <param name="json">変換対象のJson</param>
        /// <param name="options">変換オプション。Nullの場合、Bloom.Shared.Setting.Media.options1に準じます。</param>
        public async Task ConvertFromJson(Stream json,JsonSerializerOptions options = null)
        {
            //シリアライズオプションの設定
            if(options== null)
            {
                options = Bloom.Shared.Setting.Media.options1;
            }
            var data = await JsonSerializer.DeserializeAsync<Json>(json,options);
            this.id = data.id;
            this.name = data.name;
            this.enname = data.enName;
            this.comment = data.comment;
            this.location = data.location;
            this.contentUrl = data.contentUrl;
            //リスト化されたオブジェクトの順応
            foreach (var item in data.videoUrl)
            {
                videoUrl.Add(item.Value);
            }
            foreach (var item in data.cmUrl)
            {
                cmUrl.Add(item.Value);
            }
            foreach (var item in data.posterUrl)
            {
                posterUrl.Add(item.Value);
            }
        }
        /// <summary>
        /// Jsonの内容をこの変数に代入します。
        /// </summary>
        /// <param name="json">変換対象のJson</param>
        /// <param name="options">変換オプション。Nullの場合、Bloom.Shared.Setting.Media.options1に準じます。</param>
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
        public bool isWide { set; get; } = true;
        /// <summary>
        /// Jsonの内容をこの変数に代入します。
        /// </summary>
        /// <param name="json">変換対象のJson</param>
        /// <param name="options">変換オプション。Nullの場合、Bloom.Shared.Setting.Media.options1に準じます。</param>
        public async Task ConvertFromJson(Stream json, JsonSerializerOptions options = null)
        {
            //シリアライズオプションの設定
            if (options == null)
            {
                options = Bloom.Shared.Setting.Media.options1;
            }
            var data = await JsonSerializer.DeserializeAsync<Json>(json, options);
            this.isfream = data.type == "ifream" ? true : false;
            this.type = data.type;
            this.Url = data.url;
            this.isWide = data.type == "wide" ? true : false;
        }
        /// <summary>
        /// Jsonの内容をこの変数に代入します。
        /// </summary>
        /// <param name="json">変換対象のJson</param>
        /// <param name="options">変換オプション。Nullの場合、Bloom.Shared.Setting.Media.options1に準じます。</param>
        public async Task ConvertFromJson(string json, JsonSerializerOptions options = null)
        {
            var memory = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var task = ConvertFromJson(memory, options);
            await task;
        }
        /// <summary>
        /// この変数の内容をJsonに変換します。
        /// </summary>
        /// <param name="options">変換オプション。Nullの場合、Bloom.Shared.Setting.Media.options1に準じます。</param>
        /// <returns>Json変換後の文字列</returns>
        public async Task<string> ConvertToJson(JsonSerializerOptions options = null)
        {
            if (options == null)
            {
                options = Bloom.Shared.Setting.Media.options1;
            }
            var raw = new Json();
            raw.url = this.Url;
            raw.type = this.type;
            raw.Iswide = this.isWide ? "wide" : "thin";
            return JsonSerializer.Serialize(this, typeof(Bloom.Shared.Media.Json), options);
        }

        class Json
        {
            [JsonPropertyName("type")] public string type { get; set; } = String.Empty;
            [JsonPropertyName("Url")] public string url { get; set; } = String.Empty;
            [JsonPropertyName("Wide")] public string Iswide { get; set; } = String.Empty;
        }
    }
    /// <summary>
    /// マップ用クラス。各階の総合データの送信
    /// </summary>
    public class Floor
    {
        public string id { get; set; } =string.Empty;
        public Building building { get; set; }
        public int fllor { get; set; }
        public string floorTitle { get; set; } = string.Empty;//フロアタイトル
        public Media floorMap { get; set; }//マップのデータ
        public List<Group> groups { get; set; } = new List<Group>();//団体一覧
        /// <summary>
        /// Jsonの内容をこの変数に代入します。
        /// </summary>
        /// <param name="json">変換対象のJson</param>
        /// <param name="options">変換オプション。Nullの場合、Bloom.Shared.Setting.Media.options1に準じます。</param>
        public async Task ConvertFromJson(Stream json, JsonSerializerOptions options = null)
        {
            //シリアライズオプションの設定
            if (options == null)
            {
                options = Bloom.Shared.Setting.Media.options1;
            }
            var data = await JsonSerializer.DeserializeAsync<Json>(json, options);
            this.floorTitle = data.floorTitle;
            this.floorMap = data.map;
            this.id  = data.id;
            //リスト化されたオブジェクトの順応
            foreach (var item in data.groups)
            {
                if(item.Key == item.Value.id)
                {
                    groups.Add(item.Value);
                }
            }
        }
        /// <summary>
        /// Jsonの内容をこの変数に代入します。
        /// </summary>
        /// <param name="json">変換対象のJson</param>
        /// <param name="options">変換オプション。Nullの場合、Bloom.Shared.Setting.Media.options1に準じます。</param>
        public async Task ConvertFromJson(string json, JsonSerializerOptions options = null)
        {
            var memory = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var task = ConvertFromJson(memory, options);
            await task;
        }
        /// <summary>
        /// このデータをJsonに変換します。
        /// </summary>
        /// <param name="options">変換オプション。Nullの場合、Bloom.Shared.Setting.Media.options1に準じます。</param>
        /// <returns>変換後のJson</returns>
        public async Task<string> ConvertToJson(JsonSerializerOptions options = null)
        {
            Json toJson = new Json();
            toJson.floorTitle = this.floorTitle;
            toJson.map = this.floorMap;
            foreach (var item in this.groups)
            {
                toJson.groups.Add(item.id, item);
            }
            return JsonSerializer.Serialize(toJson, typeof(Bloom.Shared.Group), options);
        }
        private class Json
        {
            [JsonPropertyName("id")] public string id { get; set; }
            /// <summary>
            /// 階の名前
            /// </summary>
            [JsonPropertyName("floorTitle")] public string floorTitle { get; set; }
            [JsonPropertyName("map")] public Media map { get; set; }
            [JsonPropertyName("groups")] public Dictionary<string,Group> groups { get; set; }
        }
    }
    /// <summary>
    /// 棟識別用列挙型
    /// </summary>
    public enum Building
    {
        Gernal = 0,
        Central = 1,
        High = 2,
        Junior = 3,
        Hall = 4,
    }
    /// <summary>
    /// イベント管理クラス。Groupでもいいかもしれない。
    /// </summary>
    public class Events
    {
        public string id { get; set; } = string.Empty;
        public string title { get; set; } = string.Empty;
        public string url { get; set; } = string.Empty;
        public string sumbnaill { get; set; } = string.Empty;
        public async Task ConvertFromJson(string json)
        {
            var data = JsonSerializer.Deserialize<Json>(json);
            this.title = data.title;
            this.url = data.url;
            this.sumbnaill= data.sumbnaill;
        }

        public async Task<string> ConvertToJson(JsonSerializerOptions options = null)
        {
            if(options == null)
            {
                options = Bloom.Shared.Setting.Media.options1;
            }
            return JsonSerializer.Serialize(this, typeof(Bloom.Shared.Events), options);
        }

        private class Json
        {
            [JsonPropertyName("id")] public string id { get; set; }
            [JsonPropertyName("title")] public string title { get; set; }
            [JsonPropertyName("url")] public string url { get; set; }
            [JsonPropertyName("sumbnaill")] public string sumbnaill { get; set; }
        }
    }
}
