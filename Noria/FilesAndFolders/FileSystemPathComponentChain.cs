using Noria.ViewModel;
using System;

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
}