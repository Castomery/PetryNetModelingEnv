using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace PetryNet.Utils
{
    internal class TokenCountToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int tokenCount)
            {
                bool shouldBeVisible = tokenCount > 5; // Adjust threshold as needed

                // Check if we need to invert the logic
                if (parameter is string param && param == "inverse")
                    shouldBeVisible = !shouldBeVisible;

                return shouldBeVisible ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new object();
        }
    }
}
