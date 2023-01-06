using Bloom.Server.Utility;
using Bloom.Server.Utility.Format;
using Bloom.Server.Filer.Handler;
using Bloom.Shared;
using System.Text;
using System.Xml.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bloom.Server.Utility.Format
{

    public class BuildingExpression
    {
        public Building building { set; get; }
        public Dictionary<int, string> paths = new();
        public string name { set; get; }
    }
    public class FloorExpression {
        public string version { get; set; } = string.Empty;
        public string id { get; set; } = string.Empty;
        public Building building { get; set; }
        public int froor { get; set; }
        public string floorTitle { get; set; } = string.Empty;//フロアタイトル
        public string floorMap { get; set; }//マップのデータ
        public List<string> groups { get; set; } = new();//団体一覧
    }
    public class GreadExpression
    {
        public string version { get; set; } = string.Empty;
        public string id { get; set; } = string.Empty;
        public int gread { get; set; } = 0;
        public List<string> groups { get; set; } = new();//団体一覧のid
        public string name { get; set; } = string.Empty;
        public async Task<Floor> ConvertToFloor(bool convert = false)
        {
            var floor = new Floor();
            floor.fllor = this.gread;
            floor.floorTitle = this.name;
            var task = new List<Task<Company>>();
            foreach (var group in this.groups)
            {
                floor.groups.Add(await CompanyHandler.RetrieveGroup(group, convert));
            }
            return floor;
        }
        public void ConvertFromFloor(in Floor floor)
        {
            this.id = floor.id;
            this.name = floor.floorTitle;
            foreach (var item in floor.groups)
            {
                this.groups.Add(item.id);
            }
        }
    }
    public class CompanyExpression
    {
        public string version { get; set; } = string.Empty;
        public string id { get; set; }
        public string name { get; set; }
        public string enname { get; set; }
        public List<string> posterUrl { get; set; } = new List<string>();
        public string? comment { get; set; }
        public List<string> videoUrl { get; set; } = new List<string>();
        public List<string> cmUrl { get; set; } = new List<string>();
        public string? location { get; set; }
        public string? contentUrl { get; set; }
        public async Task<Company> ConvertToCompany(bool convert = false)
        {
            var group = new Company();
            group.id = id;
            group.name = name;
            group.enname = enname;
            group.comment = comment;
            group.location = location;
            group.contentUrl = contentUrl;
            foreach (var item in posterUrl)
            {
                group.posterUrl.Add(await MediaHandler.RetriveMedia(item, convert));
            }
            foreach (var item in videoUrl)
            {
                group.posterUrl.Add(await MediaHandler.RetriveMedia(item, convert));
            }
            foreach (var item in cmUrl)
            {
                group.posterUrl.Add(await MediaHandler.RetriveMedia(item, convert));
            }
            return group;
        }
        public void ConvertFromCompany(in Company company)
        {
            this.id = company.id;
            this.name = company.name;
            this.enname = company.enname;
            this.comment = company.comment;
            this.contentUrl = company.contentUrl;
            foreach (var item in company.cmUrl)
            {
                if (item != null)
                {
                    this.cmUrl.Add(item.id);
                }
            }
            foreach (var item in company.posterUrl)
            {
                if (item != null)
                {
                    this.posterUrl.Add(item.id);
                }
            }
            foreach (var item in company.videoUrl)
            {
                if (item != null)
                {
                    this.videoUrl.Add(item.id);
                }
            }
        }
    }
    public class MediaCarryer
    {
        public string extinction = string.Empty;
        public byte[] content = new byte[0];
    }
    public class MediaExpression
    {
        public string id = "";
        public string url = "";
        public bool recent = false;
        public string coment = "";
    }
    public class DataExpression<T>
    {
        public T? data;
        public bool recent = false;
    }
    public class FileExpression<T>
    {
        [JsonPropertyName("Version")] public string ver = "0";
        public List<DataExpression<T>> values = new List<DataExpression<T>>();
    }
}
