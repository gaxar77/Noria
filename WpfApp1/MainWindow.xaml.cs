using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Shapes = System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        enum OpenFolderFailureAction
        {
            ReverseNavigation,
            //ReverseNavigationShowErrorOnFailure,
            //ShowError,
            None
        }

        ViewModel _viewModel = new ViewModel();

        public MainWindow()
        {
            InitializeComponent();

            DataContext = _viewModel;

            ViewModelHelper.LoadFolder(_viewModel, @"C:\");
        }
        private void dgrdFolderViewRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            OpenFolderViewItem();
        }

        private bool TryOpenFolder(string directoryPath, bool scrollToTopOnSuccess, OpenFolderFailureAction failureAction)
        {
            string oldDirectoryPath = _viewModel.DirectoryPath;
            try
            {
                ViewModelHelper.LoadFolder(_viewModel, directoryPath);
                if (scrollToTopOnSuccess)
                    ScrollToTopOfFolderView();
            }
            catch(Exception)
            {
                switch(failureAction)
                {
                    case OpenFolderFailureAction.ReverseNavigation:
                            TryOpenFolder(oldDirectoryPath, false, OpenFolderFailureAction.None);
                        break;
                }

                return false;
            }

            return true;
        }

        private void OpenFolderViewItem()
        {
            FolderViewItemModel item = dgrdFolderView.SelectedItem as FolderViewItemModel;

            if (item != null)
            {
                if (item.IsFolder)
                {
                    if (!TryOpenFolder(item.FilePath, true, OpenFolderFailureAction.ReverseNavigation))
                    {
                        MessageBox.Show($"Unable to open folder \"{item.FilePath}\"");
                    }
                }
                else
                {
                    Process.Start(item.FilePath);
                }
            }
        }

        private void ScrollToTopOfFolderView()
        {
            var topItem = _viewModel.FolderViewItems.FirstOrDefault();

            if (topItem != null)
            {
                dgrdFolderView.ScrollIntoView(topItem);
            }
        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            var parentDirectory = Directory.GetParent(_viewModel.DirectoryPath);

            if (parentDirectory != null)
            {
                TryOpenFolder(parentDirectory.FullName, true, OpenFolderFailureAction.None);
            }
        }

        private void dgrdFolderView_GotFocus(object sender, RoutedEventArgs e)
        {
        }

        private void dgrdFolderView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void ShowFeatureNotImplementedMessageBox()
        {
            MessageBox.Show("Feature not yet implemented.");
        }
        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderViewItem();
        }

        //Todo: Refactor
        private void btnRename_Click(object sender, RoutedEventArgs e)
        {
            if (dgrdFolderView.SelectedItem is FolderViewItemModel item)
            {
                var renameFileDialog = new RenameFileDialog();
                renameFileDialog.ItemPath = item.FilePath;
                renameFileDialog.ItemOldName = item.FileName;
                renameFileDialog.ItemNewName = item.FileName;

                renameFileDialog.ShowDialog();

                if (renameFileDialog.UserConfirmedRename)
                {
                    try
                    {
                        var parentDir = Path.GetDirectoryName(item.FilePath);
                        var newItemPath = Path.Combine(parentDir, renameFileDialog.ItemNewName);
                        if (item.IsFolder)
                        {
                            Directory.Move(item.FilePath, newItemPath);
                        }
                        else
                        {
                            File.Move(item.FilePath, newItemPath);
                        }

                        try
                        {
                            ViewModelHelper.LoadFolder(_viewModel, _viewModel.DirectoryPath);
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Unable to refresh view.");
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Unable to rename item.");
                    }
                }
            }
        }

        //Todo: Implement Recycle Bin Storage
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgrdFolderView.SelectedItem is FolderViewItemModel item)
            {
                var result = MessageBox.Show("Are you sure you want to delete this item? You will not be able to recover it.",
                    null, MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        if (item.IsFolder)
                        {
                            Directory.Delete(item.FilePath, true);
                        }
                        else
                        {
                            File.Delete(item.FilePath);
                        }

                        _viewModel.FolderViewItems.Remove(item);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Unable to delete item.");
                    }
                }
            }
        }

        private void btnCut_Click(object sender, RoutedEventArgs e)
        {
            ShowFeatureNotImplementedMessageBox();
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            ShowFeatureNotImplementedMessageBox();
        }

        private void btnPaste_Click(object sender, RoutedEventArgs e)
        {
            ShowFeatureNotImplementedMessageBox();
        }

        private void btnProperties_Click(object sender, RoutedEventArgs e)
        {
            ShowFeatureNotImplementedMessageBox();
        }

        //Todo: Add Path validation
        private void txtPath_LostFocus(object sender, RoutedEventArgs e)
        {
            txtPath.InvalidateProperty(TextBox.TextProperty);
            dirPathBreadCrumb.Visibility = Visibility.Visible;
        }

        private void txtPath_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TryOpenFolder(txtPath.Text, true, OpenFolderFailureAction.ReverseNavigation);

                dgrdFolderView.Focus();
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            ShowFeatureNotImplementedMessageBox();
        }

        private void btnForward_Click(object sender, RoutedEventArgs e)
        {
            ShowFeatureNotImplementedMessageBox();
        }

        private void DirectoryPathBreadCrumb_BreadCrumbPathSelected(object sender, BreadCrumbPathSelected e)
        {
            TryOpenFolder(e.PathComponent.DirectoryPath, true, OpenFolderFailureAction.None);
        }

        private void dirPathBreadCrumb_MainPanelMouseDown(object sender, EventArgs e)
        {
            dirPathBreadCrumb.Visibility = Visibility.Collapsed;
        }
    }
}
