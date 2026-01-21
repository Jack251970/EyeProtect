using System;
using System.Globalization;
using System.Windows.Data;
using Project1.UI.Controls.Enums;
using Project1.UI.Cores;

namespace Project1.UI.Controls.Converters
{
    public class Project1UIIconToUnicodeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var icon = (Project1UIIconType)value;
            var result = IconFonts.GetUnicodeString(icon);
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
