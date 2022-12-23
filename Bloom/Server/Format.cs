using Bloom.Shared;
using System.Text;
using System.Xml.Linq;
using System.Text.Json;

namespace Bloom.Server.Format
{

    public class BuildingExpression
    {
        public Building building { set; get; }
        public Dictionary<int, string> paths = new();
        public string name { set; get; }
    }

    public class FloorExpression {
        public string id { get; set; } = string.Empty;
        public Building building { get; set; }
        public int froor { get; set; }
        public string floorTitle { get; set; } = string.Empty;//フロアタイトル
        public string floorMap { get; set; }//マップのデータ
        public List<string> groups { get; set; } = new();//団体一覧
    }

    public class GroupExpression
    {
        public string id { get; set; }
        public string name { get; set; }
        public string enname { get; set; }
        public List<string> posterUrl { get; set; } = new List<string>();
        public string? comment { get; set; }
        public List<string> videoUrl { get; set; } = new List<string>();
        public List<string> cmUrl { get; set; } = new List<string>();
        public string? location { get; set; }
        public string? contentUrl { get; set; }
        public async Task<Group> ConvertToGroup(bool shorten = false)
        {
            var group = new Group();
            group.id = id;
            group.name = name;
            group.enname = enname;
            group.comment = comment;
            group.location = location;
            group.contentUrl = contentUrl;
            if (!shorten)
            {
                foreach (var item in posterUrl)
                {
                    using (var sr = new FileStream(item, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        group.posterUrl.Add(await JsonSerializer.DeserializeAsync<Media>(sr));
                    }
                }
                foreach (var item in videoUrl)
                {
                    using (var sr = new FileStream(item, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        group.videoUrl.Add(await JsonSerializer.DeserializeAsync<Media>(sr));
                    }
                }
                foreach (var item in cmUrl)
                {
                    using (var sr = new FileStream(item, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        group.cmUrl.Add(await JsonSerializer.DeserializeAsync<Media>(sr));
                    }
                }
            }
            return group;
        }
    }
}
