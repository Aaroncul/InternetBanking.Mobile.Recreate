using System;
using System.Globalization;

using Xamarin.Forms;

namespace InternetBanking.Converters
{
    public class RightPivotAmountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var amount = (decimal)value;
            return (amount - Math.Truncate(amount)).ToString("F2").Substring(1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
