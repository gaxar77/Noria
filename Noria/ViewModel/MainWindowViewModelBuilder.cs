using System;
using System.Threading;

namespace Noria.ViewModel
{
    public class MainWindowViewModelBuilder
    {
        public MainWindowViewModel ViewModel { get; private set; }
        public MainWindowViewModelBuilder()
        {
            ViewModel = new MainWindowViewModel();
        }

        public void BuildFolderViewModel()
        {
            var myDocumentsFolderPath = Environment.GetFolderPath(
                Environment.SpecialFolder.MyDocuments);

            ViewModel.FolderViewModel = new FolderViewModel()
            {
                DirectoryPath = myDocumentsFolderPath
            };
        }

        public void BuildFolderTreeViewModel()
        {
            var drivePaths = Environment.GetLogicalDrives();

            ViewModel.FolderTreeViewModel = new FolderTreeViewModel();

            foreach (string drivePath in drivePaths)
            {
                var driveFolder = new FolderTreeItemModel(drivePath);
                ViewModel.FolderTreeViewModel.RootFolders.Add(driveFolder);
            }
        }

        public void BuildAll()
        {
            BuildFolderTreeViewModel();
            BuildFolderViewModel();
        }
    }
}
