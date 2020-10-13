using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Navigation;

namespace Noria.ViewModel
{
    public class FolderTreeItemModel : INotifyPropertyChanged, IFileSystemViewItem,
        IFileSystemViewItemUpdatable, IFileSystemViewSubItemsUpdatable
    {
        private string _folderName;
        private string _folderPath;
        private ObservableCollection<FolderTreeItemModel> _subFolders
            = new ObservableCollection<FolderTreeItemModel>();
        private bool _areSubFoldersLoaded;
        private int _subFolderCount;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool AreSubFoldersLoaded
        {
            get { return _areSubFoldersLoaded; }

            private set
            {
                _areSubFoldersLoaded = value;

                OnPropertyChanged(nameof(AreSubFoldersLoaded));
            }
        }
        public string FolderName
        {
            get { return _folderName; }

            set
            {
                _folderName = value;

                OnPropertyChanged(nameof(FolderName));
            }
        }

        public int SubFolderCount
        {
            get { return _subFolderCount; }

            private set
            {
                _subFolderCount = value;

                OnPropertyChanged(nameof(SubFolderCount));
                OnPropertyChanged(nameof(HasSubFolders));
            }
        }

        public bool HasSubFolders
        {
            get { return SubFolderCount > 0; }
        }

        public string FolderPath
        {
            get { return _folderPath; }

            set
            {
                _folderPath = value;

                OnPropertyChanged(nameof(FolderPath));
            }
        }

        public ObservableCollection<FolderTreeItemModel> SubFolders
        {
            get { return _subFolders; }
        }

        public string FileSystemItemPath => FolderPath;

        private void OnPropertyChanged(string propertyName)
        {
            var e = new PropertyChangedEventArgs(propertyName);

            PropertyChanged?.Invoke(this, e);
        }

        public FolderTreeItemModel(string folderPath)
        {
            Init(folderPath);
        }

        private void Init(string folderPath)
        {
            FolderPath = folderPath;
            FolderName = Path.GetFileName(folderPath);

            if (String.IsNullOrEmpty(FolderName))
                FolderName = FolderPath;

            SubFolders.Clear();

            AreSubFoldersLoaded = false;
        }

        public bool LoadSubFolders()
        {
            AreSubFoldersLoaded = false;
            SubFolders.Clear();

            try
            {
                var subFolderPaths = Directory.GetDirectories(FolderPath);

                foreach (string subFolderPath in subFolderPaths)
                {
                    var subFolder = new FolderTreeItemModel(subFolderPath);

                    SubFolders.Add(subFolder);
                }

                AreSubFoldersLoaded = true;
            }
            catch (Exception)
            {
            }

            return AreSubFoldersLoaded;
        }

        public void LoadSubFolderCount()
        {
            SubFolderCount = 0;

            try
            {
                SubFolderCount =
                    Directory.GetDirectories(FolderPath)
                        .Count();
            }
            catch (Exception)
            {
            }
        }
        public FolderTreeItemModel GetSubFolderByPath(string path)
        {
            return SubFolders.SingleOrDefault(sf => sf.FolderPath == path);
        }

        public void Update(string itemPath, string newItemPath)
        {
            if (newItemPath != null && itemPath == FileSystemItemPath)
            {
                Init(newItemPath); 
            }
        }

        public void AddItem(string itemPath)
        {
            if (!AreSubFoldersLoaded)
                return;

            if (GetSubFolderByPath(itemPath) == null)
            {
                var item = new FolderTreeItemModel(itemPath);

                SubFolders.Add(item);
            }
        }

        public void DeleteItem(string itemPath)
        {
            if (!AreSubFoldersLoaded)
                return;

            var item = GetSubFolderByPath(itemPath);

            if (item != null)
            {
                SubFolders.Remove(item);
            }
            
        }
    }
}
