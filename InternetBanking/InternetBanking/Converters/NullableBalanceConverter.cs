using System;
using System.Globalization;

using Xamarin.Forms;

namespace InternetBanking.Converters
{
    public class NullableBalanceConverter : IValueConverter
    {
        private const string DefaultString = "No account selected";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return DefaultString;
            }

            decimal converted = (decimal)value;

            if (converted == 0)
            {
                return DefaultString;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
