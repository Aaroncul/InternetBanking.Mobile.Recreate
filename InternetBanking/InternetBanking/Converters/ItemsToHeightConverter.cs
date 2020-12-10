using System;
using System.Globalization;

using Xamarin.Forms;

namespace InternetBanking.Converters
{
    public class ItemsToHeightConverter : IValueConverter
    {
        const int ItemHeight = 156;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is int ? System.Convert.ToInt32(value) * ItemHeight : (object)0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
