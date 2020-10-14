using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Security.RightsManagement;
using System.Collections.ObjectModel;
using System.Threading;

namespace Noria.ViewModel
{
    public class FolderTreeViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<FolderTreeItemModel> _rootFolders
            = new ObservableCollection<FolderTreeItemModel>();

        private GlobalFileSystemWatcherAdapter _fileSystemWatcher;

        public ObservableCollection<FolderTreeItemModel> RootFolders
        {
            get { return _rootFolders; } 
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public FolderTreeViewModel()
        {
            var fileSystemItemProvider =
                new FolderTreeViewModelFileSystemViewItemProvider(this);

            GlobalFileSystemWatcherAdapter
                .Instance
                .FileSystemItemProviders
                .Add(fileSystemItemProvider);
        }
        private void OnPropertyChanged(string propertyName)
        {
            var e = new PropertyChangedEventArgs(propertyName);

            PropertyChanged?.Invoke(this, e);
        }


    }
}
