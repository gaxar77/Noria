using System.Collections.Generic;
using System.Linq;

namespace Noria.FilesAndFolders
{
    public class FileSystemPath
    {
        public string Path { get; private set; }
        public FileSystemPathComponent[] PathComponents { get; private set; }
        public FileSystemPath(string path)
        {
            Path = path;
            PathComponents = GetPathComponents(path);
        }

        private FileSystemPathComponent[] GetPathComponents(string path)
        {
            var pathComponents = new List<FileSystemPathComponent>();

            while (path != null)
            {
                /*
                var fileSystemObjectName = System.IO.Path.GetFileName(path);
                if (fileSystemObjectName == String.Empty)
                    fileSystemObjectName = path;
                */

                var pathComponent = new FileSystemPathComponent(path);

                path = System.IO.Path.GetDirectoryName(path);

                pathComponents.Add(pathComponent);
            }

            pathComponents.Reverse();

            return pathComponents.ToArray();
        }
    }
}