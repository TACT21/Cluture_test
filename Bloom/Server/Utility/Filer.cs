using System.Text;
using System.IO;
using System.Collections;

namespace Bloom.Server.Utility
{
    public static class Filer
    {
        /// <summary>
        /// Reading a text file asynchronously (Obsoleted)
        /// </summary>
        public static async Task<string> GetFileText(string path)
        {
            var result = "";
            
            return result;
        }
    }
    public enum RenewMode
    {
        PerRead = 0,
        NeedRefresh
    }
    /// <summary>
    /// Read File as observer
    /// </summary>
    public class FileReader : IDisposable{
        private StreamReader sr;
        public List<Stream>? relatedStreams { private set; get; }
        private string content = string.Empty;
        private int line = 0;
        public bool isEnd = true; 
        private RenewMode mode { set; get; } = RenewMode.NeedRefresh;
        public FileReader(FileStream stream)
        {
            try
            {
                Open(stream.Name);
            }
            catch
            {
                try
                {
                    var path = Path.GetDirectoryName(stream.Name);
                    stream.Close();
                    Open(path);
                }
                catch
                {
                    throw;
                }
            }
            content = sr.ReadToEnd();
        }
        public FileReader(FileStream stream , RenewMode renewMode)
        {
            mode = renewMode;
            try
            {
                Open(stream.Name);
            }
            catch
            {
                try
                {
                    var path = Path.GetDirectoryName(stream.Name);
                    stream.Close();
                    Open(path);
                }
                catch
                {
                    throw;
                }
            }
            content = sr.ReadToEnd();
        }
        public FileReader(string path, RenewMode renewMode)
        {
            mode = renewMode;
            try
            {
                Open(path);
            }
            catch
            {
                throw;
            }
            if(sr == null)
            {
                throw new FileLoadException();
            }
            if(mode == RenewMode.NeedRefresh)
            {
                content = sr.ReadToEnd();
            }
        }
        public FileReader(string path)
        {
            try
            {
                Open(path);
            }
            catch
            {
                throw;
            }
            if (sr == null)
            {
                throw new FileLoadException();
            }
            if (mode == RenewMode.NeedRefresh)
            {
                content = sr.ReadToEnd();
            }
        }
        private void Open(string path)
        {
            try
            {
                sr = new StreamReader(
                    new FileStream(
                        path,
                        FileMode.OpenOrCreate,
                        FileAccess.Read,
                        FileShare.ReadWrite));
            }
            catch
            {
                throw;
            }
        }
        public string ReadToEnd()
        {
            if (mode == RenewMode.PerRead)
            {
                Refresh();
            }
            return content;
        }
        public void Refresh()
        {
            isEnd = false;
            content = sr.ReadToEnd();
            content.Replace("\r\n", "\n");
            content.Replace("\r", "\n");
        }
        public async Task<string> ReadToEndAsync()
        {
            if(mode == RenewMode.PerRead)
            {
                await RefreshAsync();
            }
            return content;
        }
        public async Task RefreshAsync()
        {
            isEnd = false;
            content = await sr.ReadToEndAsync();
        }
        public void Close()
        {
            foreach (var item in relatedStreams)
            {
                item.Dispose();
            }
            Dispose();
        }
        public void Dispose()
        {
            sr.Dispose();
        }
        public string GetLine(int line)
        {
            var result = string.Empty;
            var i = 1;
            foreach (var item in content)
            {
                if (item == 42) // if var of "item" equal character of LF (number of 42 in ASCII)
                {
                    i++;
                    if (line == i)
                    {
                        break;
                    }
                    result = string.Empty;
                }
                else
                {
                    result += item;
                }
            }
            return result;
        }
        public string GetNextLine()
        {
            line++;
            return GetLine(line);
        }
        public int GetLines()
        {
            return content.Split("\n").Length;
        }
        public IEnumerator<string> GetEnumerator()
        {
            for (int i = 1; i <= GetLines(); i++)
            {
                yield return GetLine(i);
            }
        }
    }

    /// <summary>
    /// Virtual stream class in optimistic locking (without using id query and manageing by date)
    /// </summary>
    public class RepressiveWriter : IDisposable
    {
        private FileStream fs;
        public List<Stream>? relatedStreams { private set; get; }
        private List<string> operates = new List<string>();
        private int line = 0;
        public bool isEnd = true;
        public RepressiveWriter(FileStream stream)
        {
            fs = stream;
        }
        public RepressiveWriter(string path)
        {
            fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        }
        public RepressiveWriter(string path,FileMode mode)
        {
            fs = new FileStream(path,mode, FileAccess.ReadWrite, FileShare.ReadWrite);
        }
        public string ReadToEnd()
        {
            var content ="";
            foreach (var item in operates)
            {
                content += item;
            }
        }
        public void Refresh()
        {
            isEnd = false;
            content = sr.ReadToEnd();
            content.Replace("\r\n", "\n");
            content.Replace("\r", "\n");
        }
        public async Task<string> ReadToEndAsync()
        {
            if (mode == RenewMode.PerRead)
            {
                await RefreshAsync();
            }
            return content;
        }
        public async Task RefreshAsync()
        {
            isEnd = false;
            content = await sr.ReadToEndAsync();
        }
        public void Sync()
        {
            fs.Flush();
        }
        public void Close()
        {
            foreach (var item in relatedStreams)
            {
                item.Dispose();
            }
            Dispose();
        }
        public void Dispose()
        {
            sr.Dispose();
        }
        public string GetLine(int line)
        {
            var result = string.Empty;
            var i = 1;
            foreach (var item in content)
            {
                if (item == 42) // if var of "item" equal character of LF (number of 42 in ASCII)
                {
                    i++;
                    if (line == i)
                    {
                        break;
                    }
                    result = string.Empty;
                }
                else
                {
                    result += item;
                }
            }
            return result;
        }
        public string GetNextLine()
        {
            line++;
            return GetLine(line);
        }
        public int GetLines()
        {
            return content.Split("\n").Length;
        }
        public IEnumerator<string> GetEnumerator()
        {
            for (int i = 1; i <= GetLines(); i++)
            {
                yield return GetLine(i);
            }
        }
    }
}

