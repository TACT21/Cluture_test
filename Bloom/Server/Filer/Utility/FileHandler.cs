using Bloom.Server.Utility.Format;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Bloom.Server.Utility;
using System.Collections;

namespace Bloom.Server.Filer.Utility
{
    public class OptimismFileHelper<T>
    {
        public int ver = 0;
        public string dataBasePath = "";
        public List<DataExpression<T>> values = new List<DataExpression<T>>();
        public async Task ChengeAsync(int index, DataExpression<T> item, bool overWrite = false, string path = "")
        {
            var options = new JsonSerializerOptions { WriteIndented = false };//変更しないでください。
            if (path == "" && dataBasePath == "")
            {
                throw new ArgumentNullException();
            }
            else if (path == "")
            {
                path = dataBasePath;
            }
            if (index > values.Count)
            {
                while (values.Count <= index)
                {
                    values.Add(new DataExpression<T>());
                }
            }
            //現在の情報を取得
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
            {
                var file = JsonSerializer.Deserialize<FileExpression<T>>(fs);
                //ファイル内容に更新が見られた場合若しくは、バージョンが同一かそれ以降の場合
                var chenged = (this.ver < Int32.Parse(file.ver)) ? true : false;
                if (this.ver == Int32.Parse(file.ver))
                {
                    chenged = !this.values.Equals(file.values);
                }
                if (chenged)
                {

                }
                var edit = new FileExpression<T>();
                edit.ver = (this.ver + 1).ToString("D10");
                for (int i = 0; i < index - 1; i++)
                {
                    edit.values.Add(file.values[i]);
                }
                Task task;
                var tempName = DirectoryManeger.GetAbsotoblePath("/temp/" + Guid.NewGuid().ToString("N"););
                using (var fs1 = new FileStream(tempName, FileMode.Open, FileAccess.ReadWrite, FileShare.None, 4069, FileOptions.DeleteOnClose))
                {
                    await JsonSerializer.SerializeAsync(fs1, edit, options);
                    var text = File.ReadAllText(tempName);
                    fs.SetLength(0);
                    //書き込み対象物の最初の位置を取得
                    edit.values.Add(file.values[index]);
                    task = JsonSerializer.SerializeAsync(fs1, edit, options);//次のタスク(/書き込み対象物の最後の位置を取得)を先行実行
                    var befor_bytes = Encoding.UTF8.GetBytes(text.TrimEnd(new char[2] { ']', '}' }));
                    await task;
                    text = File.ReadAllText(tempName);
                    fs.SetLength(0);
                    //書き込み対象物の最後の位置を取得
                    edit.values.Add(file.values[index]);
                    task = JsonSerializer.SerializeAsync(fs1, item, options);//次のタスク(書き込み対象物のJsonを取得)を先行実行
                    var bytes = Encoding.UTF8.GetBytes(text.TrimEnd(new char[2] { ']', '}' }));
                    var verText = Encoding.UTF8.GetBytes("{\"Version\":\"" + file.ver + "\"");
                    //書き込み対象物の最後の位置をまでカーソル移動
                    fs.Seek((bytes.Length - verText.Length - 1), SeekOrigin.Begin);
                    var after = new byte[fs.Length - bytes.Length];
                    await fs.ReadAsync(after, bytes.Length, (int)(fs.Length - bytes.Length));
                    //書き込み対象物の最初の位置をまでカーソル移動
                    fs.Seek(befor_bytes.Length - 1, SeekOrigin.Begin);
                    //書き込み対象物を取得
                    await task;
                    var aim = Encoding.UTF8.GetBytes(File.ReadAllText(tempName).TrimEnd('}').TrimStart('{'));
                    //書き込み対象物と、その後ろのものを書き込み
                    fs.Write(aim, 0, aim.Length);
                    await fs.WriteAsync(after, 0, after.Length);
                    //バージョンを書き込み
                    fs.Seek(0, SeekOrigin.Begin);
                    await fs.WriteAsync(verText, 0, verText.Length);
                }
            }
        }
        public async Task ChengeAsync(int index, T item, bool overWrite = false, string path = "")
        {
            var value = new DataExpression<T>() { data = item, recent = false };
            await ChengeAsync(index, value, overWrite, path);
        }
        public async Task OpenAsync(string path)
        {
            this.dataBasePath = path;
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var file = await JsonSerializer.DeserializeAsync<FileExpression<T>>(fs);
                if (file != null)
                {
                    throw new ArgumentNullException();
                }
                this.ver = Int32.Parse(file.ver);
                this.values = file.values;
            }
        }
    }
}
