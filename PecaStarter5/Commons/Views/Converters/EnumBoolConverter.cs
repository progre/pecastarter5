using System;
using System.Windows;
using System.Windows.Data;

namespace Progressive.Commons.Views.Converters
{
    internal class EnumBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var parameterString = parameter as string;
            if (parameterString == null)
                return DependencyProperty.UnsetValue;

            if (!Enum.IsDefined(value.GetType(), value))
                return DependencyProperty.UnsetValue;

            return Enum.Parse(value.GetType(), parameterString).Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var parameterString = parameter as string;
            if (parameterString == null || value.Equals(false))
                return DependencyProperty.UnsetValue;

            return Enum.Parse(targetType, parameterString);
        }

        #endregion
    }
}
