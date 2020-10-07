using System.IO;

namespace WpfApp1
{
    public class ViewModelHelper
    {
        public static void LoadFolder(ViewModel viewModel, string directoryPath)
        {
            viewModel.DirectoryPath = directoryPath;

            viewModel.FolderViewItems.Clear();

            LoadSubDirectories(viewModel, directoryPath);
            LoadFiles(viewModel, directoryPath);
        }
        public static void LoadParentFolder(ViewModel viewModel)
        {
            string parentDirectory = Directory.GetParent(viewModel.DirectoryPath).FullName;

            ViewModelHelper.LoadFolder(viewModel, parentDirectory);
        }
        private static void LoadFiles(ViewModel viewModel, string directoryPath)
        {
            var files = Directory.GetFiles(directoryPath);
            var fileTypeUtility = new FileTypeUtility();

            foreach (string file in files)
            {
                var item = new FolderViewItemModel()
                {
                    FilePath = file,
                    FileName = Path.GetFileName(file),
                    Created = File.GetCreationTime(file),
                    Modified = File.GetLastWriteTime(file),
                    LastAccessed = File.GetLastAccessTime(file),
                    Size = FileUtility.GetFileSize(file),
                    IsFolder = false,
                    FileType = fileTypeUtility.GetFileType(file)
                };

                viewModel.FolderViewItems.Add(item);
            }
        }

        private static void LoadSubDirectories(ViewModel viewModel, string directoryPath)
        {
            var subDirectories = Directory.GetDirectories(directoryPath);
            var directoryInfo = new DirectoryInfo(directoryPath);

            foreach (string subDirectory in subDirectories)
            {
                var item = new FolderViewItemModel()
                {
                    FilePath = subDirectory,
                    FileName = Path.GetFileName(subDirectory),
                    Created = Directory.GetCreationTime(subDirectory),
                    Modified = Directory.GetLastWriteTime(subDirectory),
                    LastAccessed = Directory.GetLastAccessTime(subDirectory),
                    Size = null,
                    IsFolder = true
                };

                viewModel.FolderViewItems.Add(item);
            }
        }
    }
}
