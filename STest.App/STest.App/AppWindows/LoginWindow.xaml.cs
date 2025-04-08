using System;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using WinRT;
using STest.App.Utilities;
using STest.App.Domain.Interfaces;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Composition;
using Windows.UI;
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
        private readonly ILogger<LoginWindow> m_logger;
        /// <summary>
        /// The Windows system dispatcher queue helper
        /// </summary>
        private WindowsSystemDispatcherQueueHelper? m_wsdqHelper;
        /// <summary>
        /// The acrylic controller
        /// </summary>
        private DesktopAcrylicController? m_acrylicController;
        /// <summary>
        /// The configuration source
        /// </summary>
        private SystemBackdropConfiguration? m_configurationSource;
        /// <summary>
        /// The value to indicate if the object has been disposed
        /// </summary>
        private bool m_disposedValue;

        /// <summary>
        /// Constructor
        /// </summary>
        public LoginWindow()
        {
            this.InitializeComponent();
            m_localization = ServiceHelper.GetService<ILocalization>();
            m_localData = ServiceHelper.GetService<ILocalData>();
            m_logger = ServiceHelper.GetLogger<LoginWindow>();
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
                m_localData.SetString(Constants.USER_RANK_LOCAL_DATA, UserRank.Student.ToString());

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
                EmailTitle.Text = m_localization.GetString(Constants.EMAIL_KEY);
                PasswordTitle.Text = m_localization.GetString(Constants.PASSWORD_KEY);
                SendButton.Content = m_localization.GetString(Constants.SEND_KEY);

                this.Title = m_localization.GetString(Constants.APP_DISPLAY_NAME_KEY);
                TitleBarTextBlock.Text = this.Title;

                if (!AppWindowTitleBar.IsCustomizationSupported())
                {
                    return;
                }

                if (ExtendsContentIntoTitleBar == true)
                {
                    this.AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
                }

                this.AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
                this.AppWindow.TitleBar.ButtonBackgroundColor = Color.FromArgb(0, 255, 255, 255);
                this.AppWindow.TitleBar.InactiveBackgroundColor = this.AppWindow.TitleBar.ButtonBackgroundColor;
                this.AppWindow.TitleBar.ButtonInactiveBackgroundColor = this.AppWindow.TitleBar.ButtonBackgroundColor;

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