using System;
using System.Collections;
using System.Linq;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml;

namespace STest.App.Utilities
{
    public sealed partial class EmptyListToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is IEnumerable enumerable && !enumerable.Cast<object>().Any())
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return Visibility.Visible;
        }
    }
}