namespace Noria.ViewModel
{
    public class FolderTreeViewModelFileSystemViewItemProvider : IFileSystemViewItemProvider
    {
        FolderTreeViewModel _folderTreeViewModel;
        public FolderTreeViewModelFileSystemViewItemProvider(FolderTreeViewModel folderTreeViewModel)
        {
            _folderTreeViewModel = folderTreeViewModel;
        }

        public IFileSystemViewItem GetFileSystemItem(string path)
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
