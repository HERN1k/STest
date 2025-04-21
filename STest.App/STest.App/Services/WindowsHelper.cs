using System;
using STest.App.Domain.Interfaces;
using Windows.Networking.Connectivity;
using Windows.System.Profile;
using STest.App.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.UI;

namespace STest.App.Services
{
    /// <summary>
    /// OS Windows helper service
    /// </summary>
    public sealed class WindowsHelper : IWindowsHelper
    {
        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger<WindowsHelper> m_logger;
        /// <summary>
        /// Windows version number
        /// </summary>
        private readonly ulong m_versionNumber;
        /// <summary>
        /// Windows version major
        /// </summary>
        private readonly ulong m_major;
        /// <summary>
        /// Windows version minor
        /// </summary>
        private readonly ulong m_minor;
        /// <summary>
        /// Windows version build
        /// </summary>
        private readonly ulong m_build;
        /// <summary>
        /// Windows version revision
        /// </summary>
        private readonly ulong m_revision;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public WindowsHelper(ILogger<WindowsHelper> logger)
        {
            try
            {
                m_logger = logger ?? throw new ArgumentNullException(nameof(logger));

                string version = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;

                m_logger.LogInformation("Windows version {version}", version);

                if (ulong.TryParse(version, out ulong versionNumber))
                {
                    m_versionNumber = versionNumber;
                    m_major = (versionNumber & 0xFFFF000000000000L) >> 48;
                    m_minor = (versionNumber & 0x0000FFFF00000000L) >> 32;
                    m_build = (versionNumber & 0x00000000FFFF0000L) >> 16;
                    m_revision = (versionNumber & 0x000000000000FFFFL);
                }
                else
                {
                    throw new InvalidOperationException(Constants.FAILED_TO_RETRIEVE_THE_WINDOWS_VERSION);
                }
            }
            catch (Exception ex)
            {
                Alerts.ShowCriticalErrorWindow(ex);

                throw;
            }
        }

        /// <summary>
        /// Check if the current OS is Windows 11 or higher
        /// </summary>
        /// <returns></returns>
        public bool IsWindows11OrHigher() => m_major >= 10 && m_build >= 22000;

        /// <summary>
        /// Check if the current OS is Windows 10 or higher
        /// </summary>
        public bool IsWindowsVersionAtLeast(int major, int minor, int build)
        {
            if (m_major > (ulong)major)
            {
                return true;
            }
            if (m_major == (ulong)major && m_minor > (ulong)minor)
            {
                return true;
            }
            if (m_major == (ulong)major && m_minor == (ulong)minor && m_build >= (ulong)build)
            {
                return true;
            }

            return false;
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
    }
}