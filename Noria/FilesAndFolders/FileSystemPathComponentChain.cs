using Noria.ViewModel;
using System;

namespace Noria.FilesAndFolders
{
    public class FileSystemPathComponentChainViewItemProvider : IFileSystemViewItemProvider
    {
        private object _syncRoot = new object();
        private FileSystemPathComponentChain _pathComponentChain;
        public FileSystemPathComponentChain PathComponentChain
        {
            get 
            {
                lock (_syncRoot)
                {
                    return _pathComponentChain;
                }
            }

            set
            {
                lock (_syncRoot)
                {
                    _pathComponentChain = value;
                }
            }
        }
  
        public IFileSystemViewItem GetFileSystemViewItem(string path)
        {
            if (PathComponentChain == null)
                return null;

            var link = PathComponentChain.FirstLink;
            while (link != null)
            {
                if (link.PathComponent.Path == path)
                    return link;

                link = link.NextLink;
            }

            return null;
        }

    }
    public class FileSystemPathComponentChain
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

                    link.Deleted += (sender, e) =>
                    {
                        OnDeleted();
                    };

                }
            }

            FirstLink = link;
        }

        public event EventHandler<FileSystemPathComponentChainUpdatedEventArgs> Updated;
        public event EventHandler Deleted;
        private void OnUpdated(FileSystemPathComponentLink lastPathComponentLink)
        {
            var e = new FileSystemPathComponentChainUpdatedEventArgs(
                lastPathComponentLink.PathComponent.Path);

            Updated?.Invoke(this, e);
        }

        private void OnDeleted()
        {
            var e = new EventArgs();

            Deleted?.Invoke(this, e);
        }
    }
}