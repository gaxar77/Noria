using System.IO;

namespace Noria.ViewModel
{
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
}