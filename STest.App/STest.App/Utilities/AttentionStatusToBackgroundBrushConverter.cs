using System;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using STLib.Core.Testing;

namespace STest.App.Utilities
{
    public sealed partial class AttentionStatusToBackgroundBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not AttentionType type)
            {
                return new SolidColorBrush(Colors.White);
            }

            string resourceKey = type switch
            {
                AttentionType.Warning => "SystemFillColorCautionBackgroundBrush",
                AttentionType.Critical => "SystemFillColorCriticalBackgroundBrush",
                _ => string.Empty
            };

            if (string.IsNullOrEmpty(resourceKey))
            {
                return new SolidColorBrush(Colors.White);
            }

            if (Application.Current.Resources.TryGetValue(resourceKey, out var brush))
            {
                return brush;
            }

            return new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return new SolidColorBrush(Colors.White);
        }
    }
}