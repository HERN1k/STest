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
    [SupportedOSPlatform("windows10.0.17763.0")] // Добавить нормальное логирование 
    public sealed partial class SettingsPage : Page
    {
        /// <summary>
        /// <see cref="ILocalization"/> instance
        /// </summary>
        private readonly ILocalization m_localization;
        /// <summary>
        /// <see cref="ILocalData"/> instance
        /// </summary>
        private readonly ILocalData m_localData;
        /// <summary>
        /// <see cref="ILogger"/> instance
        /// </summary>
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
        }

        #region OnNavigated
        /// <summary>
        /// OnNavigatedTo
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            SubscribeToEvents();
            TitleText.Text = m_localization.GetString(Constants.SETTINGS_KEY);
            SetLanguageDropDownTexts();
            VersionText.Text = GetVersion();
            DebugDashboardList.ItemsSource = GenerateDebugDashboardItems();
            SetDebugDashboardText();
        }

        /// <summary>
        /// OnNavigatingFrom
        /// </summary>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            UnSubscribeToEvents();
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
            LanguageText.Text = m_localization.GetString(Constants.APPLICATION_LANGUAGE_KEY);

            LanguageDropDown.Content = m_localization.CurrentCulture.Name switch
            {
                string name when name.Equals(
                    m_localization.EnglishCulture.Name,
                    StringComparison.InvariantCultureIgnoreCase
                ) => m_localization.GetString(Constants.ENGLISH_KEY),

                string name when name.Equals(
                    m_localization.UkrainianCulture.Name,
                    StringComparison.InvariantCultureIgnoreCase
                ) => m_localization.GetString(Constants.UKRAINIAN_KEY),

                _ => m_localization.GetString(Constants.ENGLISH_KEY)
            };

            var menuItems = new List<MenuFlyoutItem>()
            {
                new()
                {
                    Text = m_localization.GetString(Constants.ENGLISH_KEY),
                    Tag = m_localization.EnglishCulture.Name,

                },
                new()
                {
                    Text = m_localization.GetString(Constants.UKRAINIAN_KEY),
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
        /// Log event handler
        /// </summary>
        private async void LogEventHandler(object? sender, LogEventInfo args)
        {
            DebugDashboardText.Blocks.Add(StringExtensions.FormattedLog(args));

            await Task.Delay(50);

            ScrollConsoleToBottom();
        }

        /// <summary>
        /// On loaded
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            ScrollConsoleToBottom();
        }

        /// <summary>
        /// Language drop down click
        /// </summary>
        private void LanguageDropDownClick(object sender, RoutedEventArgs args)
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

        /// <summary>
        /// Copy to clipboard button click
        /// </summary>
        private void CopyToClipboardButtonClick(object sender, RoutedEventArgs srgs)
        {
            var package = new DataPackage();

            package.SetText((sender as FrameworkElement)?.Tag.ToString() ?? string.Empty);

            Clipboard.SetContent(package);

            for (int i = 0; i < 50; i++)
            {
                m_logger.LogInformation("Copied to clipboard");
            }
        }

        /// <summary>
        /// Clear console button click
        /// </summary>
        private void ClearConsoleButtonClick(object sender, RoutedEventArgs srgs)
        {
            if (Application.Current is App app)
            {
                app.LoggerTarget.Clear();

                DebugDashboardText.Blocks.Clear();
            }
        }

        #region Exceptions
        /// <summary>
        /// Show alert
        /// </summary>
        private void EnsureAddedTaskToUIThread(bool isEnqueued)
        {
            if (!isEnqueued)
            {
                this.ShowAlert(
                    title: m_localization.GetString(Constants.ERROR_KEY),
                    message: m_localization.GetString(Constants.FAILED_ADD_TASK_TO_UI_THREAD_KEY),
                    InfoBarSeverity.Error);
            }
        }

        /// <summary>
        /// Show exception
        /// </summary>
        private void ShowException(Exception ex)
        {
#if DEBUG
            this.ShowAlertExceptionWithTrace(ex, m_localization);
#else
            this.ShowAlertException(ex, m_localization);
#endif
        }
        #endregion
    }
}