using System;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;

namespace Noria.UI
{
    public class FileTypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)value == "Folder"
                ? Brushes.Yellow
                : Brushes.Blue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
