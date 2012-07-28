using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Progressive.PecaStarter.View.Converter
{
    public class AgeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return TimeSpan.FromSeconds((int)value).ToString(@"h\:mm");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan result;
            if (!TimeSpan.TryParse((string)value, out result))
            {
                return 0;
            }
            return (int)result.TotalSeconds;
        }
    }
}
