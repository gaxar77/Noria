using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.IO;

namespace WpfApp1
{
    public class FolderViewModel : INotifyPropertyChanged
    {
        private string _directoryPath;
        private ImmutableFolder _folder;

        private Stack<ImmutableFolder> _prevFolders
            = new Stack<ImmutableFolder>();

        private Stack<ImmutableFolder> _nextFolders
            = new Stack<ImmutableFolder>();

        public ImmutableFolder Folder
        {
            get { return _folder;  }

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

        private void OnNavigated(ImmutableFolder prevFolder, ImmutableFolder newFolder)
        {
            var args = new FolderViewModelNavigationEventArgs(prevFolder, newFolder);

            Navigated?.Invoke(this, args);
        }

        public bool TryRefresh()
        {
            try
            {
                Folder = ImmutableFolder.CreateFolder(DirectoryPath);
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
                _prevFolders.Push(Folder);

            if (clearNextFolders)
                _nextFolders.Clear();

            Folder = null;

            try
            {
                Folder = ImmutableFolder.CreateFolder(folderPath);

                _directoryPath = Folder.FolderPath;
                OnPropertyChanged(nameof(DirectoryPath));

                OnNavigated(prevFolder, Folder);
                return true;
            }
            catch (Exception exc)
            {
                return false;
            }
        }

        public bool TryNavigateBack()
        {
            if (_prevFolders.Count == 0)
                return true;

            var prevFolder = _prevFolders.Pop();

            if (prevFolder == null)
                return true;

            _nextFolders.Push(Folder);

            return TryNavigate(prevFolder.FolderPath, true, false);
        }

        public bool TryNavigateForward()
        {
            if (_nextFolders.Count == 0)
                return true;

            var nextFolder = _nextFolders.Pop();

            if (nextFolder == null)
                return true;

            return TryNavigate(nextFolder.FolderPath, false, false);
        }
    }

    public enum ImmutableFolderItemType
    {
        File,
        Folder
    }
}
