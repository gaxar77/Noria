using Noria.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Noria.FilesAndFolders
{
    public class FileSystemPathComponentChain : IFileSystemViewItemProvider
    {
        public FileSystemPathComponentLink FirstLink { get; private set; }

        public FileSystemPathComponentChain(string path)
        {
            var fileSystemPath = new FileSystemPath(path);

            var link = (FileSystemPathComponentLink)null;
            for (int i = fileSystemPath.PathComponents.Length - 1; i >= 0; i--)
            {
                link = new FileSystemPathComponentLink(
                    fileSystemPath.PathComponents[i],
                    link);

                if (i == fileSystemPath.PathComponents.Length - 1)
                {
                    link.Updated += (sender, e) =>
                    {
                        OnUpdated((FileSystemPathComponentLink)sender);
                    };

                }
            }

            FirstLink = link;
        }

        public IFileSystemViewItem GetFileSystemViewItem(string path)
        {
            var link = FirstLink;
            while (link != null)
            {
                if (link.PathComponent.Path == path)
                    return link;

                link = link.NextLink;
            }

            return null;
        }

        public event EventHandler<FileSystemPathComponentChainUpdatedEventArgs> Updated;

        private void OnUpdated(FileSystemPathComponentLink lastPathComponentLink)
        {
            var e = new FileSystemPathComponentChainUpdatedEventArgs(
                lastPathComponentLink.PathComponent.Path);

            Updated?.Invoke(this, e);
        }
    }

    public class FileSystemPathComponentChainUpdatedEventArgs
    {
        public string Path { get; private set; }

        public FileSystemPathComponentChainUpdatedEventArgs(string path)
        {
            Path = path;
        }
    }
    public class FileSystemPathComponentLink : IFileSystemViewItem,
        IFileSystemViewItemUpdatable
    {
        public FileSystemPathComponent PathComponent { get; private set; }
        public FileSystemPathComponentLink NextLink { get; private set; }

        public string FileSystemItemPath => PathComponent.Path;

        public FileSystemPathComponentLink(FileSystemPathComponent pathComponent,
            FileSystemPathComponentLink nextLink)
        {
            PathComponent = pathComponent;
            NextLink = nextLink;
        }

        public void Update(string itemPath, string newItemPath)
        {
            if (newItemPath != null)
            {
                PathComponent = new FileSystemPathComponent(newItemPath);
                if (NextLink != null)
                {
                    var nextPathComponent = PathComponent.CreateNextComponent(
                        NextLink
                            .PathComponent
                            .FileSystemObjectName);

                    NextLink.Update(NextLink.FileSystemItemPath,
                                    nextPathComponent.Path);
                }
                OnUpdated();
            }
        }

        public event EventHandler Updated;

        private void OnUpdated()
        {
            var e = new EventArgs();

            Updated?.Invoke(this, e);
        }
    }

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