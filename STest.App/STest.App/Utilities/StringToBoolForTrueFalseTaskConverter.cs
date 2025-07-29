using System;
using Microsoft.UI.Xaml.Data;

namespace STest.App.Utilities
{
    public sealed partial class StringToBoolForTrueFalseTaskConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value is string str && parameter is string param && str.Equals(param, StringComparison.OrdinalIgnoreCase);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return false;
        }
    }
}