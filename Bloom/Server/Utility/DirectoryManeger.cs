namespace Bloom.Server.Utility
{
    public static class DirectoryManeger
    {
        private static string path = @"/srv/masuosai";
        private static string webStoragePath = @"/srv/http";
        public static string GetAbsotoblePath (string relative)
        {
            return (path + relative);
        }
        public static string GetWebStoragePath(string relative)
        {
            return (webStoragePath + relative);
        }
    }
}
