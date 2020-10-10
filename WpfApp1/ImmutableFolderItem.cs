using System;
using System.IO;

namespace WpfApp1
{
    public class ImmutableFolderItem
    {
        public string ItemName { get; private set; }
        public string ItemPath { get; private set; }
        public DateTime DateModified { get; private set; }
        public long? SizeInBytes { get; private set; }

        public string FileType { get; private set; }
        public ImmutableFolderItemType ItemType { get; private set; }

        public static ImmutableFolderItem CreateFolderFolderItem(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                return null;

            var info = new DirectoryInfo(folderPath);

            var newItem = new ImmutableFolderItem()
            {
                ItemPath = Path.GetFullPath(folderPath),
                ItemName = Path.GetFileName(folderPath),
                ItemType = ImmutableFolderItemType.Folder,
                SizeInBytes = null,
                DateModified = info.LastWriteTime,
                FileType = "Folder"
            };

            return newItem;
        }

        public static ImmutableFolderItem CreateFileFolderItem(string filePath,
            FileTypeUtility fileTypeUtility)
        {
            if (!File.Exists(filePath))
                return null;

            var info = new FileInfo(filePath);

            var newItem = new ImmutableFolderItem()
            {
                ItemPath = Path.GetFullPath(filePath),
                ItemName = Path.GetFileName(filePath),
                ItemType = ImmutableFolderItemType.File,
                SizeInBytes = info.Length,
                DateModified = info.LastWriteTime,
                FileType = fileTypeUtility.GetFileType(filePath)
            };

            return newItem;
        }
    }
}
