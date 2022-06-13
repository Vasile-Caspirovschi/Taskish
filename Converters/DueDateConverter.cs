using System;
using System.Windows.Data;

namespace Taskish.Converters
{
    public class DueDateConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //return (value != null) ? "Due " + ((DateOnly)value).ToString("ddd, dd MMMM") : parameter;
            if (value == null)
                return parameter;
            else if (((DateOnly)value) == DateOnly.FromDateTime(DateTime.Today.Date))
                return "Due Today";
            else if (((DateOnly)value) == DateOnly.FromDateTime(DateTime.Today.Date).AddDays(1))
                return "Due Tomorrow";
            else return "Due " + ((DateOnly)value).ToString("ddd, dd MMMM");

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }

    }
}
