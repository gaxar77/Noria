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
using System.Windows.Shapes;

namespace Noria.UI
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class RenameFileDialog : Window, INotifyPropertyChanged
    {
        private string _itemPath;
        private string _itemNewName;
        private string _itemOldName;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool UserConfirmedRename { get; private set; }

        public string ItemPath
        {
            get { return _itemPath; }

            set
            {
                _itemPath = value;

                InvokePropertyChanged(nameof(ItemPath));
            }
        }
        public string ItemNewName
        {
            get { return _itemNewName; }

            set
            {
                _itemNewName = value;

                InvokePropertyChanged(nameof(ItemNewName));
            }
        }

        public string ItemOldName
        {
            get { return _itemOldName; }

            set
            {
                _itemOldName = value;

                InvokePropertyChanged(nameof(ItemOldName));
            }
        }


        public RenameFileDialog()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void btnOkay_Click(object sender, RoutedEventArgs e)
        {
            if (IsFileNameValid(ItemNewName))
            {
                UserConfirmedRename = true;
                Close();
            }
            else
            {
                MessageBox.Show("The filename you specified is invalid.");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            UserConfirmedRename = false;
            Close();
        }

        private void InvokePropertyChanged(string propertyName)
        {
            var args = new PropertyChangedEventArgs(propertyName);

            PropertyChanged?.Invoke(this, args);
        }

        //Note: Not a perfect valiadation function.
        private bool IsFileNameValid(string fileName)
        {
            char[] invalidCharacter = { '/', '\\', '*', ':', '"', '|', '?', '<', '>', '\0' };
            string[] invalidFileNames = { "CON", "RPN", "AUX", "NUL", "COM1", "COM2", "COM3",
                                          "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "LPT1",
                                          "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9" };

            var isFileNameInvalid = (invalidCharacter.Any(ch => fileName.Contains(ch)) ||
                                     invalidFileNames.Any(name => String.Compare(name, fileName, true) == 0)) ||
                                     fileName.EndsWith(".");

            return !isFileNameInvalid;
        }
    }
}
