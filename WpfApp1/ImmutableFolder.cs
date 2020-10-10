using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.IO;

namespace WpfApp1
{
    public class ImmutableFolder
    {
        public string FolderPath { get; private set; }
        public ReadOnlyCollection<ImmutableFolderItem> Items { get; private set; }

        public static ImmutableFolder CreateFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                return null;

            var items = new List<ImmutableFolderItem>();

            try
            {
                AddFolderFolderItems(folderPath, items);
                AddFileFolderItems(folderPath, items);

                var folder = new ImmutableFolder()
                {
                    Items = new ReadOnlyCollection<ImmutableFolderItem>(items),
                    FolderPath = folderPath
                };

                return folder;
            }
            catch (UnauthorizedAccessException)
            {
                return new InaccessableImmutableFolder()
                {
                    FolderPath = folderPath
                };
            }
        }

        private static void AddFileFolderItems(string folderPath, List<ImmutableFolderItem> items)
        {
            var fileTypeUtility = new FileTypeUtility();
            var filePaths = Directory.GetFiles(folderPath);
            foreach (string filePath in filePaths)
            {
                var fileFolderItem = ImmutableFolderItem.CreateFileFolderItem(filePath,
                    fileTypeUtility);

                items.Add(fileFolderItem);
            }
        }

        private static void AddFolderFolderItems(string folderPath, List<ImmutableFolderItem> items)
        {
            var subFolderPaths = Directory.GetDirectories(folderPath);

            foreach (string subFolderPath in subFolderPaths)
            {
                var folderFolderItem = ImmutableFolderItem.CreateFolderFolderItem(subFolderPath);

                items.Add(folderFolderItem);
            }
        }
    }
}
