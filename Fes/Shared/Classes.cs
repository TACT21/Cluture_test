using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fes.Shared
{
    class Org
    {
        [JsonPropertyName("Id")] public string id { get; set; }
        [JsonPropertyName("Name")] public string name { get; set; }
        [JsonPropertyName("PosterURL")] public string? posterUrl { get; set; }
        [JsonPropertyName("Comment")] public string? comment { get; set; }
        [JsonPropertyName("VideoLink")] public string? videoUrl { get; set; }
        [JsonPropertyName("CMLink")] public string? cmUrl { get; set; }
        [JsonPropertyName("Location")] public string? location { get; set; }
        [JsonPropertyName("ContentLink")] public string? contentUrl { get; set; }
    }
    
    class Media
    {
        [JsonPropertyName("filetype")] public string type { get; set; }
        [JsonPropertyName("Url")] public int url { get; set; }
    }
}