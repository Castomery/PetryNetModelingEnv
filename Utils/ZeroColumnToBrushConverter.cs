using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace PetryNet.Utils
{
    internal class ZeroColumnToBrushConverter : IMultiValueConverter
    {
        public Brush HighlightBrush { get; set; } = Brushes.Red;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2) return Brushes.Transparent;

            if (values[0] is int index && values[1] is IEnumerable<int> zeroCols)
            {
                return zeroCols.Contains(index) ? HighlightBrush : Brushes.Transparent;
            }

            return Brushes.Transparent;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
