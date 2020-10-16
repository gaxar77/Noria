using Noria.FilesAndFolders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Noria.ViewModel
{
    public class FolderViewModel : INotifyPropertyChanged
    {
        private const int MaxStackItemCount = 20;

        private string _directoryPath;
        private FolderModel _folder;

        private List<FolderModel> _prevFolders
            = new List<FolderModel>();

        private List<FolderModel> _nextFolders
            = new List<FolderModel>();

        private FileSystemPathComponentChain _pathComponentChain;
        private FileSystemPathComponentChainViewItemProvider
            _pathComponentChainViewItemProvider;

        GlobalFileSystemWatcherAdapter _fileSystemWatcher;

        public FolderViewModel()
        {
            var fileSystemItemProvider =
                new FolderViewModelFileSystemViewItemProvider(this);

            GlobalFileSystemWatcherAdapter
                .Instance
                .FileSystemItemProviders
                .Add(fileSystemItemProvider);

            _pathComponentChainViewItemProvider =
                new FileSystemPathComponentChainViewItemProvider();

            GlobalFileSystemWatcherAdapter
                .Instance
                .FileSystemItemProviders
                .Add(_pathComponentChainViewItemProvider);
        }
        public bool DoesUpdateFromFileSystem()
        {
            return _fileSystemWatcher.HasFileSystemWatcher(DirectoryPath);
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

                Folder = FolderModel.CreateFolder(DirectoryPath, this);
                
                OnNavigated(_prevFolders.LastOrDefault(), Folder);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void SetDirectoryPathComponentChain()
        {
            if (_pathComponentChain != null)
            {
                _pathComponentChain.Updated -= _pathComponentChain_Updated;
                _pathComponentChain.Deleted -= _pathComponentChain_Deleted;
            }

            _pathComponentChain = new FileSystemPathComponentChain(
                _directoryPath);

            _pathComponentChainViewItemProvider.PathComponentChain = _pathComponentChain;

            _pathComponentChain.Updated += _pathComponentChain_Updated;
            _pathComponentChain.Deleted += _pathComponentChain_Deleted;
        }

        private void _pathComponentChain_Deleted(object sender, EventArgs e)
        {
            TryRefresh();
        }

        private void _pathComponentChain_Updated(object sender, FileSystemPathComponentChainUpdatedEventArgs e)
        {
            TryNavigate(e.Path, true);
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
                Folder = FolderModel.CreateFolder(folderPath, this);

                _directoryPath = Folder.FolderPath;

                SetDirectoryPathComponentChain();
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
}