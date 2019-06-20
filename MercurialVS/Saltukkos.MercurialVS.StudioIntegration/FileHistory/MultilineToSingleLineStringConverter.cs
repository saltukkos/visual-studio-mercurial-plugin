using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace Saltukkos.MercurialVS.StudioIntegration.FileHistory
{
    public class MultilineToSingleLineStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is null ? null : Regex.Replace(value.ToString(), @"(\r\n|\r|\n)", " ⏎ ");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}