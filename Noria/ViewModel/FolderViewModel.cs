using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Navigation;

namespace Noria.ViewModel
{
    public class FolderViewModelFileSystemItemProvider : IFileSystemItemProvider
    {
        FolderViewModel _folderViewModel;
        public FolderViewModelFileSystemItemProvider(FolderViewModel folderViewModel)
        {
            _folderViewModel = folderViewModel;
        }

        public IFileSystemItem GetFileSystemItem(string path)
        {
            if (_folderViewModel.DirectoryPath == path)
            {
                return _folderViewModel.Folder;
            }
            else
            {
                return (IFileSystemItem)_folderViewModel.Folder.GetItemByPath(path);
            }
        }
    }
    public class FolderViewModel : INotifyPropertyChanged
    {
        private const int MaxStackItemCount = 20;

        private string _directoryPath;
        private FolderModel _folder;

        private List<FolderModel> _prevFolders
            = new List<FolderModel>();

        private List<FolderModel> _nextFolders
            = new List<FolderModel>();

        public FolderViewModel()
        {
        }

        public FolderModel Folder
        {
            get { return _folder; }

            set
            {
                _folder = value;

                OnPropertyChanged(nameof(Folder));
            }
        }

        public string DirectoryPath
        {
            get { return _directoryPath; }

            set
            {
                _directoryPath = Path.GetFullPath(value);
                OnDirectoryPathChanged(value);

                OnPropertyChanged(nameof(DirectoryPath));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<FolderViewModelNavigationEventArgs> Navigated;

        private void OnDirectoryPathChanged(string oldValue)
        {
            TryNavigate(DirectoryPath);
        }

        private void OnPropertyChanged(string propertyName)
        {
            var args = new PropertyChangedEventArgs(propertyName);

            PropertyChanged?.Invoke(this, args);
        }

        private void OnNavigated(FolderModel prevFolder, FolderModel newFolder)
        {
            var args = new FolderViewModelNavigationEventArgs(prevFolder, newFolder);

            Navigated?.Invoke(this, args);
        }

        public bool TryRefresh()
        {
            try
            {

                Folder = FolderModel.CreateFolder(DirectoryPath);
                
                OnNavigated(_prevFolders.LastOrDefault(), Folder);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool TryNavigate(string folderPath, bool suppressSavingInHistory = false, bool clearNextFolders = true)
        {
            var prevFolder = Folder;
            if (!suppressSavingInHistory)
                _prevFolders.Add(Folder);

            if (clearNextFolders)
                _nextFolders.Clear();

            Folder = null;

            try
            {
                Folder = FolderModel.CreateFolder(folderPath);

                _directoryPath = Folder.FolderPath;

                OnPropertyChanged(nameof(DirectoryPath));

                OnNavigated(prevFolder, Folder);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                LimitPrevFoldersStack();
            }
        }

        public bool TryNavigateBack()
        {
            try
            {
                if (_prevFolders.Count == 0)
                    return true;

                var prevFolder = _prevFolders.Last();
                _prevFolders.Remove(prevFolder);

                if (prevFolder == null)
                    return true;

                _nextFolders.Add(Folder);

                return TryNavigate(prevFolder.FolderPath, true, false);
            }
            finally
            {
                LimitNextFoldersStack();
            }
        }

        public bool TryNavigateForward()
        {

            if (_nextFolders.Count == 0)
                return true;

            var nextFolder = _nextFolders.Last();
            _nextFolders.Remove(nextFolder);

            if (nextFolder == null)
                return true;

            return TryNavigate(nextFolder.FolderPath, false, false);
        }

        public void LimitPrevFoldersStack()
        {
            if (_prevFolders.Count > 20)
                _prevFolders.RemoveAt(0);
        }

        public void LimitNextFoldersStack()
        {
            if (_nextFolders.Count > 20)
                _nextFolders.RemoveAt(0);
        }
    }
    //public class FolderInvalidatedEventArgs
    //{
     //   public string NewFolderPath { get; private set; }
        
    //}
}