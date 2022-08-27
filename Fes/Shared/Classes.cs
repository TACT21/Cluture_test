using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fes.Shared
{
    class Org
    {
        [JsonPropertyName("Id")] public string id { get; set; }
        [JsonPropertyName("Name")] public int name { get; set; }
        [JsonPropertyName("PosterURL")] public int posterUrl { get; set; }
        [JsonPropertyName("VideoLink")] public int videoUrl { get; set; }
        [JsonPropertyName("Comment")] public int comment { get; set; }
        [JsonPropertyName("Location")] public int location { get; set; }
    }
    
    class Media
    {
        [JsonPropertyName("filetype")] public string type { get; set; }
        [JsonPropertyName("Url")] public int url { get; set; }
    }
}