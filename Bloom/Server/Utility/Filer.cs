using System.Text;
using System.IO;

namespace Bloom.Server.Utility
{
    public static class Filer
    {
        /// <summary>
        /// ファイルないの文字列全文取得関数(非同期)
        /// </summary>
        public static async Task<string> GetFileText(string path)
        {
            string result = "";
            using (StreamReader sr = new StreamReader(new FileStream(
                DirectoryManeger.GetAbsotoblePath(path), //./data/FloorIndexer.json.jsonを展開
                FileMode.Open, //ファイルを開くに。
                FileAccess.Read, //読み取り専用
                FileShare.ReadWrite)))//書き込み中を無視
            {
                result = await sr.ReadToEndAsync();
            }
            return result;
        }
    }
}
