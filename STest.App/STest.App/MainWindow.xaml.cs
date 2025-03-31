using System;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System.Diagnostics;
using WinRT;
using STest.App.Utilities;
using STest.App.Domain.Interfaces;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Composition;
using Windows.UI;
using STest.App.Pages.Home;
using STest.App.Pages.Settings;
using STest.App.Pages.Account;

namespace STest.App
{
    /// <summary>
    /// The main window of the application
    /// </summary>
    public sealed partial class MainWindow : Window, IDisposable
    {
        /// <summary>
        /// <see cref="IWindowsHelper"/> instance
        /// </summary>
        private readonly IWindowsHelper m_windowsHelper;
        /// <summary>
        /// <see cref="ILocalization"/> instance
        /// </summary>
        private readonly ILocalization m_localization;
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
        public MainWindow()
        {
            this.InitializeComponent();
            m_windowsHelper = ServiceHelper.GetService<IWindowsHelper>();
            m_localization = ServiceHelper.GetService<ILocalization>();
            SubscribeToEvents();
            Init();
            TrySetAcrylicBackdrop(useAcrylicThin: false);
            NavigateToDefaultPage();
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
            NavigationView.SelectionChanged += NavigationSelectionChanged;
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
                }
                catch (Exception ex)
                {
                    ShowException(ex);
                }
            });

            EnsureAddedTaskToUIThread(isEnqueued, nameof(Init), m_localization);
        }

        /// <summary>
        /// Navigate to the default page
        /// </summary>
        private void NavigateToDefaultPage()
        {
            bool isEnqueued = this.DispatcherQueue.TryEnqueue(() =>
            {
                try
                {
                    RootFrame.Navigate(typeof(HomePage));
                }
                catch (Exception ex)
                {
                    ShowException(ex);
                }
            });

            EnsureAddedTaskToUIThread(isEnqueued, nameof(NavigateToDefaultPage), m_localization);
        }

        /// <summary>
        /// The navigation selection changed event
        /// </summary>
        private void NavigationSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            bool isEnqueued = this.DispatcherQueue.TryEnqueue(() =>
            {
                try
                {
                    if (args.IsSettingsSelected)
                    {
                        RootFrame.Navigate(typeof(SettingsPage));

                        return;
                    }

                    var selectedItem = args.SelectedItem as NavigationViewItem;

                    if (selectedItem != null)
                    {
                        var tag = selectedItem.Tag as string;

                        switch (tag)
                        {
                            case "Home":
                                RootFrame.Navigate(typeof(HomePage));
                                break;
                            case "Account":
                                RootFrame.Navigate(typeof(AccountPage));
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowException(ex);
                }
            });

            EnsureAddedTaskToUIThread(isEnqueued, nameof(NavigationSelectionChanged), m_localization);
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
        /// Show an exception to the user
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
        /// Set the configuration source theme
        /// </summary>
        private void OnWindowActivated(object sender, WindowActivatedEventArgs args)
        {
            if (m_configurationSource != null)
            {
                m_configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
            }
        }

        /// <summary>
        /// Window closed event
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
                    NavigationView.SelectionChanged -= NavigationSelectionChanged;
                }

                m_disposedValue = true;
            }
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~MainWindow()
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