using System.ComponentModel;

namespace Noria.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        FolderViewModel _folderViewModel;
        FolderTreeViewModel _folderTreeViewModel;
        public FolderViewModel FolderViewModel
        {
            get { return _folderViewModel; }

            set
            {
                _folderViewModel = value;

                OnPropertyChanged(nameof(FolderViewModel));
            }
               
        }

        public FolderTreeViewModel FolderTreeViewModel
        {
            get { return _folderTreeViewModel; }

            set
            {
                _folderTreeViewModel = value;

                OnPropertyChanged(nameof(FolderTreeViewModel));
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            var args = new PropertyChangedEventArgs(propertyName);

            PropertyChanged?.Invoke(this, args);
        }
    }
}
