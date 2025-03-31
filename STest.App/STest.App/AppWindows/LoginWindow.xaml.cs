using System;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System.Diagnostics;
using WinRT;
using STest.App.Utilities;
using STest.App.Domain.Interfaces;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Composition;
using Windows.UI;
using Windows.Graphics;

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
            var app = Application.Current as App 
                ?? throw new ArgumentNullException(nameof(Application.Current));

            m_localData.SetString(Constants.EMAIL_LOCAL_DATA, EmailInput.Text); 

            app.ActivateMainWindow();
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
            bool isEnqueued = this.DispatcherQueue.TryEnqueue(() =>
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
                    ShowException(ex);
                }
            });

            EnsureAddedTaskToUIThread(isEnqueued, nameof(Init), m_localization);
        }

        /// <summary>
        /// Ensure that a task is added to the UI thread
        /// </summary>
        private static void EnsureAddedTaskToUIThread(bool isEnqueued, string method, ILocalization localization)
        {
            if (!isEnqueued)
            {
#if DEBUG
                Debug.WriteLine(Constants.FAILED_TO_ADD_TASK_TO_UI_THREAD);
#endif
                Alerts.ShowCriticalErrorWindow(
                    string.Concat(Constants.FAILED_TO_ADD_TASK_TO_UI_THREAD,
                    "\n\n",
                    localization.GetString(Constants.METHOD_KEY) ?? Constants.METHOD_KEY,
                    ": ",
                    method));
            }
        }

        /// <summary>
        /// Show an exception
        /// </summary>
        private static void ShowException(Exception ex)
        {
#if DEBUG
            Debug.WriteLine(ex);
#endif
            Alerts.ShowCriticalErrorWindow(ex);
        }

        #region Acrylic Backdrop
        /// <summary>
        /// Try to set the acrylic backdrop
        /// </summary>
        private void TrySetAcrylicBackdrop(bool useAcrylicThin)
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

        /// <summary>
        /// Handle the window activated event
        /// </summary>
        private void OnWindowActivated(object sender, WindowActivatedEventArgs args)
        {
            if (m_configurationSource != null)
            {
                m_configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
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