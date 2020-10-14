using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace Noria.ViewModel
{
    public class GlobalFileSystemWatcherAdapter : IDisposable
    {
        private static GlobalFileSystemWatcherAdapter _instance;

        public static GlobalFileSystemWatcherAdapter Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GlobalFileSystemWatcherAdapter(
                        SynchronizationContext.Current);
                }

                return _instance;
            }
        }

        private object _syncRoot = new object();
        private List<FileSystemWatcher> _fileSystemWatchers
            = new List<FileSystemWatcher>();

        private List<IFileSystemViewItemProvider> _providers;
        private SynchronizationContext _syncContext;

        private bool _isDisposed;

        public List<IFileSystemViewItemProvider> FileSystemItemProviders
        {
            get { return _providers; }
        }
            
        public GlobalFileSystemWatcherAdapter(SynchronizationContext syncContext)
        {
            _providers = new List<IFileSystemViewItemProvider>();
            _syncContext = syncContext;

            CreateFileSystemWatchers();
        }

        public void CreateFileSystemWatchers()
        {
            var drivePaths = Environment.GetLogicalDrives();

            foreach (string drivePath in drivePaths)
            {
                try
                {
                    CreateFileSystemWatcher(drivePath);
                }
                catch (Exception)
                {
                }
            }
        }

        //Untested Code
        public bool HasFileSystemWatcher(string path)
        {
            var drivePath = Path.GetPathRoot(path);

            var hasFileSystemWatcher =
                _fileSystemWatchers.Any(fsw => 
                    String.Compare(fsw.Path, drivePath, true) == 0);

            return hasFileSystemWatcher;
        }

        public void CreateFileSystemWatcher(string path)
        {
            var fileSystemWatcher = new FileSystemWatcher(
                path,
                "*.*");

            fileSystemWatcher.NotifyFilter = NotifyFilters.DirectoryName |
                                             NotifyFilters.FileName |
                                             NotifyFilters.Size |
                                             NotifyFilters.LastAccess;

            fileSystemWatcher.Created += FileSystemWatcher_Created;
            fileSystemWatcher.Deleted += FileSystemWatcher_Deleted;
            fileSystemWatcher.Renamed += FileSystemWatcher_Renamed;
            fileSystemWatcher.Changed += FileSystemWatcher_Changed;
            
            fileSystemWatcher.IncludeSubdirectories = true;

            fileSystemWatcher.EnableRaisingEvents = true;

            _fileSystemWatchers.Add(fileSystemWatcher);
        }

        public void DestoryFileSystemWatchers()
        {
            lock (_syncRoot)
            {
                foreach (FileSystemWatcher watcher in _fileSystemWatchers)
                {
                    watcher.Dispose();
                }

                _fileSystemWatchers.Clear();
            }
        }

        private string GetParentDirectoryPath(string directoryPath)
        {
            var pathRoot = Path.GetPathRoot(directoryPath);
            var parentDir = Path.GetDirectoryName(directoryPath);

            if (parentDir == null && pathRoot != null)
            {
                parentDir = pathRoot;
            }

            return parentDir;
        }

        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            _syncContext.Post((s) =>
            {
                lock (_syncRoot)
                {
                    UpdateItem(e.FullPath, null);
                }

            }, null);
        }

        private void UpdateItem(string oldPath, string newPath)
        {
            foreach (IFileSystemViewItemProvider provider in _providers)
            {
                var item = provider.GetFileSystemViewItem(oldPath);

                var updatableItem = item as IFileSystemViewItemUpdatable;

                if (updatableItem != null)
                    updatableItem.Update(oldPath, newPath);
            }
        }

        private void FileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            _syncContext.Post((s) =>
            {
                lock (_syncRoot)
                {
                    UpdateItem(e.OldFullPath, e.FullPath);
                }
            }, null);
        }

        private void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            _syncContext.Post((s) =>
            {
                lock (_syncRoot)
                {
                    foreach (IFileSystemViewItemProvider provider in _providers)
                    {
                        var item = provider.GetFileSystemViewItem(e.FullPath);
                        var deletableItem = item as IFileSystemViewItemDeletable;

                        if (deletableItem != null)
                            deletableItem.Delete();

                        var parentDir = GetParentDirectoryPath(e.FullPath);
                        item = provider.GetFileSystemViewItem(parentDir);

                        var subItemUpdatable = item as IFileSystemViewSubItemsUpdatable;
                        if (subItemUpdatable != null)
                        {
                            subItemUpdatable.DeleteItem(e.FullPath);
                        }
                    }
                }
            }, null);
        }

        private void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            _syncContext.Post((s) =>
            {
                lock (_syncRoot)
                {
                    foreach (IFileSystemViewItemProvider provider in _providers)
                    {
                        var parentDir = GetParentDirectoryPath(e.FullPath);
                        var subItemUpdatable = provider.GetFileSystemViewItem(parentDir)
                            as IFileSystemViewSubItemsUpdatable;

                        if (subItemUpdatable != null)
                            subItemUpdatable.AddItem(e.FullPath);
                    }
                }
            }, null);
        }

        public void Dispose()
        {
            lock (_syncRoot)
            {
                if (!_isDisposed)
                {
                    DestoryFileSystemWatchers();
                    _isDisposed = true;
                }
            }
        }
    }
}