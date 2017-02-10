using Avalonia.Markup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace AvaloniaAV
{
    public class TimeToSecondsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var time = value as TimeSpan?;
            return time?.TotalSeconds;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return default(TimeSpan);
            var seconds = (double)value;
            return TimeSpan.FromSeconds(seconds);
        }

        public static TimeToSecondsConverter Instance { get; set; } = new TimeToSecondsConverter();
    }
}
