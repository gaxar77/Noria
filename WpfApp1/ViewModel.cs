using System.ComponentModel;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.IO;

namespace WpfApp1
{
    public class ViewModel : INotifyPropertyChanged
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
        
        private void OnDirectoryPathChanged(string oldValue)
        {
            TryNavigate(DirectoryPath);
        }

        private void OnPropertyChanged(string propertyName)
        {
            var args = new PropertyChangedEventArgs(propertyName);

            PropertyChanged?.Invoke(this, args);
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

    public class ImmutableFolderItem
    {
        public string ItemName { get; private set; }
        public string ItemPath { get; private set; }
        public DateTime DateModified { get; private set; }
        public long? SizeInBytes { get; private set; }

        public string FileType { get; private set; }
        public ImmutableFolderItemType ItemType { get; private set; }

        public static ImmutableFolderItem CreateFolderFolderItem(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                return null;

            var info = new DirectoryInfo(folderPath);

            var newItem = new ImmutableFolderItem()
            {
                ItemPath = Path.GetFullPath(folderPath),
                ItemName = Path.GetFileName(folderPath),
                ItemType = ImmutableFolderItemType.Folder,
                SizeInBytes = null,
                DateModified = info.LastWriteTime,
                FileType = "Folder"
            };

            return newItem;
        }

        public static ImmutableFolderItem CreateFileFolderItem(string filePath,
            FileTypeUtility fileTypeUtility)
        {
            if (!File.Exists(filePath))
                return null;

            var info = new FileInfo(filePath);

            var newItem = new ImmutableFolderItem()
            {
                ItemPath = Path.GetFullPath(filePath),
                ItemName = Path.GetFileName(filePath),
                ItemType = ImmutableFolderItemType.File,
                SizeInBytes = info.Length,
                DateModified = info.LastWriteTime,
                FileType = fileTypeUtility.GetFileType(filePath)
            };

            return newItem;
        }
    }

    public class InaccessableImmutableFolder : ImmutableFolder
    {
    }

    public class ImmutableFolder
    {
        public string FolderPath { get; private set; }
        public ReadOnlyCollection<ImmutableFolderItem> Items { get; private set; }

        public static ImmutableFolder CreateFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                return null;

            var items = new List<ImmutableFolderItem>();

            try
            {
                AddFolderFolderItems(folderPath, items);
                AddFileFolderItems(folderPath, items);

                var folder = new ImmutableFolder()
                {
                    Items = new ReadOnlyCollection<ImmutableFolderItem>(items),
                    FolderPath = folderPath
                };

                return folder;
            }
            catch (UnauthorizedAccessException)
            {
                return new InaccessableImmutableFolder()
                {
                    FolderPath = folderPath
                };
            }
        }

        private static void AddFileFolderItems(string folderPath, List<ImmutableFolderItem> items)
        {
            var fileTypeUtility = new FileTypeUtility();
            var filePaths = Directory.GetFiles(folderPath);
            foreach (string filePath in filePaths)
            {
                var fileFolderItem = ImmutableFolderItem.CreateFileFolderItem(filePath,
                    fileTypeUtility);

                items.Add(fileFolderItem);
            }
        }

        private static void AddFolderFolderItems(string folderPath, List<ImmutableFolderItem> items)
        {
            var subFolderPaths = Directory.GetDirectories(folderPath);

            foreach (string subFolderPath in subFolderPaths)
            {
                var folderFolderItem = ImmutableFolderItem.CreateFolderFolderItem(subFolderPath);

                items.Add(folderFolderItem);
            }
        }
    }
}
