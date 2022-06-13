using System;
using System.Globalization;
using System.Windows.Data;

namespace Taskish.Converters
{
    public class StringEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateOnly)
            {
                if (string.IsNullOrEmpty(((DateOnly)value).ToString()))
                    return parameter;
                else if (((DateOnly)value) == DateOnly.FromDateTime(DateTime.Today.Date))
                    return "Today";
                else if (((DateOnly)value) == DateOnly.FromDateTime(DateTime.Today.Date).AddDays(1))
                    return "Tomorrow";
                else return ((DateOnly)value).ToString("ddd, dd MMMM");
            }
            return string.IsNullOrEmpty((string)value) ? parameter : value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
