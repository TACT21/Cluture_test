using Bloom.Server.Utility.Format;
using Bloom.Server.Utility;
using Bloom.Shared;
using MimeTypes;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Immutable;

namespace Bloom.Server.Filer.Handler
{
    internal static class MediaHandler
    {
        public static async Task<string> BullyMedia(MediaCarryer media,string name = "" ,string coment = "")
        {
            var extinction = Path.GetExtension(media.extinction);
            var madianame = new Guid().ToString("N");
            using (var sr = new FileStream(DirectoryManeger.GetWebStoragePath("/media/" + madianame + extinction), FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                await sr.WriteAsync(media.content, 0, media.content.Length);
            }
            if(name == "")
            {
                name = new Guid().ToString("N");
            }
            var list = new List<MediaExpression>();
            if (File.Exists(DirectoryManeger.GetAbsotoblePath("/data/media/" + name +".json")))
            {
                File.Create(DirectoryManeger.GetAbsotoblePath("/data/media/" + name + ".json"));
            }
            using (var fs = new FileStream(DirectoryManeger.GetAbsotoblePath("/data/media/" + name + ".json"), FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
            {
                list = await JsonSerializer.DeserializeAsync<List<MediaExpression>>(fs);
                using (var sr = new StreamWriter(fs))
                {
                    list.Add(new MediaExpression() { id = madianame , url = DirectoryManeger.GetWebStoragePath("/media/" + madianame + extinction),recent = false,coment = coment });
                    await sr.WriteAsync(JsonSerializer.Serialize(list));
                }
            }
            return madianame;
        }
        public static async Task<string> BullyMedia(Media media, string name = "", string coment = "")
        {
            var madianame = "URL";
            if (media.type == null || media.type == "" || media.Url == null|| media.Url == "")
            {
                throw new ArgumentNullException();
            }
            var carryer = new MediaCarryer();
            carryer.extinction = MimeTypeMap.GetExtension(media.type);
            var reg = new Regex("data:/w*/x2f /w*;base64");
            var m = reg.Matches(media.Url);
            if (m[0].Value == media.Url.Substring(0, m[0].Value.Length))
            {
                carryer.content = Encoding.UTF8.GetBytes(media.Url.Substring(m[0].Value.Length));
                madianame = await BullyMedia(carryer, name, coment);
            }
            else
            {
                var list = new List<MediaExpression>();
                if (File.Exists(DirectoryManeger.GetAbsotoblePath("/data/media/" + name + ".json")))
                {
                    File.Create(DirectoryManeger.GetAbsotoblePath("/data/media/" + name + ".json"));
                }
                using (var fs = new FileStream(DirectoryManeger.GetAbsotoblePath("/data/media/" + name + ".json"), FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
                {
                    list = await JsonSerializer.DeserializeAsync<List<MediaExpression>>(fs);
                    using (var sr = new StreamWriter(fs))
                    {
                        list.Add(new MediaExpression() { id = madianame, url = media.Url, recent = false, coment = coment });
                        await sr.WriteAsync(JsonSerializer.Serialize(list));
                    }
                }
            }
            return madianame;
        }
        private static async Task<Media> GainMedia(MediaExpression aim, bool convert)
        {
            var result = new Media();
            result.type = MimeTypeMap.GetMimeType(aim.url);
            result.Url = aim.url;
            if (aim.url.IndexOf("http") != -1)
            {
                aim.url = DirectoryManeger.GetWebStoragePath(aim.url);
            }
            else
            {
                convert = false;
            }
            result.isfream = false;
            result.isWide = false;
            if (result.type.IndexOf("image") != -1)
            {
                using (var image = Image.Load(aim.url))
                {
                    if (image.Height < image.Width)
                    {
                        result.isWide = true;
                    }
                    result.Width = image.Width;
                    result.Height = image.Height;
                }
            }
            if (convert)
            {
                var text = "data:" + result.type + "; base64,";
                string[] strings;
                using (var sr = new FileStream(result.Url, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var tasks = new List<Task<string>>();
                    var count = (sr.Length % 24 == 0) ? sr.Length / 24 : ((sr.Length - (sr.Length % 24)) / 24) + 1;
                    for (var i = 0; i < count; i++)
                    {
                        var x = i;
                        var task = (async () =>
                        {
                            byte[] bytes = new byte[24];
                            await sr.ReadAsync(bytes, i * 24, 24);
                            return Convert.ToBase64String(bytes);
                        });
                        tasks.Add(task.Invoke());
                    }
                    strings = await Task.WhenAll(tasks);
                    tasks.Clear();
                }
                foreach (var item in strings)
                {
                    text += item;
                }
                aim.url = text;
            }
            return result;
        }
        public static async Task<Media> RetriveMedia(string id, bool convert = false)
        {
            if (File.Exists(DirectoryManeger.GetAbsotoblePath("/data/media/" + id + ".json")))
            {
                throw new ArgumentException();
            }
            MediaExpression? aim = new MediaExpression();
            var list = new List<MediaExpression>();
            using (var fs = new FileStream(DirectoryManeger.GetAbsotoblePath("/data/media/" + id + ".json"), FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
            {
                list = await JsonSerializer.DeserializeAsync<List<MediaExpression>>(fs);
            }
            foreach (var item in list)
            {
                if (item.recent)
                {
                    aim = item;
                }
            }
            list.Clear();
            var result = await GainMedia(aim, convert);
            result.id = id;
            return result;
        }
        public static async Task <List<Media>> RetriveMediaList(string id, bool convert = false)
        {
            var result = new List<Media>();
            if (File.Exists(DirectoryManeger.GetAbsotoblePath("/data/media/" + id + ".json")))
            {
                throw new ArgumentException();
            }
            MediaExpression? aim = new MediaExpression();
            var list = new List<MediaExpression>();
            using (var fs = new FileStream(DirectoryManeger.GetAbsotoblePath("/data/media/" + id + ".json"), FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
            {
                list = await JsonSerializer.DeserializeAsync<List<MediaExpression>>(fs);
            }
            foreach (var item in list)
            {
                result.Add(await GainMedia(item, false));
            }
            result[0].id = id;
            list.Clear();
            return result;
        }
        public static async Task ReplaceMedia(string id,string mediaId)
        {
            if (File.Exists(DirectoryManeger.GetAbsotoblePath("/data/media/" + id + ".json")))
            {
                throw new ArgumentException();
            }
            var list = new List<MediaExpression>();
            using (var fs = new FileStream(DirectoryManeger.GetAbsotoblePath("/data/media/" + id + ".json"), FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
            {
                list = await JsonSerializer.DeserializeAsync<List<MediaExpression>>(fs);
                bool exist = false;
                int elder = list.Count;
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].id == mediaId)
                    {
                        exist = true;
                        list[i].recent = true;
                    }
                    else if(list[i].recent && !exist)
                    {
                        elder = i;
                        list[i].recent = false;
                    }
                    else
                    {
                        list[i].recent = false;
                    }
                }
                if (!exist)
                {
                    if(elder != list.Count)
                    {
                        list[elder].recent = true;
                    }
                    throw new DirectoryNotFoundException();
                }
                using (var sr = new StreamWriter(fs))
                {
                    await sr.WriteAsync(JsonSerializer.Serialize(list));
                }
            }
        }
    }
}