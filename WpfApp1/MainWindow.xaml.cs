using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
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
        ViewModel _viewModel = new ViewModel();

        public MainWindow()
        {
            InitializeComponent();

            DataContext = _viewModel;

            ViewModelHelper.LoadFolder(_viewModel, @"C:\");
        }
        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModelHelper.LoadFolder(_viewModel, txtPath.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot navigate to directory.");
            }
        }
        private void dgrdFolderViewRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenFolderViewItem();
        }

        private void OpenFolderViewItem()
        {
            try
            {
                FolderViewItemModel item = dgrdFolderView.SelectedItem as FolderViewItemModel;

                if (item != null)
                {
                    if (item.IsFolder)
                    {
                        ViewModelHelper.LoadFolder(_viewModel, item.FilePath);
                    }
                    else
                    {
                        Process.Start(item.FilePath);
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot navigate to directory.");
                try
                {
                    ViewModelHelper.LoadParentFolder(_viewModel);
                    ScrollToTopOfFolderView();
                }
                catch (Exception)
                {

                }
            }
            finally
            {
                
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
            try
            {
                ViewModelHelper.LoadParentFolder(_viewModel);
                ScrollToTopOfFolderView();
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot navigate to directory.");
            }
        }

        private void dgrdFolderView_GotFocus(object sender, RoutedEventArgs e)
        {
            txtPath.InvalidateProperty(TextBox.TextProperty);
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

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            ShowFeatureNotImplementedMessageBox();
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
    }
}
