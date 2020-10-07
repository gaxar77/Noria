using System.IO;

namespace WpfApp1
{
    public class FileUtility
    {
        public static long GetFileSize(string path)
        {
            var fileInfo = new FileInfo(path);

            return fileInfo.Length;
        }
    }
}
