using System;
using System.Globalization;

using Xamarin.Forms;

namespace InternetBanking.Converters
{
    public class NormalizeDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dateTime = DateTime.Parse(value.ToString());
            var difference = DateTime.Now.Subtract(dateTime);

            if (difference.TotalSeconds < 60)
            {
                return "Just now";
            }

            if (difference.TotalMinutes < 60)
            {
                return $"{(int)difference.TotalMinutes} minutes ago";
            }

            if (difference.TotalDays < 1)
            {
                return $"{(int)difference.TotalHours} hours ago";
            }

            if (difference.TotalDays < 2)
            {
                return "Yesterday";
            }

            return dateTime.ToString("dd/MM/yyyy");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
