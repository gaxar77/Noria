using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Navigation;

namespace Noria.ViewModel
{
    public class FolderTreeItemModel : INotifyPropertyChanged, IFileSystemItem,
        IFileSystemItemUpdatable, IFileSystemSubItemsUpdatable
    {
        private string _folderName;
        private string _folderPath;
        private ObservableCollection<FolderTreeItemModel> _subFolders
            = new ObservableCollection<FolderTreeItemModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        public string FolderName
        {
            get { return _folderName; }

            set
            {
                _folderName = value;

                OnPropertyChanged(nameof(FolderName));
            }
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
        }

        public bool LoadSubFolders()
        {
            SubFolders.Clear();

            try
            {
                var subFolderPaths = Directory.GetDirectories(FolderPath);

                foreach (string subFolderPath in subFolderPaths)
                {
                    var subFolder = new FolderTreeItemModel(subFolderPath);

                    SubFolders.Add(subFolder);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
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
            if (GetSubFolderByPath(itemPath) == null)
            {
                var item = new FolderTreeItemModel(itemPath);

                SubFolders.Add(item);
            }
        }

        public void DeleteItem(string itemPath)
        {
            var item = GetSubFolderByPath(itemPath);

            if (item != null)
            {
                SubFolders.Remove(item);
            }
        }
    }
}
