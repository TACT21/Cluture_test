using System.Text.Json;
using System.Text.Json.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Bloom.Shared
{
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
    }

    public class Media
    {
        public bool isfream { get; set; }
        public string Url { set; get; }
    }

    public class GroupJson
    {
        [JsonPropertyName("Id")] public string id { get; set; }
        [JsonPropertyName("Name")] public string name { get; set; }
        [JsonPropertyName("EnName")] public string enName { get; set; }
        [JsonPropertyName("PosterURL")] public string? posterUrl { get; set; }
        [JsonPropertyName("Comment")] public string? comment { get; set; }
        [JsonPropertyName("VideoLink")] public string? videoUrl { get; set; }
        [JsonPropertyName("CMLink")] public string? cmUrl { get; set; }
        [JsonPropertyName("Location")] public string? location { get; set; }
        [JsonPropertyName("ContentLink")] public string? contentUrl { get; set; }

        public Group Convert()
        {
            var result = new Group();
            result.id = this.id;
            result.name = this.name;
            result.enname = this.enName;
            result.posterUrl = JsonSerializer.Deserialize<List<Media>>(this.posterUrl);
            result.comment = this.comment;
            result.cmUrl = JsonSerializer.Deserialize<List<Media>>(this.cmUrl);
            result.videoUrl = JsonSerializer.Deserialize<List<Media>>(this.videoUrl);
            result.location = this.location;
            result.contentUrl = this.contentUrl;
            return result;
        }
    }

    public class MediaJson
    {
        [JsonPropertyName("filetype")] public string type { get; set; }
        [JsonPropertyName("Url")] public string url { get; set; }

        public Media Convert()
        {
            var result = new Media();
            result.isfream = this.type == "ifream" ? true : false;
            result.Url = this.url;
            return result;
        }
    }

    public class GroupCompactJson
    {
        [JsonPropertyName("Id")] public string id { get; set; }
        [JsonPropertyName("Name")] public string name { get; set; }
        [JsonPropertyName("Comment")] public string? comment { get; set; }
        [JsonPropertyName("Location")] public string? location { get; set; }

        public Group Convert2Nomal()
        {
            var result = new Group();
            result.id = this.id;
            result.name = this.name;
            result.comment = this.comment;
            result.location = this.location;
            return result;
        }

        public Group Marge (Group baseG)
        {
            baseG.id = this.id;
            baseG.name = this.name;
            baseG.comment = this.comment;
            baseG.location = this.location;
            return baseG;
        }

        public GroupCompact Convert()
        {
            var result = new GroupCompact();
            result.id = this.id;
            result.name = this.name;
            result.comment = this.comment;
            result.location = this.location;
            return result;
        }
    }
    public class GroupCompact
    {
        public string id { get; set; }
        public string name { get; set; }
        public string? comment { get; set; }
        public string? posterUrl { get; set; }
        public string? location { get; set; }
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

        private class Json
        {
            [JsonPropertyName("floorTitle")] public string floorTitle { get; set; }
            [JsonPropertyName("groups")] public string groups { get; set; }
        }
    }
}
