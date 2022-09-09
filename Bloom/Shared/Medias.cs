using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

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
}
