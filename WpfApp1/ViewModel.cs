using System.ComponentModel;
using System.Collections.ObjectModel;

namespace WpfApp1
{
    public class ViewModel : INotifyPropertyChanged
    {
        private string _directoryPath;

        private ObservableCollection<FolderViewItemModel> _folderViewItems;

        public ObservableCollection<FolderViewItemModel> FolderViewItems
        {
            get { return _folderViewItems; }
        }

        public string DirectoryPath
        {
            get { return _directoryPath; }

            set
            {
                _directoryPath = value;

                OnPropertyChanged(nameof(DirectoryPath));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            var args = new PropertyChangedEventArgs(propertyName);

            PropertyChanged?.Invoke(this, args);
        }
        public ViewModel()
        {
            _folderViewItems = new ObservableCollection<FolderViewItemModel>();
        }
    }
}
