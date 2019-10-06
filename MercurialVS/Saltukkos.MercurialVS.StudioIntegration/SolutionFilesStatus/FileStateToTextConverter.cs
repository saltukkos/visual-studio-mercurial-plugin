using System;
using System.Globalization;
using System.Windows.Data;
using Saltukkos.MercurialVS.HgServices;

namespace Saltukkos.MercurialVS.StudioIntegration.SolutionFilesStatus
{
    public class FileStateToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is FileStatus fileStatus))
            {
                return null;
            }

            switch (fileStatus)
            {
                case FileStatus.Unknown:
                    return "?";
                case FileStatus.Modified:
                    return "M";
                case FileStatus.Added:
                    return "A";
                case FileStatus.Removed:
                    return "R";
                case FileStatus.Clean:
                    return "C";
                case FileStatus.Missing:
                    return "!";
                case FileStatus.Ignored:
                    return "I";
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), fileStatus, string.Empty);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}