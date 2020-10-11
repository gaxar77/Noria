﻿using System;
using System.ComponentModel;
using System.Security.RightsManagement;
using System.Runtime.CompilerServices;
using System.Windows.Markup;

namespace Noria.ViewModel
{
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