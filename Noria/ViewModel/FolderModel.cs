using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Noria.UI;
using Noria.Files;

namespace Noria.ViewModel
{
    public class FolderModel : INotifyPropertyChanged
    {
        private string _folderPath;
        private ObservableCollection<FolderItemModel> _items
            = new ObservableCollection<FolderItemModel>();

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

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            var e = new PropertyChangedEventArgs(propertyName);

            PropertyChanged?.Invoke(this, e);
        }

        public static FolderModel CreateFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                return null;

            var items = new List<FolderItemModel>();

            try
            {
                LoadFolderItems(folderPath, items);
                LoadFileItems(folderPath, items);

                var folder = new FolderModel()
                {
                    FolderPath = folderPath
                };

                items.ForEach(i => folder.Items.Add(i));

                return folder;
            }
            catch (UnauthorizedAccessException)
            {
                return new InaccessibleFolderModel()
                {
                    FolderPath = folderPath
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
    }
}
