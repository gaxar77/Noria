using Noria.ViewModel;
using System;

namespace Noria.FilesAndFolders
{
    public class FileSystemPathComponentLink : IFileSystemViewItem,
        IFileSystemViewItemUpdatable, IFileSystemViewItemDeletable
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
        public event EventHandler Deleted;

        private void OnUpdated()
        {
            var e = new EventArgs();

            Updated?.Invoke(this, e);
        }

        private void OnDeleted()
        {
            var e = new EventArgs();

            Deleted?.Invoke(this, e);
        }

        public void Delete()
        {
            NextLink?.Delete();

            OnDeleted();
        }
    }
}