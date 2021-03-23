using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VideoTranslationTool.Converters
{
    /// <summary>
    /// Converter <c>BoolToVisibilityConverter</c> to convert bool to Button Visibility
    /// </summary>
    public class BoolVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool input = (bool)value;
            if (input) return Visibility.Visible;
            else return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
