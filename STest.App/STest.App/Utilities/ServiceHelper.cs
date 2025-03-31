using System;
using Microsoft.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
using STest.App.Domain.Interfaces;

namespace STest.App.Utilities
{
    /// <summary>
    /// Helper class to get services
    /// </summary>
    public sealed class ServiceHelper
    {
        /// <summary>
        /// Private constructor
        /// </summary>
        private ServiceHelper()
        {
        }

        /// <summary>
        /// Get a service of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static T GetService<T>() where T : class, IService
        {
            return (Application.Current as App)?.ServiceProvider.GetService<T>()
                ?? throw new ArgumentNullException($"Service of type {nameof(IWindowsHelper)[1..]} not found.");
        }
    }
}