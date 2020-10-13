using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Shapes = System.Windows.Shapes;
using System.IO;
using System.CodeDom;
using Noria.FilesAndFolders;

namespace Noria.UI
{
    //Todo: Add SubFolder Drop Downs
    //Todo: Add Horizontal Scrolling
    //Todo: Change events to dependency events.

    /// <summary>
    /// Interaction logic for DirectoryPathBreadCrumb.xaml
    /// </summary>
    public partial class DirectoryPathBreadCrumb : UserControl, INotifyPropertyChanged
    {
        public DirectoryPathBreadCrumb()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty DirectoryPathProperty =
            DependencyProperty.Register("DirectoryPath", typeof(string),
                typeof(DirectoryPathBreadCrumb),
                new PropertyMetadata("", OnDirectoryPathChanged));

        public string DirectoryPath
        {
            get 
            { 
                return (string)GetValue(DirectoryPathProperty);
            }

            set
            {
                SetValue(DirectoryPathProperty, value);
            }
        }

        private static void OnDirectoryPathChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((DirectoryPathBreadCrumb)obj).OnDirectoryPathChanged(e);
        }

        private void OnDirectoryPathChanged(DependencyPropertyChangedEventArgs e)
        {
            CreateBreadCrumbButtons();
        }

        public event EventHandler<BreadCrumbPathSelectedEventArgs> BreadCrumbPathSelected;

        private FileSystemPath _path;
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler MainPanelMouseDown;

        private void InvokePropertyChanged(string propertyName)
        {
            var args = new PropertyChangedEventArgs(propertyName);

            PropertyChanged?.Invoke(this, args);
        }

        public void CreateBreadCrumbButtons()
        {
            mainPanel.Children.Clear();
            _path = new FileSystemPath(DirectoryPath);

            foreach (FileSystemPathComponent pathComponent in _path.PathComponents)
            {
                var breadCrumbButton = new Button()
                {
                    Content = pathComponent.FileSystemObjectName,
                    Padding = new Thickness(5, 5, 5, 5),
                    Margin = new Thickness(0, 0, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                breadCrumbButton.Click += (o, e) =>
                    {
                        var args = new BreadCrumbPathSelectedEventArgs(pathComponent.Path);

                        BreadCrumbPathSelected?.Invoke(this, args);
                    };

                mainPanel.Children.Add(breadCrumbButton);
            }
        }

        private void mainPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainPanelMouseDown?.Invoke(this, new EventArgs());
        }
    }
   
    public class BreadCrumbPathSelectedEventArgs
    {
        public string DirectoryPath { get; private set; }
        public BreadCrumbPathSelectedEventArgs(string path)
        {
            DirectoryPath = path;
        }
    }
}
