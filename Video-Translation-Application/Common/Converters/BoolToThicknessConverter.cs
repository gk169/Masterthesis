using System;
using System.Globalization;
using System.Windows.Data;

namespace VideoTranslationTool.Converters
{
    /// <summary>
    /// Converter <c>BoolToThicknessConverter</c> to convert bool to Button Border Thickness
    /// </summary>
    public class BoolToThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool input = (bool)value;
            if (input) return 3;
            else return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
