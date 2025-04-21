using System;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using STest.App.Domain.Interfaces;
using Windows.Graphics;
using Windows.UI;
using STest.App.Utilities;
using Microsoft.Extensions.Logging;

namespace STest.App.AppWindows
{
    /// <summary>
    /// Error window
    /// </summary>
    public sealed partial class ErrorWindow : Window, IDisposable
    {
        private readonly IWindowsHelper m_windowsHelper;
        private readonly ILogger<ErrorWindow> m_logger;
        private readonly Color m_buttonForegroundColor = Color.FromArgb(255, 255, 255, 255);
        private readonly Color m_buttonBackgroundColor = Color.FromArgb(0, 255, 255, 255);
        private string m_exceptionMessage = Constants.MESSAGE_KEY;
        private bool m_disposedValue;

        /// <summary>
        /// Constructor
        /// </summary>
        public ErrorWindow(string? message)
        {
            this.InitializeComponent();
            m_windowsHelper = ServiceHelper.GetService<IWindowsHelper>();
            m_logger = ServiceHelper.GetLogger<ErrorWindow>();
            SubscribeToEvents();
            Init(message);
        }

        /// <summary>
        /// Initialize the window
        /// </summary>
        private void Init(string? message)
        {
            try
            {
                if (!string.IsNullOrEmpty(message))
                {
                    m_exceptionMessage = message;
                }

                MessageBox.Text = m_exceptionMessage;

                if (m_windowsHelper.IsWindowsVersionAtLeast(10, 0, 19041))
                {
#pragma warning disable CA1416
                    this.Title = "Critical Error";
                    TitleBarTextBlock.Text = "Critical Error";
#pragma warning restore
                }
                else
                {
                    this.Title = "Critical Error";
                    TitleBarTextBlock.Text = "Critical Error";
                }

                if (AppWindowTitleBar.IsCustomizationSupported())
                {
                    if (ExtendsContentIntoTitleBar == true)
                    {
                        this.AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
                    }

                    this.AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;

                    this.AppWindow.TitleBar.ButtonForegroundColor = m_buttonForegroundColor;
                    this.AppWindow.TitleBar.ButtonBackgroundColor = m_buttonBackgroundColor;
                    this.AppWindow.TitleBar.ButtonHoverForegroundColor = m_buttonForegroundColor;
                    this.AppWindow.TitleBar.ButtonHoverBackgroundColor = m_buttonBackgroundColor;

                    this.AppWindow.TitleBar.InactiveForegroundColor = m_buttonForegroundColor;
                    this.AppWindow.TitleBar.InactiveBackgroundColor = m_buttonBackgroundColor;
                    this.AppWindow.TitleBar.ButtonInactiveForegroundColor = m_buttonForegroundColor;
                    this.AppWindow.TitleBar.ButtonInactiveBackgroundColor = m_buttonBackgroundColor;
                }

                this.AppWindow.Resize(new SizeInt32
                {
                    Width = 600,
                    Height = 720
                });

                m_logger?.LogInformation("Window \"{Name}\" is activate", nameof(ErrorWindow));
            }
            catch (Exception ex)
            {
                m_logger.LogError(ex, "Error initializing ErrorWindow");
            }
        }

        /// <summary>
        /// Subscribe to events
        /// </summary>
        private void SubscribeToEvents()
        {
            CloseButton.Click += OnCloseButtonClick;
        }

        /// <summary>
        /// Close button click event
        /// </summary>
        private void OnCloseButtonClick(object sender, RoutedEventArgs args) => this.Close();

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
                    CloseButton.Click -= OnCloseButtonClick;
                }

                m_disposedValue = true;
            }
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~ErrorWindow()
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