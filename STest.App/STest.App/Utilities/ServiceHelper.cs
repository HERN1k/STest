using System;
using Microsoft.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
using STest.App.Domain.Interfaces;
using Microsoft.Extensions.Logging;

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
        /// <exception cref="ArgumentNullException"></exception>
        public static T GetService<T>() where T : class, IService
        {
            try
            {
                return (Application.Current as App)?.ServiceProvider.GetService<T>()
                    ?? throw new ArgumentNullException($"Service of type {nameof(IWindowsHelper)[1..]} not found.");
            }
            catch (Exception ex)
            {
                Alerts.ShowCriticalErrorWindow(ex);

                throw;
            }
        }

        /// <summary>
        /// Get a logger of type <typeparamref name="T"/>
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public static ILogger<T> GetLogger<T>() where T : class
        {
            try
            {
                return (Application.Current as App)?.ServiceProvider.GetService<ILogger<T>>()
                    ?? throw new ArgumentNullException($"Logger for type {nameof(T)[1..]} not found.");
            }
            catch (Exception ex)
            {
                Alerts.ShowCriticalErrorWindow(ex);

                throw;
            }
        }
    }
}