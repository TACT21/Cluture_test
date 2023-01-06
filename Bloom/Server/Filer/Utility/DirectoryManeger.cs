namespace Bloom.Server.Utility
{
    public static class DirectoryManeger
    {
        public static string path { private set; get; } = @"/srv/masuosai";
        public static string webStoragePath { private set; get; } = @"/srv/http";
        public static string GetAbsotoblePath (string relative)
        {
            return (path + relative);
        }
        public static string GetWebStoragePath(string relative)
        {
            return (webStoragePath + relative);
        }
        public static string ConvertPath2Id(string path)
        {
            return Path.GetFileName(path).TrimEnd(new char[5] { '.', 'j', 's', 'o', 'n' });
        }
    }
}
