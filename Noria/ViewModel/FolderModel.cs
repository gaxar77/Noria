using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Noria.UI;
using Noria.FilesAndFolders;
using System.Windows.Navigation;
using System.Windows.Controls.Primitives;
using System.Runtime.InteropServices;
using System.Security.RightsManagement;
using System.Security.Policy;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace Noria.ViewModel
{
    public class FolderModel : INotifyPropertyChanged, IFileSystemViewItem,
        IFileSystemViewSubItemsUpdatable, IFileSystemViewItemDeletable,
        IFileSystemViewItemUpdatable
    {
        private string _folderPath;
        private ObservableCollection<FolderItemModel> _items
            = new ObservableCollection<FolderItemModel>();
        
        public FolderViewModel FolderViewModel { get; private set; }
        public string FolderPath 
        {
            get { return _folderPath; }

            set
            {
                _folderPath = value;

                OnPropertyChanged(nameof(FolderPath));
            }
        }
        public ObservableCollection<FolderItemModel> Items
        {
            get { return _items; }
        }

        public string FileSystemItemPath => FolderPath;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            var e = new PropertyChangedEventArgs(propertyName);

            PropertyChanged?.Invoke(this, e);
        }

        public static FolderModel CreateFolder(string folderPath, FolderViewModel folderViewModel)
        {
            if (!Directory.Exists(folderPath))
                return new InaccessibleFolderModel()
                {
                    FolderPath = folderPath,
                    FolderViewModel = folderViewModel
                };

            var items = new List<FolderItemModel>();

            try
            {
                LoadFolderItems(folderPath, items);
                LoadFileItems(folderPath, items);

                var folder = new FolderModel()
                {
                    FolderPath = folderPath,
                    FolderViewModel = folderViewModel
                };

                items.ForEach(i => folder.Items.Add(i));

                return folder;
            }
            catch (UnauthorizedAccessException)
            {
                return new InaccessibleFolderModel()
                {
                    FolderPath = folderPath,
                    FolderViewModel = folderViewModel
                };
            }
        }

        private static void LoadFileItems(string folderPath, List<FolderItemModel> items)
        {
            var fileTypeUtility = new FileTypeUtility();
            var filePaths = Directory.GetFiles(folderPath);
            foreach (string filePath in filePaths)
            {
                var fileFolderItem = new FileFolderItemModel(filePath, fileTypeUtility);

                items.Add(fileFolderItem);
            }
        }

        private static void LoadFolderItems(string folderPath, List<FolderItemModel> items)
        {
            var subFolderPaths = Directory.GetDirectories(folderPath);

            foreach (string subFolderPath in subFolderPaths)
            {
                var folderFolderItem = new FolderFolderItemModel(subFolderPath);

                items.Add(folderFolderItem);
            }
        }

        public void AddItem(string itemPath)
        {
            var item = this.GetItemByPath(itemPath);

            if (item == null)
            {
                item = FolderItemModel.CreateFromPath(itemPath);
                _items.Add(item);
            }
        }

        public void DeleteItem(string itemPath)
        {
            var item = this.GetItemByPath(itemPath);

            if (item != null)
            {
                _items.Remove(item);
            }
        }

        public void Delete()
        {
            FolderViewModel.TryRefresh();
        }

        public void Update(string itemPath, string newItemPath)
        {
            if (newItemPath != null)
            {
                FolderViewModel.TryNavigate(newItemPath, false);
            }
        }
    }
}
