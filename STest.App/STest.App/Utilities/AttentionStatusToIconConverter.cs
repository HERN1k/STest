using System;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using STLib.Core.Testing;

namespace STest.App.Utilities
{
    public sealed partial class AttentionStatusToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not AttentionType type)
            {
                return new FontIcon()
                {
                    Glyph = "\uE71A",
                    Foreground = new SolidColorBrush(Colors.Black)
                };
            }

            string glyph = type switch
            {
                AttentionType.Warning => "\uF167",
                AttentionType.Critical => "\uE814",
                _ => "\uE71A"
            };

            string resourceKey = type switch
            {
                AttentionType.Warning => "SystemFillColorCautionBrush",
                AttentionType.Critical => "SystemFillColorCriticalBrush",
                _ => string.Empty
            };

            if (string.IsNullOrEmpty(resourceKey))
            {
                return new FontIcon()
                {
                    Glyph = glyph,
                    Foreground = new SolidColorBrush(Colors.Black)
                };
            }

            if (Application.Current.Resources.TryGetValue(resourceKey, out var brush))
            {
                return new FontIcon()
                {
                    Glyph = glyph,
                    Foreground = brush as SolidColorBrush ?? new SolidColorBrush(Colors.Black)
                };
            }

            return new FontIcon()
            {
                Glyph = "\uE71A",
                Foreground = new SolidColorBrush(Colors.Black)
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return new FontIcon()
            {
                Glyph = "\uE71A",
                Foreground = new SolidColorBrush(Colors.Black)
            };
        }
    }
}