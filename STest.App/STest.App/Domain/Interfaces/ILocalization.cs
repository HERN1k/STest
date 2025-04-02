using System;
using System.Globalization;
using static STest.App.Services.Localization;

namespace STest.App.Domain.Interfaces
{
    /// <summary>
    /// Service for localization
    /// </summary>
    public interface ILocalization : IService, IDisposable
    {
        /// <summary>
        /// Current culture
        /// </summary>
        CultureInfo CurrentCulture { get; }

        /// <summary>
        /// English culture
        /// </summary>
        CultureInfo EnglishCulture { get; }

        /// <summary>
        /// Ukrainian culture
        /// </summary>
        CultureInfo UkrainianCulture { get; }

        /// <summary>
        /// Event for culture changed
        /// </summary>
        event EventHandler<CultureChangedEventArgs>? CultureChanged;

        /// <summary>
        /// Get string by key
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        string GetString(string key, CultureInfo? culture = null);

        /// <summary>
        /// Change culture
        /// </summary>
        /// <exception cref="CultureNotFoundException"></exception>
        /// <exception cref="TimeoutException"></exception>
        bool ChangeCulture(string cultureCode);
    }
}