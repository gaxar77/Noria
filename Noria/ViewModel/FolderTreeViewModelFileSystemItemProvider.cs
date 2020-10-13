namespace Noria.ViewModel
{
    public class FolderTreeViewModelFileSystemItemProvider : IFileSystemItemProvider
    {
        FolderTreeViewModel _folderTreeViewModel;
        public FolderTreeViewModelFileSystemItemProvider(FolderTreeViewModel folderTreeViewModel)
        {
            _folderTreeViewModel = folderTreeViewModel;
        }

        public IFileSystemItem GetFileSystemItem(string path)
        {
            foreach (FolderTreeItemModel rootFolder in _folderTreeViewModel.RootFolders)
            {
                var item = GetFolderTreeItem(rootFolder, path);
                if (item != null)
                    return item;
            }

            return null;
        }

        private FolderTreeItemModel GetFolderTreeItem(FolderTreeItemModel item, string path)
        {
            if (item.FolderPath == path)
                return item;

            foreach (FolderTreeItemModel subFolder in item.SubFolders)
            {
                var fsItem = GetFolderTreeItem(subFolder, path);
                if (fsItem != null)
                    return fsItem;
            }

            return null;
        }
    }
}
