using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Saltukkos.MercurialVS.HgServices;
using Color = System.Drawing.Color;

namespace Saltukkos.MercurialVS.StudioIntegration.FileHistory
{
    public class DiffLineTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is DiffLineType diffLineType))
            {
                return null;
            }

            Color resultColor;
            switch (diffLineType)
            {
                case DiffLineType.MetaInfoLine:
                    resultColor = Color.MediumPurple;
                    break;
                case DiffLineType.AddLine:
                    resultColor = Color.ForestGreen;
                    break;
                case DiffLineType.RemoveLine:
                    resultColor = Color.Red;
                    break;
                default:
                    return new SolidBrush(Color.FromArgb(0));
            }

            return new SolidColorBrush(
                System.Windows.Media.Color.FromArgb(0x50, resultColor.R, resultColor.G, resultColor.B));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}