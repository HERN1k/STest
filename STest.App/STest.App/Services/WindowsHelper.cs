using System;
using STest.App.Domain.Interfaces;
using Windows.Networking.Connectivity;
using STest.App.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.UI;
using System.Management;
using System.Linq;

namespace STest.App.Services
{
    /// <summary>
    /// OS Windows helper service
    /// </summary>
    public sealed class WindowsHelper : IWindowsHelper
    {
        public string SystemName => m_systemName;

        private readonly ILogger<WindowsHelper> m_logger;
        private readonly string m_systemName;

        public WindowsHelper(ILogger<WindowsHelper> logger)
        {
            try
            {
                m_logger = logger ?? throw new ArgumentNullException(nameof(logger));
                m_systemName = GetSystemName();

                m_logger.LogInformation("{system}", m_systemName);
            }
            catch (Exception ex)
            {
                Alerts.ShowCriticalErrorWindow(ex);

                throw;
            }
        }

        /// <summary>
        /// Check if the internet is available
        /// </summary>
        public bool IsInternetAvailable()
        {
            var connectionProfile = NetworkInformation.GetInternetConnectionProfile();

            return connectionProfile != null &&
                   connectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
        }

        /// <summary>
        /// Configure title bar for the current window
        /// </summary>
        public void ConfigureTitleBar(Window currentWindow)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(currentWindow);

                currentWindow.Title = MessageProvider.APP_DISPLAY_NAME_KEY;

                if (!AppWindowTitleBar.IsCustomizationSupported())
                {
                    return;
                }

                if (currentWindow.ExtendsContentIntoTitleBar == true)
                {
                    currentWindow.AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
                }

                currentWindow.AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
                currentWindow.AppWindow.TitleBar.ButtonBackgroundColor = Color.FromArgb(0, 255, 255, 255);
                currentWindow.AppWindow.TitleBar.InactiveBackgroundColor = currentWindow.AppWindow.TitleBar.ButtonBackgroundColor;
                currentWindow.AppWindow.TitleBar.ButtonInactiveBackgroundColor = currentWindow.AppWindow.TitleBar.ButtonBackgroundColor;
            }
            catch (Exception ex)
            {
                ex.Show(m_logger);
            }
        }

        /// <summary>
        /// Get the system name
        /// </summary>
        private static string GetSystemName()
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
            string systemName = string.Empty;

            foreach (var os in searcher.Get())
            {
                systemName = string.Concat(
                    os.GetPropertyValue("Caption").ToString() ?? Constants.NULL,    // Example: Microsoft Windows 11 Pro
                    " ",
                    os.GetPropertyValue("Version").ToString() ?? Constants.NULL     // Example: 10.0.22000
                );
            }

            var lines = systemName.Split(' ').ToList();

            if (lines.Count > 0)
            {
                lines.RemoveAt(0);  // Remove the first element ("Microsoft")
            }

            return string.Join(' ', lines) ?? Constants.NULL;
        }
    }
}