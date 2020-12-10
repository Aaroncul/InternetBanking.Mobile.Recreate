using System;
using System.Globalization;

using Xamarin.Forms;

namespace InternetBanking.Converters
{
    public class CharactersRemainingConverter : IValueConverter
    {
        private const int MaximumCharacters = 500;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string s))
            {
                return $"{MaximumCharacters} characters left";
            }

            var remaining = MaximumCharacters - s.Length;

            return $"{remaining} character{(remaining == 1 ? "" : "s")} left";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

