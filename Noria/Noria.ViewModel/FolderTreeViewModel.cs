using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Security.RightsManagement;
using System.Collections.ObjectModel;
using System.IO;

namespace Noria.ViewModel
{
    public class FolderTreeViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<FolderTreeItemModel> _rootFolders
            = new ObservableCollection<FolderTreeItemModel>();

        public ObservableCollection<FolderTreeItemModel> RootFolders
        {
            get { return _rootFolders; } 
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            var e = new PropertyChangedEventArgs(propertyName);

            PropertyChanged?.Invoke(this, e);
        }
    }

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
