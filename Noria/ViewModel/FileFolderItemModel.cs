using System.IO;
using Noria.Files;

namespace Noria.ViewModel
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
}