using System.IO;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;

namespace Noria.ViewModel
{
    public class FolderViewModelUpdaterThroughWatchingFolder
    {
        private FolderViewModel _folderViewModel;
        private FolderModel _folder;
        private FileSystemWatcher _itemsWatcher;
        private FileSystemWatcher _folderWatcher;
        private bool _isDisposed;
        private object _syncRoot = new object();
        private SynchronizationContext _synchronizationContext;
        private const int PostDelay = 100;

        public FolderViewModelUpdaterThroughWatchingFolder(FolderViewModel folderViewModel,
            SynchronizationContext synchronizationContext)
        {
            _folderViewModel = folderViewModel;

            _folderViewModel.Navigated += _folderViewModel_Navigated;
            _synchronizationContext = synchronizationContext;

        }

        private void _folderViewModel_Navigated(object sender, FolderViewModelNavigationEventArgs e)
        {
            Folder = _folderViewModel.Folder;
        }

        private FolderModel Folder
        {
            get { return _folder; }

            set
            {
                _folder = value;

                lock (_syncRoot)
                {
                    if (_folder != null)
                    {
                        SetupFileSystemWatcher();
                    }
                    else
                    {
                        _itemsWatcher.Dispose();
                        _itemsWatcher = null;

                        _folderWatcher.Dispose();
                        _folderWatcher = null;
                    }
                }
            }
        }

        public void SetupFileSystemWatcher()
        {
            CreateItemsWatcher();
            CreateFolderWatcher();
        }

        private void AttachWatcherEvents(FileSystemWatcher watcher)
        {
            watcher.Created += _fileSystemWatcher_Created;
            watcher.Deleted += _fileSystemWatcher_Deleted;
            watcher.Changed += _fileSystemWatcher_Changed;
            watcher.Renamed += _fileSystemWatcher_Renamed;
            watcher.Error += Watcher_Error;
        }

        private void Watcher_Error(object sender, ErrorEventArgs e)
        {
            Task.Delay(PostDelay).ContinueWith((t) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _folderViewModel.TryRefresh();
                });
            });
        }

        private void CreateItemsWatcher()
        {
            if (_itemsWatcher != null)
                _itemsWatcher.Dispose();

            _itemsWatcher = new FileSystemWatcher(_folder.FolderPath, "*.*");
            _itemsWatcher.NotifyFilter = NotifyFilters.FileName |
                                              NotifyFilters.DirectoryName |
                                              NotifyFilters.LastWrite |
                                              NotifyFilters.Size;


            AttachWatcherEvents(_itemsWatcher);
            _itemsWatcher.EnableRaisingEvents = true;
        }

        private void CreateFolderWatcher()
        {
            if (_folderWatcher != null)
                _folderWatcher.Dispose();

            var parentPath = Directory.GetParent(_folder.FolderPath);

            if (parentPath != null && parentPath.FullName != Path.GetPathRoot(parentPath.FullName))
            {
                var folderName = Path.GetFileName(_folder.FolderPath);

                _folderWatcher = new FileSystemWatcher(parentPath.FullName, folderName);
                _folderWatcher.NotifyFilter = //NotifyFilters.FileName |
                                                  NotifyFilters.DirectoryName |
                                                  NotifyFilters.LastWrite |
                                                  NotifyFilters.Size;

                AttachWatcherEvents(_folderWatcher);
                _folderWatcher.EnableRaisingEvents = true;
            }
            else
            {
                _folderWatcher = null;
            }
        }

        private void _fileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            Task.Delay(PostDelay).ContinueWith((t) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (e.OldFullPath == _folder.FolderPath)
                    {
                        _folder.Items.Clear();

                        OnFolderInvalidated(e.FullPath);
                    }
                    else
                    {
                        ReplaceItem(e.OldFullPath, e.FullPath);
                    }
                });
            });
        }

        private void ReplaceItem(string oldPath, string newPath)
        {
            Task.Delay(PostDelay).ContinueWith((t) =>
            {
                _synchronizationContext.Post((s) =>
                {
                    var oldItem = _folder.GetItemByPath(oldPath);
                    if (oldItem != null)
                    {
                        _folder.Items.Remove(oldItem);

                        var newItem = FolderItemModel.CreateFromPath(newPath);
                        _folder.Items.Add(newItem);
                    }
                }, null);
            });
        }

        private void _fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            Task.Delay(PostDelay).ContinueWith((t) =>
            {
                _synchronizationContext.Post((s) =>
                {
                    if (e.FullPath != _folder.FolderPath)
                    {
                        var item = _folder.GetItemByPath(e.FullPath);

                        if (item != null)
                        {
                            item.Load();
                        }
                        else
                        {
                            item = FolderItemModel.CreateFromPath(e.FullPath);
                            _folder.Items.Add(item);
                        }
                    }
                }, null);
            });
        }

        private void _fileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            Task.Delay(PostDelay).ContinueWith((t) =>
            {
                _synchronizationContext.Post((s) =>
                {
                    if (e.FullPath == _folder.FolderPath)
                    {
                        _folder.Items.Clear();

                        OnFolderInvalidated(null);
                    }
                    else
                    {
                        var item = _folder.GetItemByPath(e.FullPath);
                        if (item != null)
                        {
                            _folder.Items.Remove(item);
                        }
                    }
                }, null);
            });
        }

        private void _fileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            Task.Delay(PostDelay).ContinueWith((t) =>
            {
                _synchronizationContext.Post((s) =>
                {
                    var item = _folder.GetItemByPath(e.FullPath);
                    if (item != null)
                    {
                        _folder.Items.Remove(item);
                    }

                    item = FolderItemModel.CreateFromPath(e.FullPath);
                    _folder.Items.Add(item);
                }, null);
            });
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                Folder = null;
                _isDisposed = true;
            }
        }

        public void OnFolderInvalidated(string newFolderPath)
        {
            if (newFolderPath != null)
            {
                _folderViewModel.TryNavigate(newFolderPath);
            }
            else
            {
                _folderViewModel.TryRefresh();
            }
        }
    }
}
