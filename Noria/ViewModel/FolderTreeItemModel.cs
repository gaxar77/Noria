using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;

namespace Noria.ViewModel
{
    public class FolderTreeItemModel : INotifyPropertyChanged
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
        private void OnPropertyChanged(string propertyName)
        {
            var e = new PropertyChangedEventArgs(propertyName);

            PropertyChanged?.Invoke(this, e);
        }

        public FolderTreeItemModel(string folderPath)
        {
            FolderPath = folderPath;
            FolderName = Path.GetFileName(folderPath);

            if (String.IsNullOrEmpty(FolderName))
                FolderName = FolderPath;
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
    }
}
