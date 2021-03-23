using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace VideoTranslationTool.Converters
{
    /// <summary>
    /// Converter <c>BoolToColorConverter</c> to convert bool to Button BorderBrush Color
    /// </summary>
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool input = (bool)value;
            if (input) return (SolidColorBrush)Application.Current.Resources["HHN_Blue"];
            else return SystemColors.ActiveBorderBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
