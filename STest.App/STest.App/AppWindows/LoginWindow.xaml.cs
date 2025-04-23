using System;
using Microsoft.UI.Xaml;
using WinRT;
using STest.App.Utilities;
using STest.App.Domain.Interfaces;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Composition;
using Windows.Graphics;
using Microsoft.Extensions.Logging;
using STest.App.Domain.Enums;

namespace STest.App.AppWindows
{
    /// <summary>
    /// Login window
    /// </summary>
    public sealed partial class LoginWindow : Window, IDisposable
    {
        private readonly ILocalization m_localization;
        private readonly ILocalData m_localData;
        private readonly ILogger<LoginWindow> m_logger;
        private readonly IWindowsHelper m_windowsHelper;
        private WindowsSystemDispatcherQueueHelper? m_wsdqHelper;
        private DesktopAcrylicController? m_acrylicController;
        private SystemBackdropConfiguration? m_configurationSource;
        private bool m_disposedValue;

        public LoginWindow(ILocalization localization, ILocalData localData, ILogger<LoginWindow> logger, IWindowsHelper windowsHelper)
        {
            this.InitializeComponent();
            m_localization = localization ?? throw new ArgumentNullException(nameof(localization));
            m_localData = localData ?? throw new ArgumentNullException(nameof(localData));
            m_logger = logger ?? throw new ArgumentNullException(nameof(logger));
            m_windowsHelper = windowsHelper ?? throw new ArgumentNullException(nameof(windowsHelper));
            m_windowsHelper.ConfigureTitleBar(this);
            SubscribeToEvents();
            Init();
            TrySetAcrylicBackdrop(useAcrylicThin: false);
        }

        /// <summary>
        /// Activate the main window
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        private void ButtonSendClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var app = Application.Current as App
                    ?? throw new ArgumentNullException(nameof(Application.Current));

                m_localData.SetString(Constants.USER_EMAIL_LOCAL_DATA, EmailInput.Text);
                m_localData.SetString(Constants.USER_RANK_LOCAL_DATA, UserRank.Teacher.ToString());
                m_localData.SetString(Constants.USER_NAME_LOCAL_DATA, "Pochtalyon Petchkin");

                app.ActivateMainWindow();
            }
            catch (Exception ex)
            {
                ex.Show(m_logger);
            }
        }

        /// <summary>
        /// Subscribe to events
        /// </summary>
        private void SubscribeToEvents()
        {
            if (DesktopAcrylicController.IsSupported())
            {
                this.Activated += OnWindowActivated;
                this.Closed += OnWindowClosed;
            }
        }

        /// <summary>
        /// Initialize the window
        /// </summary>
        private void Init()
        {
            try
            {
                this.AppWindow.Resize(new SizeInt32
                {
                    Width = 600,
                    Height = 500
                });
            }
            catch (Exception ex)
            {
                ex.Show(m_logger);
            }
        }

        #region Acrylic Backdrop
        /// <summary>
        /// Try to set the acrylic backdrop
        /// </summary>
        private void TrySetAcrylicBackdrop(bool useAcrylicThin)
        {
            try
            {
                if (!DesktopAcrylicController.IsSupported())
                {
                    return;
                }

                m_wsdqHelper = new WindowsSystemDispatcherQueueHelper();
                m_wsdqHelper.EnsureWindowsSystemDispatcherQueueController();

                m_configurationSource = new()
                {
                    IsInputActive = true
                };

                SetConfigurationSourceTheme();

                m_acrylicController = new()
                {
                    Kind = useAcrylicThin ? DesktopAcrylicKind.Thin : DesktopAcrylicKind.Base
                };

                m_acrylicController.AddSystemBackdropTarget(this.As<ICompositionSupportsSystemBackdrop>());
                m_acrylicController.SetSystemBackdropConfiguration(m_configurationSource);
            }
            catch (Exception ex)
            {
                ex.Show(m_logger);
            }
        }

        /// <summary>
        /// Handle the window activated event
        /// </summary>
        private void OnWindowActivated(object sender, WindowActivatedEventArgs args)
        {
            try
            {
                if (m_configurationSource != null)
                {
                    m_configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
                }
            }
            catch (Exception ex)
            {
                ex.Show(m_logger);
            }
        }

        /// <summary>
        /// Handle the window closed event
        /// </summary>
        private void OnWindowClosed(object sender, WindowEventArgs args)
        {
            if (m_acrylicController != null)
            {
                m_acrylicController.Dispose();
                m_acrylicController = null;
            }
            this.Activated -= OnWindowActivated;
            m_configurationSource = null;
        }

        /// <summary>
        /// Set the configuration source theme
        /// </summary>
        private void SetConfigurationSourceTheme()
        {
            if (m_configurationSource == null)
            {
                return;
            }

            switch (((FrameworkElement)this.Content).ActualTheme)
            {
                case ElementTheme.Dark:
                    m_configurationSource.Theme = SystemBackdropTheme.Dark;
                    break;
                case ElementTheme.Light:
                    m_configurationSource.Theme = SystemBackdropTheme.Light;
                    break;
                case ElementTheme.Default:
                    m_configurationSource.Theme = SystemBackdropTheme.Default;
                    break;
            }
        }
        #endregion

        #region Disposing
        /// <summary>
        /// Dispose
        /// </summary>
        private void Dispose(bool disposing)
        {
            if (!m_disposedValue)
            {
                if (disposing)
                {
                    if (m_acrylicController != null)
                    {
                        m_acrylicController.Dispose();
                        m_acrylicController = null;
                    }
                    this.Activated -= OnWindowActivated;
                    m_configurationSource = null;
                }

                m_disposedValue = true;
            }
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~LoginWindow()
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