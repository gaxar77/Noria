using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Security.RightsManagement;
using System.Collections.ObjectModel;

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
}
