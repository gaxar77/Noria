using System;
using System.IO;
using System.ComponentModel;
using System.Security.RightsManagement;
using System.Runtime.CompilerServices;
using System.Windows.Markup;

namespace WpfApp1
{
    public class FileFolderItemModel : FolderItemModel
    {
        public long? _sizeInBytes;
        public FileTypeUtility _fileTypeUtility;

        public long? SizeInBytes
        {
            get { return _sizeInBytes; }

            set
            {
                _sizeInBytes = value;

                OnPropertyChanged(nameof(SizeInBytes));
            }
        }

        public FileTypeUtility FileTypeUtility
        {
            get { return _fileTypeUtility; }

            set
            {
                _fileTypeUtility = value;

                OnPropertyChanged(nameof(FileTypeUtility));
            }
        }

        public FileFolderItemModel(string filePath, FileTypeUtility fileTypeUtility)
        {
            ItemPath = filePath;
            FileTypeUtility = fileTypeUtility;

            Load();
        }

        public override void Load()
        {
            IsLoaded = false;

            var fileInfo = new FileInfo(ItemPath);

            if (fileInfo.Exists)
            {
                ItemName = Path.GetFileName(fileInfo.FullName);
                SizeInBytes = fileInfo.Length;
                DateModified = fileInfo.LastWriteTime;
                FileType = FileTypeUtility.GetFileType(ItemName);
                IsLoaded = true;
            }
        }
    }
    public class FolderFolderItemModel : FolderItemModel
    {
        public FolderFolderItemModel(string folderPath)
        {
            ItemPath = folderPath;
            
            Load();
        }

        public override void Load()
        {
            IsLoaded = false;

            var folderInfo = new DirectoryInfo(ItemPath);

            if (folderInfo.Exists)
            {
                ItemName = Path.GetFileName(folderInfo.FullName);
                FileType = "Folder";
                DateModified = folderInfo.LastWriteTime;
                IsLoaded = true;
            }
        }
    }
    public abstract class FolderItemModel : INotifyPropertyChanged
    {
        bool _isLoaded;
        public string _itemName;
        public string _itemPath;
        public string _fileType;
        public DateTime _dateModified;

        public bool IsLoaded
        {
            get { return _isLoaded; }

            set
            {
                _isLoaded = value;

                OnPropertyChanged(nameof(IsLoaded));
            }
        }
        public string ItemName
        {
            get { return _itemName; }

            set
            {
                _itemName = value;

                OnPropertyChanged(nameof(ItemName));
            }
        }

        public string ItemPath
        {
            get { return _itemPath; }

            set
            {
                _itemPath = value;

                OnPropertyChanged(nameof(ItemName));
            }
        }

        public string FileType
        {
            get { return _fileType; }

            set
            {
                _fileType = value;

                OnPropertyChanged(nameof(FileType));
            }
        }  

        public DateTime DateModified
        {
            get { return _dateModified; }

            set
            {
                _dateModified = value;

                OnPropertyChanged(nameof(DateModified));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            var e = new PropertyChangedEventArgs(propertyName);

            PropertyChanged?.Invoke(this, e);
        }

        public abstract void Load();
    }
}