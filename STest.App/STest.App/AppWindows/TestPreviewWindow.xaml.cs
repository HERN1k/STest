using System;
using WinRT;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using STest.App.Domain.Interfaces;
using STest.App.Utilities;
using Windows.UI;
using STLib.Core.Testing;
using Microsoft.Extensions.Caching.Memory;
using STLib.Tasks.Checkboxes;
using STLib.Tasks.MultipleChoice;
using STest.App.Services;

namespace STest.App.AppWindows
{
    /// <summary>
    /// The test preview window of the application
    /// </summary>
    public sealed partial class TestPreviewWindow : Window, IDisposable
    {
        public ExtendedObservableCollection<CoreTask> TasksList { get; set; }

        private readonly ILocalization m_localization;
        private readonly IMemoryCache m_memoryCache;
        private readonly ILogger<TestPreviewWindow> m_logger;
        private readonly IWindowsHelper m_windowsHelper;
        private readonly Test m_test;
        private WindowsSystemDispatcherQueueHelper? m_wsdqHelper;
        private DesktopAcrylicController? m_acrylicController;
        private SystemBackdropConfiguration? m_configurationSource;
        private bool m_disposedValue;

        public TestPreviewWindow(ILocalization localization, IMemoryCache memoryCache, ILogger<TestPreviewWindow> logger, IWindowsHelper windowsHelper)
        {
            this.InitializeComponent();
            m_localization = localization ?? throw new ArgumentNullException(nameof(localization));
            m_memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            m_logger = logger ?? throw new ArgumentNullException(nameof(logger));
            m_windowsHelper = windowsHelper ?? throw new ArgumentNullException(nameof(windowsHelper));
            TasksList = new ExtendedObservableCollection<CoreTask>();
            m_test = GetCurrentTest();
            m_windowsHelper.ConfigureTitleBar(this);
            SubscribeToEvents();
            Init();
            TrySetAcrylicBackdrop(useAcrylicThin: false);
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
                this.Title = T(Constants.APP_DISPLAY_NAME_KEY);

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

                TestName.Text = m_test.Name;
                TestDescription.Text = m_test.Description;
                TestInstructions.Text = string.Concat(T(Constants.INSTRUCTIONS_KEY), ": ", m_test.Instructions);
                TestTimerHeader.Text = string.Concat(T(Constants.TIME_LEFT_KEY), ": ");
                TestTimer.Text = m_test.TestTime.ToString(@"hh\:mm\:ss", m_localization.CurrentCulture);

                TasksList.AddRange(m_test.SortedTasks);
            }
            catch (Exception ex)
            {
                ex.Show(m_logger);
            }
        }

        /// <summary>
        /// Event handler for the checkboxes task loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void CheckboxesTaskLoaded(object sender, RoutedEventArgs args)
        {
            if (sender is StackPanel element)
            {
                if (element.DataContext is not CheckboxesTask task)
                {
                    return;
                }

                foreach (var item in task.Answers)
                {
                    element.Children.Add(new CheckBox() { Content = item });
                }

                if (element.Children.Count == 0)
                {
                    element.Children.Add(new CheckBox() { Content = Constants.NULL });
                }
            }
        }

        /// <summary>
        /// Event handler for the multiple choice task loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void MultipleChoiceTaskLoaded(object sender, RoutedEventArgs args)
        {
            if (sender is RadioButtons element)
            {
                if (element.DataContext is not MultipleChoiceTask task)
                {
                    return;
                }

                foreach (var item in task.Answers)
                {
                    element.Items.Add(new RadioButton() { Content = item });
                }

                if (element.Items.Count == 0)
                {
                    element.Items.Add(new RadioButton() { Content = Constants.NULL });
                }
            }
        }

        /// <summary>
        /// Get the current test from the memory cache
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private Test GetCurrentTest()
        {
            try
            {
                if (m_memoryCache.TryGetValue<Test>(Constants.CURRENT_TEST_IN_BUILDER, out var test))
                {
                    if (test == null)
                    {
                        throw new InvalidOperationException("Current test is null.");
                    }

                    foreach (var task in test.Tasks)
                    {
                        task.SetName(task.Type switch
                        {
                            TaskType.Text => T(Constants.ENTER_CORRECT_ANSWER_IN_FIELD_BELOW_KEY),
                            TaskType.TrueFalse => T(Constants.INDICATE_BELOW_WHETHER_YOU_AGREE_WITH_STATEMENT_ABOVE_KEY),
                            TaskType.Checkboxes => T(Constants.CHOOSE_SEVERAL_CORRECT_ANSWERS_KEY),
                            TaskType.MultipleChoice => T(Constants.CHOOSE_ONLY_ONE_CORRECT_ANSWER_KEY),
                            _ => string.Concat(Constants.NULL, " :(")
                        });
                    }

                    return test;
                }
            }
            catch (Exception ex)
            {
                ex.Show(m_logger);
            }

            throw new InvalidOperationException("Current test is null.");
        }

        /// <summary>
        /// Get the localized string by key
        /// </summary>
        /// <param name="key"></param>
        private string T(string key) => m_localization.T(key);

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
        /// Set the configuration source theme
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
        /// Window closed event
        /// </summary>
        private void OnWindowClosed(object sender, WindowEventArgs args)
        {
            try
            {
                if (m_acrylicController != null)
                {
                    m_acrylicController.Dispose();
                    m_acrylicController = null;
                }
                this.Activated -= OnWindowActivated;
                m_configurationSource = null;
            }
            catch (Exception ex)
            {
                ex.Show(m_logger);
            }
        }

        /// <summary>
        /// Set the configuration source theme
        /// </summary>
        private void SetConfigurationSourceTheme()
        {
            try
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
            catch (Exception ex)
            {
                ex.Show(m_logger);
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
        ~TestPreviewWindow()
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