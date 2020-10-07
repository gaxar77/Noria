using System;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;

namespace WpfApp1
{
    public class IsFolderToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value == true
                ? Brushes.Yellow
                : Brushes.Blue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
