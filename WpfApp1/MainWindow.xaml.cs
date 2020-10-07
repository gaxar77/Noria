using System;
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
            try
            {
                FolderViewItemModel item = dgrdFolderView.SelectedItem as FolderViewItemModel;

                if (item != null)
                {
                    ViewModelHelper.LoadFolder(_viewModel, item.FilePath);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot navigate to directory.");
            }
        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModelHelper.LoadParentFolder(_viewModel);
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
    }
}
