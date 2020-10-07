using System;
using System.ComponentModel;

namespace WpfApp1
{
    public class FolderViewItemModel : INotifyPropertyChanged
    {
        private string _fileName;
        private string _filePath;
        private string _fileType;
        private DateTime _modified;
        private DateTime _created;
        private DateTime _lastAccessed;
        private long? _size;
        private bool _isFolder;

        public string FilePath
        {
            get { return _filePath; }

            set
            {
                _filePath = value;

                OnPropertyChanged(nameof(FilePath));
            }
        }
        public string FileName
        {
            get { return _fileName; }

            set
            {
                _fileName = value;

                OnPropertyChanged(nameof(FileName));
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

        public long? Size
        {
            get { return _size; }

            set
            {
                _size = value;

                OnPropertyChanged(nameof(Size));
            }
        }

        public DateTime Modified
        {
            get { return _modified; }

            set
            {
                _modified = value;

                OnPropertyChanged(nameof(Modified));
            }
        }

        public DateTime Created
        {
            get { return _created; }

            set
            {
                _created = value;

                OnPropertyChanged(nameof(Created));
            }
        }

        public DateTime LastAccessed
        {
            get { return _lastAccessed; }

            set
            {
                _lastAccessed = value;

                OnPropertyChanged(nameof(LastAccessed));
            }
        }

        public bool IsFolder
        {
            get { return _isFolder; }

            set
            {
                _isFolder = value;

                OnPropertyChanged(nameof(IsFolder));
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
