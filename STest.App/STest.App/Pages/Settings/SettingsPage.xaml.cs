using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using NLog;
using STest.App.Domain.Interfaces;
using STest.App.Domain.ListViewEntities;
using STest.App.Utilities;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;

namespace STest.App.Pages.Settings
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class SettingsPage : Page
    {
#pragma warning disable CA1822 
        public string Version => GetVersion();
#pragma warning restore CA1822
        public ExtendedObservableCollection<DebugDashboardItem> DebugDashboardItems { get; }

        private readonly ILocalization m_localization;
        private readonly ILocalData m_localData;
        private readonly ILogger<SettingsPage> m_logger;

        /// <summary>
        /// Constructor
        /// </summary>
        public SettingsPage()
        {
            this.InitializeComponent();
            m_localization = ServiceHelper.GetService<ILocalization>();
            m_localData = ServiceHelper.GetService<ILocalData>();
            m_logger = ServiceHelper.GetLogger<SettingsPage>();
            DebugDashboardItems = new();
            this.DataContext = this;
        }

        #region OnNavigated
        /// <summary>
        /// OnNavigatedTo
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                base.OnNavigatedTo(e);

                SubscribeToEvents();
                SetLanguageDropDownTexts();
                DebugDashboardItems.AddRange(GenerateDebugDashboardItems());
                SetDebugDashboardText();
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }

        /// <summary>
        /// OnNavigatingFrom
        /// </summary>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            try
            {
                base.OnNavigatingFrom(e);

                UnSubscribeToEvents();
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        #endregion

        /// <summary>
        /// Subscribe to events
        /// </summary>
        private void SubscribeToEvents()
        {
            if (Application.Current is App app)
            {
                app.LoggerTarget.LogReceived += LogEventHandler;
            }
            this.Loaded += OnLoaded;
        }

        /// <summary>
        /// Un subscribe to events
        /// </summary>
        private void UnSubscribeToEvents()
        {
            if (Application.Current is App app)
            {
                app.LoggerTarget.LogReceived -= LogEventHandler;
            }
            this.Loaded -= OnLoaded;
        }

        /// <summary>
        /// Generate debug dashboard items
        /// </summary>
        private IEnumerable<DebugDashboardItem> GenerateDebugDashboardItems()
        {
            return m_localData
                .GetAll()
                .Select((item) => 
                {
                    if (string.IsNullOrEmpty(item.Key) || item.Key.Length < 12)
                    {
                        return new DebugDashboardItem("Null", "Null");
                    }

                    if (string.IsNullOrEmpty(item.Key) || item.Key.Length < 12)
                    {
                        return new DebugDashboardItem("Null", "Null");
                    }

                    var keySpan = item.Key.AsSpan();
                    var firstChar = char.ToUpper(keySpan[0]);
                    var restSpan = keySpan.Slice(1, keySpan.Length - 12);

                    var sb = new StringBuilder(restSpan.Length + 1);
                    sb.Append(firstChar);

                    foreach (var ch in restSpan)
                    {
                        sb.Append(ch == '_' ? ' ' : char.ToLowerInvariant(ch));
                    }

                    var key = sb.ToString();

                    var value = !string.IsNullOrEmpty(item.Value) ? item.Value : "Null";

                    return new DebugDashboardItem(key, value);
                });
        }

        /// <summary>
        /// Get version
        /// </summary>
        private static string GetVersion()
        {
            var sb = new StringBuilder();
            var version = Package.Current.Id.Version;

            sb.Append(nameof(Package.Current.Id.Version));
            sb.Append(' ');
            sb.Append(version.Major);
            sb.Append('.');
            sb.Append(version.Minor);
            sb.Append('.');
            sb.Append(version.Build);
            sb.Append('.');
            sb.Append(version.Revision);

            return sb.ToString();
        }

        /// <summary>
        /// Set language drop down texts
        /// </summary>
        private void SetLanguageDropDownTexts()
        {
            LanguageText.Text = T(Constants.APPLICATION_LANGUAGE_KEY);

            LanguageDropDown.Content = m_localization.CurrentCulture.Name switch
            {
                string name when name.Equals(
                    m_localization.EnglishCulture.Name,
                    StringComparison.InvariantCultureIgnoreCase
                ) => T(Constants.ENGLISH_KEY),

                string name when name.Equals(
                    m_localization.UkrainianCulture.Name,
                    StringComparison.InvariantCultureIgnoreCase
                ) => T(Constants.UKRAINIAN_KEY),

                _ => T(Constants.ENGLISH_KEY)
            };

            var menuItems = new List<MenuFlyoutItem>()
            {
                new()
                {
                    Text = T(Constants.ENGLISH_KEY),
                    Tag = m_localization.EnglishCulture.Name,
                },
                new()
                {
                    Text = T(Constants.UKRAINIAN_KEY),
                    Tag = m_localization.UkrainianCulture.Name
                }
            };

            var menuFlyout = new MenuFlyout();

            foreach (var item in menuItems)
            {
                item.Click += LanguageDropDownClick;

                menuFlyout.Items.Add(item);
            }

            LanguageDropDown.Flyout = menuFlyout;
        }

        /// <summary>
        /// Set debug dashboard text
        /// </summary>
        private void SetDebugDashboardText()
        {
            if (Application.Current is App app)
            {
                var paragraphs = app.LoggerTarget.Logs
                    .Select(StringExtensions.FormattedLog)
                    .ToList();

                DebugDashboardText.Blocks.Clear();

                foreach (var p in paragraphs)
                {
                    DebugDashboardText.Blocks.Add(p);
                }
            }
        }

        /// <summary>
        /// Scroll console to bottom
        /// </summary>
        private void ScrollConsoleToBottom()
        {
            DebugDashboardTextScrollViewer.ChangeView(
                horizontalOffset: null,
                verticalOffset: DebugDashboardTextScrollViewer.ScrollableHeight,
                zoomFactor: null);
        }

        /// <summary>
        /// Get the localized string by key
        /// </summary>
        /// <param name="key"></param>
        private string T(string key) => m_localization.T(key);

        /// <summary>
        /// Log event handler
        /// </summary>
        private async void LogEventHandler(object? sender, LogEventInfo args)
        {
            await this.ExecuteOnDispatcherQueueAsync(m_logger, async () => 
            {
                DebugDashboardText.Blocks.Add(StringExtensions.FormattedLog(args));

                await Task.Delay(50);

                ScrollConsoleToBottom();
            });
        }

        /// <summary>
        /// On loaded
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            try
            {
                ScrollConsoleToBottom();
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }

        /// <summary>
        /// Language drop down click
        /// </summary>
        private void LanguageDropDownClick(object sender, RoutedEventArgs args)
        {
            try
            {
                if (sender is MenuFlyoutItem item)
                {
                    string culture = item.Tag.ToString() switch
                    {
                        string name when name.Equals(
                            m_localization.EnglishCulture.Name,
                            StringComparison.InvariantCultureIgnoreCase
                        ) => m_localization.EnglishCulture.Name,

                        string name when name.Equals(
                            m_localization.UkrainianCulture.Name,
                            StringComparison.InvariantCultureIgnoreCase
                        ) => m_localization.UkrainianCulture.Name,

                        _ => m_localization.EnglishCulture.Name
                    };

                    if (culture.Equals(m_localization.CurrentCulture.Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return;
                    }

                    m_localization.ChangeCulture(culture);
                    LanguageDropDown.Content = item.Text;

                    (Application.Current as App)?.ReloadPage();
                }
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }

        /// <summary>
        /// Copy to clipboard button click
        /// </summary>
        private void CopyToClipboardButtonClick(object sender, RoutedEventArgs srgs)
        {
            try
            {
                var package = new DataPackage();

                package.SetText((sender as FrameworkElement)?.Tag.ToString() ?? string.Empty);

                Clipboard.SetContent(package);
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }

        /// <summary>
        /// Clear console button click
        /// </summary>
        private void ClearConsoleButtonClick(object sender, RoutedEventArgs srgs)
        {
            try
            {
                if (Application.Current is App app)
                {
                    app.LoggerTarget.Clear();

                    DebugDashboardText.Blocks.Clear();
                }
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
    }
}