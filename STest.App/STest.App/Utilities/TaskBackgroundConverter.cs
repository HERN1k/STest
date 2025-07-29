using System;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using STLib.Core.Testing;

namespace STest.App.Utilities
{
    public sealed partial class TaskBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!Application.Current.Resources.TryGetValue("LayerOnAcrylicFillColorDefaultBrush", out var fallbackBrush))
            {
                return fallbackBrush;
            }

            if (value is CoreTask task)
            {
                string resourceKey = "SystemFillColorNeutralBackgroundBrush";

                if (task.Consider)
                {
                    if (task.Grade > 0)
                    {
                        resourceKey = "SystemFillColorSuccessBackgroundBrush";
                    }
                    else
                    {
                        resourceKey = "SystemFillColorCriticalBackgroundBrush";
                    }
                }

                if (Application.Current.Resources.TryGetValue(resourceKey, out var brush))
                {
                    return brush;
                }
            }

            return fallbackBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (Application.Current.Resources.TryGetValue("LayerOnAcrylicFillColorDefaultBrush", out var brush))
            {
                return brush;
            }

            return new SolidColorBrush(Colors.White);
        }
    }
}