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
    public class Company
    {
        public string id { get; set; }
        public string ver { get; set; }
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
            return JsonSerializer.Serialize(this, typeof(Bloom.Shared.Company), options);
        }
        public class Json
        {
            [JsonPropertyName("Id")] public string id { get; set; }
            [JsonPropertyName("Version")] public string var { get; set; }
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
        public string id { get; set; }
        public bool isfream { get; set; }//互換性維持のために残置。Typeを使用すること
        public string type { get; set; }
        public string Url { set; get; }
        public bool isWide { set; get; } = true;
        public int Width { set; get; } = 0;
        public int Height { set; get; } = 0;
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
            this.isfream = data.type == "ifream" ? true : false;
            this.type = data.type;
            this.Url = data.url;
            this.id = data.id;
            this.Width = Int32.Parse(data.width);
            this.Height = Int32.Parse(data.height);
            this.isWide = (Int32.Parse(data.width) > Int32.Parse(data.height)) ? true : false;
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
            raw.width = this.Width.ToString();
            raw.height = this.Height.ToString();
            raw.id = this.id;
            return JsonSerializer.Serialize(this, typeof(Bloom.Shared.Media.Json), options);
        }

        class Json
        {
            [JsonPropertyName("id")] public string id { get; set; } = String.Empty;
            [JsonPropertyName("type")] public string type { get; set; } = String.Empty;
            [JsonPropertyName("Url")] public string url { get; set; } = String.Empty;
            [JsonPropertyName("Width")] public string width { get; set; } = String.Empty;
            [JsonPropertyName("Height")] public string height { get; set; } = String.Empty;
        }
    }
    /// <summary>
    /// マップ用クラス。各階の総合データの送信
    /// </summary>
    public class Floor
    {
        public string id { get; set; }
        public Building building { get; set; }
        public int fllor { get; set; }
        public string floorTitle { get; set; } = string.Empty;//フロアタイトル
        public Media floorMap { get; set; }//マップのデータ
        public Company[] companys { get; set; } = new Company[0];//団体一覧
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
            foreach (var item in data.companys)
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
            toJson.id = this.id;
            toJson.floorTitle = this.floorTitle;
            toJson.map = this.floorMap;
            foreach (var item in this.groups)
            {
                toJson.companys.Add(item.id, item);
            }
            return JsonSerializer.Serialize(toJson, typeof(Bloom.Shared.Company), options);
        }
        private class Json
        {
            [JsonPropertyName("id")] public string id { get; set; }
            /// <summary>
            /// 階の名前
            /// </summary>
            [JsonPropertyName("floorTitle")] public string floorTitle { get; set; }
            [JsonPropertyName("map")] public Media map { get; set; }
            [JsonPropertyName("groups")] public Dictionary<string, Company> companys { get; set; }
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
    public class Event
    {
        public string id { get; set; } = string.Empty;
        public string title { get; set; } = string.Empty;
        public string url { get; set; } = string.Empty;
        public Media sumbnaill { get; set; } = new Media();
        public EventType type { set; get; } = EventType.Record;
        public async Task ConvertFromJson(Stream json, JsonSerializerOptions options = null)
        {
            //シリアライズオプションの設定
            if (options == null)
            {
                options = Bloom.Shared.Setting.Media.options1;
            }
            var data = await JsonSerializer.DeserializeAsync<Json>(json, options);
            this.title = data.title;
            this.url = data.url;
            await this.sumbnaill.ConvertFromJson(data.sumbnaill);
            try
            {
                this.type = (EventType)Enum.Parse(typeof(EventType),data.type);
            }
            catch {}
        }

        public async Task ConvertFromJson(string json, JsonSerializerOptions options = null)
        {
            var memory = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await ConvertFromJson(memory, options);
        }

        public async Task<string> ConvertToJson(JsonSerializerOptions options = null)
        {
            var data = new Json();
            data.title = this.title;
            data.id = this.id;
            data.type = this.type.ToString();
            data.sumbnaill = await this.sumbnaill.ConvertToJson();
            if(options == null)
            {
                options = Bloom.Shared.Setting.Media.options1;
            }
            return JsonSerializer.Serialize(this, typeof(Bloom.Shared.Event.Json), options);
        }

        private class Json
        {
            [JsonPropertyName("id")] public string id { get; set; }
            [JsonPropertyName("title")] public string title { get; set; }
            [JsonPropertyName("url")] public string url { get; set; }
            [JsonPropertyName("sumbnaill")] public string sumbnaill { get; set; }
            [JsonPropertyName("event_type")] public string type { set; get; }
        }
    }
    /// <summary>
    /// イベントタイプ用列挙型
    /// </summary>
    public enum EventType
    {
        Live = 0,
        Record = 1,
    }
}
