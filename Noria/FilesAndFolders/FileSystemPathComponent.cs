using System;
using System.IO;

namespace Noria.FilesAndFolders
{
    public class FileSystemPathComponent
    {
        public string FileSystemObjectName { get; private set; }
        public string Path { get; private set; }
        public FileSystemPathComponent(string fileSystemObjectName, string path)
        {
            FileSystemObjectName = fileSystemObjectName;
            Path = path;
        }
        public FileSystemPathComponent(string path)
        {
            FileSystemObjectName = System.IO.Path.GetFileName(path);
            if (FileSystemObjectName == String.Empty)
                FileSystemObjectName = path;

            Path = path;
        }

        public FileSystemPathComponent CreateNextComponent(string fileSystemObjectName)
        {
            var path = System.IO.Path.Combine(Path, fileSystemObjectName);
            var pathComponent = new FileSystemPathComponent(path);

            return pathComponent;
        }
    }
}
