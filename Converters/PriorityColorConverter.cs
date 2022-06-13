using System;
using System.Drawing;
using System.Windows.Data;
using System.Windows.Media;

namespace Taskish.Converters
{
    public class PriorityColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var priority = (byte)value;

            switch (priority)
            {
                case 3:
                    return (SolidColorBrush)new BrushConverter().ConvertFromString("#de4c4a");
                case 2:
                    return (SolidColorBrush)new BrushConverter().ConvertFromString("#f49c18");
                case 1:
                    return (SolidColorBrush)new BrushConverter().ConvertFromString("#4073d6");
                case 0:
                    return (SolidColorBrush)new BrushConverter().ConvertFromString("#687681");
            }
            return (SolidColorBrush)new BrushConverter().ConvertFromString("#687681");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
