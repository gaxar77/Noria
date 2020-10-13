namespace Noria.ViewModel
{
    public class FolderViewModelFileSystemViewItemProvider : IFileSystemViewItemProvider
    {
        FolderViewModel _folderViewModel;
        public FolderViewModelFileSystemViewItemProvider(FolderViewModel folderViewModel)
        {
            _folderViewModel = folderViewModel;
        }

        public IFileSystemViewItem GetFileSystemItem(string path)
        {
            if (_folderViewModel.DirectoryPath == path)
            {
                return _folderViewModel.Folder;
            }
            else
            {
                return (IFileSystemViewItem)_folderViewModel.Folder.GetItemByPath(path);
            }
        }
    }
    //public class FolderInvalidatedEventArgs
    //{
     //   public string NewFolderPath { get; private set; }
        
    //}
}