using Microsoft.Extensions.Logging;
using System.Globalization;
using System;
using STest.App.Domain.Interfaces;
using STest.App.Utilities;
using System.Threading;
using Microsoft.Windows.Globalization;
using System.Resources;
using System.Reflection;
using System.Text.RegularExpressions;

namespace STest.App.Services
{
    /// <summary>
    /// Service for localization
    /// </summary>
    public partial class Localization : ILocalization
    {
        /// <summary>
        /// Current culture
        /// </summary>
        public CultureInfo CurrentCulture
        {
            get => m_currentCulture;
            private set
            {
                if (m_currentCulture != value)
                {
                    m_currentCulture = value;

                    OnCultureChanged(new CultureChangedEventArgs(value));
                }
            }
        }
        /// <summary>
        /// English culture
        /// </summary>
        public CultureInfo EnglishCulture { get => m_englishCulture; }
        /// <summary>
        /// Ukrainian culture
        /// </summary>
        public CultureInfo UkrainianCulture { get => m_ukrainianCulture; }
        /// <summary>
        /// Event for culture changed
        /// </summary>
        public event EventHandler<CultureChangedEventArgs>? CultureChanged;

        /// <summary>
        /// Local data
        /// </summary>
        private readonly ILocalData m_localData;
        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger<Localization> m_logger;
        /// <summary>
        /// Lock object
        /// </summary>
        private readonly object m_lockObj;
        /// <summary>
        /// Resource manager
        /// </summary>
        private readonly ResourceManager m_resourceManager;
        /// <summary>
        /// English culture
        /// </summary>
        private readonly CultureInfo m_englishCulture;
        /// <summary>
        /// Ukrainian culture
        /// </summary>
        private readonly CultureInfo m_ukrainianCulture;
        /// <summary>
        /// Current culture
        /// </summary>
        private CultureInfo m_currentCulture;
        /// <summary>
        /// Disposed value
        /// </summary>
        private bool m_disposedValue;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public Localization(ILocalData localData, ILogger<Localization> logger)
        {
            m_localData = localData ?? throw new ArgumentNullException(nameof(localData));
            m_logger = logger ?? throw new ArgumentNullException(nameof(logger));
            m_lockObj = new();
            m_resourceManager = new(Constants.RESOURCE_MANAGER_NAME, Assembly.GetExecutingAssembly());
            m_englishCulture = new("en-US");
            m_ukrainianCulture = new("uk-UA");
            m_currentCulture = GetPreferredLanguage();
            Thread.CurrentThread.CurrentCulture = m_currentCulture;
            Thread.CurrentThread.CurrentUICulture = m_currentCulture;
            CultureInfo.CurrentCulture = m_currentCulture;
            CultureInfo.CurrentUICulture = m_currentCulture;
            m_logger.LogInformation("Preferred language {Name}", m_currentCulture.EnglishName);
        }

        /// <summary>
        /// Get string by key
        /// </summary>
        public string T(string key, CultureInfo? culture = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                return Constants.KEY_NOT_FOUND;
            }
            
            try
            {
                string? result = m_resourceManager.GetString(key, culture ?? CurrentCulture);

                if (string.IsNullOrEmpty(result))
                {
                    return Constants.KEY_NOT_FOUND;
                }

                return result;
            }
            catch (Exception ex)
            {
                m_logger.LogError(ex, "{Message}", ex.Message);

                return Constants.KEY_NOT_FOUND;
            }
        }

        /// <summary>
        /// Change culture
        /// </summary>
        /// <exception cref="CultureNotFoundException"></exception>
        /// <exception cref="TimeoutException"></exception>
        public bool ChangeCulture(string cultureCode)
        {
            try
            {
                if (!string.IsNullOrEmpty(cultureCode))
                {
                    if (!cultureCode.Equals(m_englishCulture.Name, StringComparison.OrdinalIgnoreCase) &&
                        !cultureCode.Equals(m_ukrainianCulture.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new CultureNotFoundException(Constants.CULTURE_CODE_NOT_FOUND_OR_INVALID);
                    }

                    CultureInfo culture;
                    try
                    {
                        culture = new CultureInfo(cultureCode);
                    }
                    catch (CultureNotFoundException)
                    {
                        m_logger.LogWarning(Constants.CULTURE_CODE_NOT_FOUND_OR_INVALID);
                        return false;
                    }
                    catch (Exception)
                    {
                        throw;
                    }

                    lock (m_lockObj)
                    {
                        ApplicationLanguages.PrimaryLanguageOverride = culture.Name;

                        Windows.ApplicationModel.Resources.Core.ResourceContext
                            .GetForViewIndependentUse()
                            .Reset();

                        Thread.CurrentThread.CurrentCulture = culture;
                        Thread.CurrentThread.CurrentUICulture = culture;
                        CultureInfo.CurrentCulture = culture;
                        CultureInfo.CurrentUICulture = culture;
                    }

                    var retries = 0;
                    const int maxRetries = 10;
                    do
                    {
                        Thread.Sleep(50);

                        retries++;
                        if (retries >= maxRetries)
                        {
                            throw new TimeoutException(Constants.FAILED_TO_LOAD_RESOURCES_FOR_NEW_CULTURE);
                        }
                    }
                    while (string.IsNullOrEmpty(T(Constants.APP_DISPLAY_NAME_KEY, culture)));

                    CurrentCulture = culture;
                    m_localData.SetString(Constants.PREFERRED_LANGUAGE_LOCAL_DATA, culture.Name);

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                m_logger.LogError(ex, "{Message}", ex.Message);

                return false;
            }
        }

        /// <summary>
        /// Event for culture changed
        /// </summary>
        protected virtual void OnCultureChanged(CultureChangedEventArgs args)
        {
            var temp = Volatile.Read(ref CultureChanged);

            m_logger.LogInformation("Language changed on {Name}", args.CurrentCulture.EnglishName);

            temp?.Invoke(this, args);
        }

        /// <summary>
        /// Event arguments for culture changed
        /// </summary>
        public sealed class CultureChangedEventArgs(CultureInfo currentCulture)
        {
            public CultureInfo CurrentCulture { get; } = currentCulture 
                ?? throw new ArgumentNullException(nameof(currentCulture));
        }

        /// <summary>
        /// Get preferred language
        /// </summary>
        /// <exception cref="CultureNotFoundException"></exception>
        private CultureInfo GetPreferredLanguage()
        {
            try
            {
                var preferredLanguage = m_localData.GetString(Constants.PREFERRED_LANGUAGE_LOCAL_DATA);

                if (!string.IsNullOrEmpty(preferredLanguage))
                {
                    if (!preferredLanguage.Equals(m_englishCulture.Name, StringComparison.OrdinalIgnoreCase) &&
                        !preferredLanguage.Equals(m_ukrainianCulture.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new CultureNotFoundException(Constants.CULTURE_CODE_NOT_FOUND_OR_INVALID);
                    }

                    return new CultureInfo(preferredLanguage);
                }

                var languages = ApplicationLanguages.Languages;

                if (languages.Count == 0)
                {
                    m_localData.SetString(Constants.PREFERRED_LANGUAGE_LOCAL_DATA, m_englishCulture.Name);

                    return new CultureInfo(m_englishCulture.Name);
                }

                foreach (var lang in languages)
                {
                    if (string.IsNullOrEmpty(lang))
                    {
                        continue;
                    }

                    if (lang.Equals(m_englishCulture.Name, StringComparison.OrdinalIgnoreCase) ||
                        lang.Equals(m_ukrainianCulture.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        preferredLanguage = lang;

                        break;
                    }
                }

                if (string.IsNullOrEmpty(preferredLanguage))
                {
                    m_localData.SetString(Constants.PREFERRED_LANGUAGE_LOCAL_DATA, m_englishCulture.Name);

                    return new CultureInfo(m_englishCulture.Name);
                }

                m_localData.SetString(Constants.PREFERRED_LANGUAGE_LOCAL_DATA, preferredLanguage);

                return new CultureInfo(preferredLanguage);
            }
            catch (Exception ex)
            {
                m_logger.LogError(ex, "{Message}", ex.Message);

                return new CultureInfo(m_englishCulture.Name);
            }
        }

        #region Disposing
        /// <summary>
        /// Dispose
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposedValue)
            {
                if (disposing)
                {
                    CultureChanged = null;
                }

                m_disposedValue = true;
            }
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~Localization()
        {
            Dispose(disposing: false);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}