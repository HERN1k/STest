using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppNotifications.Builder;
using Microsoft.Windows.AppNotifications;
using STest.App.Domain.Interfaces;
using STest.App.Services;
using STest.App.Utilities;
using STest.App.AppWindows;
using NLog.Config;
using NLog;
using NLog.Extensions.Logging;

namespace STest.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, IDisposable
    {
        /// <summary>
        /// <see cref="Utilities.RealTimeLogTarget"/> instance
        /// </summary>
        public InMemoryLoggerTarget LoggerTarget 
        {
            get => m_loggerTarget ?? throw new InvalidOperationException(Constants.AN_UNEXPECTED_ERROR_OCCURRED);
        }
        /// <summary>
        /// <see cref="IServiceProvider"/> instance
        /// </summary>
        public IServiceProvider ServiceProvider
        {
            get => m_services ?? throw new InvalidOperationException(Constants.AN_UNEXPECTED_ERROR_OCCURRED);
            set => throw new InvalidOperationException("Don't even think about it 😅");
        }

        /// <summary>
        /// <see cref="Utilities.RealTimeLogTarget"/> instance
        /// </summary>
        private readonly InMemoryLoggerTarget m_loggerTarget;
        /// <summary>
        /// <see cref="IServiceProvider"/> instance
        /// </summary>
        private readonly IServiceProvider? m_services;
        /// <summary>
        /// <see cref="ILogger"/> instance
        /// </summary>
        private readonly ILogger<App>? m_logger;
        /// <summary>
        /// <see cref="ILocalData"/> instance
        /// </summary>
        private readonly ILocalData? m_localData;
        /// <summary>
        /// <see cref="MainWindow"/> instance
        /// </summary>
        private MainWindow? m_window;
        /// <summary>
        /// <see cref="LoginWindow"/> instance
        /// </summary>
        private LoginWindow? m_loginWindow;
        /// <summary>
        /// The value to indicate if the object has been disposed
        /// </summary>
        private bool m_disposedValue;

        /// <summary>
        /// Constructor
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            SubscribeToEvents();
            m_loggerTarget = new InMemoryLoggerTarget();
            m_services = ConfigureServices();
            m_logger = GetLogger();
            m_localData = GetLocalData();
        }

        /// <summary>
        /// OnLaunched
        /// </summary>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            try
            {
                m_logger?.LogInformation(Constants.APP_INITIALIZED);

                if (m_localData == null)
                {
                    Current.Exit();
                    return;
                }

                if (!string.IsNullOrEmpty(m_localData.GetString(Constants.EMAIL_LOCAL_DATA)))
                {
                    ActivateMainWindow();
                }
                else
                {
                    ActivateLoginWindow();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex);
            }
        }

        /// <summary>
        /// Reload this the page
        /// </summary>
        public void ReloadPage()
        {
            var type = new StackTrace()
                .GetFrame(1)
                ?.GetMethod()
                ?.DeclaringType;

            if (type != null && m_window != null)
            {
                m_window.NavigateTo(type);
            }
        }

        /// <summary>
        /// Configuration <see cref="IServiceProvider"/> instance
        /// </summary>
        private ServiceProvider? ConfigureServices()
        {
            try
            {
                var services = new ServiceCollection();

                services.AddLogging(BuildLogger);

                services.AddMemoryCache();

                services.AddSingleton<IWindowsHelper, WindowsHelper>();
                services.AddSingleton<ILocalization, Localization>();
                services.AddSingleton<ILocalData, LocalData>();

                services.AddSingleton<MainWindow>();
                services.AddSingleton<LoginWindow>();

                return services.BuildServiceProvider();
            }
            catch (Exception ex)
            {
                ShowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Build logger
        /// </summary>
        private void BuildLogger(ILoggingBuilder builder)
        {
            var config = new LoggingConfiguration();
#if DEBUG
            config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, m_loggerTarget);
#else
            config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, realTimeLogTarget);
#endif

            builder.AddNLog(config);
            builder.AddDebug();
#if DEBUG
            builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
#else
            builder.SetMinimumLevel(LogLevel.Warning);
#endif
            builder.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", Microsoft.Extensions.Logging.LogLevel.Error)
                .AddFilter("Microsoft.EntityFrameworkCore", Microsoft.Extensions.Logging.LogLevel.Error);
        }

        /// <summary>
        /// Get <see cref="ILogger"/> instance
        /// </summary>
        private ILogger<App>? GetLogger()
        {
            try
            {
                ArgumentNullException.ThrowIfNull(m_services, nameof(m_services));

                return m_services.GetRequiredService<ILogger<App>>();
            }
            catch (Exception ex)
            {
                ShowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Get <see cref="ILocalData"/> instance
        /// </summary>
        private ILocalData? GetLocalData()
        {
            try
            {
                ArgumentNullException.ThrowIfNull(m_services, nameof(m_services));

                return m_services.GetRequiredService<ILocalData>();
            }
            catch (Exception ex)
            {
                ShowException(ex);
                return null;
            }
        }

        /// <summary>
        /// Subscribe to events
        /// </summary>
        private void SubscribeToEvents()
        {
            this.UnhandledException += OnAppUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        }

        /// <summary>
        /// Activate the main window
        /// </summary>
        public void ActivateMainWindow()
        {
            if (m_services == null)
            {
                Current.Exit();
                return;
            }

            try
            {
                m_window = m_services!.GetService<MainWindow>();

                if (m_window == null)
                {
#if DEBUG
                    if (Debugger.IsAttached)
                    {
                        Debugger.Break();
                    }
#endif
                    throw new InvalidOperationException(Constants.MAIN_WINDOW_RESOLUTION_ERROR_MESSAGE);
                }

                m_window.Activate();

                if (m_loginWindow != null)
                {
                    m_loginWindow.Close();
                    m_loginWindow = null;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex);
            }
        }

        /// <summary>
        /// Activate the login window
        /// </summary>
        private void ActivateLoginWindow()
        {
            if (m_services == null)
            {
                Current.Exit();
                return;
            }

            try
            {
                m_loginWindow = m_services!.GetService<LoginWindow>();

                if (m_loginWindow == null)
                {
#if DEBUG
                    if (Debugger.IsAttached)
                    {
                        Debugger.Break();
                    }
#endif
                    throw new InvalidOperationException(Constants.INTRODUCTION_WINDOW_RESOLUTION_ERROR_MESSAGE);
                }

                m_loginWindow.Activate();

                if (m_window != null)
                {
                    m_window.Close();
                    m_window = null;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex);
            }
        }

        /// <summary>
        /// OnAppUnhandledException
        /// </summary>
        private void OnAppUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            m_logger?.LogCritical(
                e.Exception,
                "{AN_UNEXPECTED_ERROR_OCCURRED}{Message}",
                Constants.AN_UNEXPECTED_ERROR_OCCURRED, e.Exception.Message);

            var notification = new AppNotificationBuilder()
                .AddText("An exception was thrown.")
                .AddText($"Type: {e.Exception.GetType()}")
                .AddText($"Message: {e.Exception.Message}\r\n" +
                         $"HResult: {e.Exception.HResult}")
                .BuildNotification();

            e.Handled = true;

            AppNotificationManager.Default.Show(notification);
        }

        /// <summary>
        /// OnUnobservedTaskException
        /// </summary>
        private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            m_logger?.LogCritical(
                e.Exception,
                "{AN_UNEXPECTED_ERROR_OCCURRED}{Message}",
                Constants.AN_UNEXPECTED_ERROR_OCCURRED, e.Exception.Message);

            var notification = new AppNotificationBuilder()
                .AddText("An exception was thrown.")
                .AddText($"Type: {e.Exception.GetType()}")
                .AddText($"Message: {e.Exception.Message}\r\n" +
                         $"HResult: {e.Exception.HResult}")
                .BuildNotification();

            e.SetObserved();

            AppNotificationManager.Default.Show(notification);
        }

        /// <summary>
        /// Show exception
        /// </summary>
        private static void ShowException(string message)
        {
#if DEBUG
            Debug.WriteLine(string.Concat(Constants.AN_UNEXPECTED_ERROR_OCCURRED, " ", message));
#endif
            Alerts.ShowCriticalErrorWindow(message);
        }

        /// <summary>
        /// Show exception
        /// </summary>
        private static void ShowException(Exception ex)
        {
#if DEBUG
            Debug.WriteLine(ex);
#endif
            Alerts.ShowCriticalErrorWindow(ex);
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
                    this.UnhandledException -= OnAppUnhandledException;
                    TaskScheduler.UnobservedTaskException -= OnUnobservedTaskException;
                }

                m_disposedValue = true;
            }
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~App()
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