using System;

namespace Noria.ViewModel
{
    public class FolderViewModelNavigationEventArgs : EventArgs
    {
        public FolderModel PreviousFolder { get; private set; }
        public FolderModel NewFolder { get; private set; }

        public FolderViewModelNavigationEventArgs(FolderModel previousFolder, FolderModel newFolder)
        {
            PreviousFolder = previousFolder;
            NewFolder = newFolder;
        }
    }
}
