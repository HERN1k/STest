using System;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using STest.App.Domain.Interfaces;
using STest.App.Utilities;
using STLib.Core.Testing;

namespace STest.App.Pages.Builder
{
    /// <summary>
    /// Builder page
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class BuilderPage : Page
    {
        public ExtendedObservableCollection<Test> TestsList { get; set; }
        public ExtendedObservableCollection<CoreTask> TasksList { get; set; }

        private readonly ILocalization m_localization;
        private readonly ILocalData m_localData;
        private readonly ITestManager m_testManager;
        private readonly IXamlUIUtilities m_uiUtilities;
        private readonly ILogger<BuilderPage> m_logger;
        private readonly Storyboard m_fadeInAnimation;
        private readonly Storyboard m_fadeOutAnimation;
        private Test? m_thisTest;

        public BuilderPage()
        {
            this.InitializeComponent();
            m_localization = ServiceHelper.GetService<ILocalization>();
            m_localData = ServiceHelper.GetService<ILocalData>();
            m_testManager = ServiceHelper.GetService<ITestManager>();
            m_uiUtilities = ServiceHelper.GetService<IXamlUIUtilities>();
            m_logger = ServiceHelper.GetLogger<BuilderPage>();
            m_fadeInAnimation = m_uiUtilities.GetStoryboard(this.Resources, "FadeInAnimation");
            m_fadeOutAnimation = m_uiUtilities.GetStoryboard(this.Resources, "FadeOutAnimation");
            TestsList = new ExtendedObservableCollection<Test>();
            TasksList = new ExtendedObservableCollection<CoreTask>();
            this.DataContext = this;
        }

        /// <inheritdoc />
        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            try
            {
                base.OnNavigatedTo(args);

                ReopenTest();
                await SetTestListItemsAsync();
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }

        /// <inheritdoc />
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs args)
        {
            try
            {
                base.OnNavigatingFrom(args);
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        
        /// <summary>
        /// Get the localized string by key
        /// </summary>
        /// <param name="key"></param>
        private string T(string key) => m_localization.T(key);

        /// <summary>
        /// Retrieves the task from the data context.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataContext"></param>
        /// <returns></returns>
        public T? GetTaskFromDataContext<T>(object? dataContext) where T : CoreTask
        {
            if (dataContext == null)
            {
                return null;
            }

            if (dataContext is not CoreTask task)
            {
                return null;
            }

            return TasksList.FirstOrDefault(t => t.TaskID.Equals(task.TaskID)) as T;
        }

        /// <summary> 
        /// Sets the items in the test list.  
        /// Displays a message if the list is empty or shows the "Create New Test" button otherwise.  
        /// </summary>  
        private async Task SetTestListItemsAsync()
        {
            try
            {
                var tests = await m_testManager.GetBuilderTestsAsync();
                var now = DateTime.Now;

                TestsList.Clear();
                TestsList.AddRange(tests.OrderBy(t => Math.Abs((t.Created - now).Ticks)));

                if (TestsList.Count == 0)
                {
                    EmptyTestsListButton.Visibility = Visibility.Visible;
                }
                else
                {
                    CreateNewTestButton.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }

        /// <summary>
        /// Event handler for the TestBuilderCloseButton click event.
        /// </summary>
        /// <param name="testID"></param>
        private void OpenTestBuilder(Guid testID)
        {
            try
            {
                m_thisTest = TestsList.FirstOrDefault(t => t.TestID == testID)
                    ?? throw new ArgumentNullException(nameof(testID), $"Test not found.");

                m_uiUtilities.ExecuteAnimation(m_fadeInAnimation, TestBuilderBorder);
                TestBuilderBorder.Visibility = Visibility.Visible;

                TestsBuilderName.Text = m_thisTest.Name;
                TestsBuilderDescription.Text = m_thisTest.Description;
                TestsBuilderInstructions.Document.SetText(Microsoft.UI.Text.TextSetOptions.None, m_thisTest.Instructions);
                TestsBuilderTime.Time = m_thisTest.TestTime;
                TestCodeText.Text = m_thisTest.Code;
                TestCodeButton.Tag = m_thisTest.Code;

                TasksList.Clear();
                TasksList.AddRange(m_thisTest.Tasks);

                SetCurrentTest();
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }

        /// <summary>  
        /// Reopens the current test if it exists in the memory cache.  
        /// Adds the test to the list if the list is empty and opens the test builder.  
        /// </summary>  
        private void ReopenTest()
        {
            var test = m_testManager.GetCurrentBuilderTest();

            if (test != null)
            {
                if (!TestsList.Contains(test))
                {
                    TestsList.Add(test);
                }

                OpenTestBuilder(test.TestID);
            }
        }

        /// <summary>  
        /// Sets the current test in the memory cache.  
        /// Removes existing tasks and adds updated tasks from the task list.  
        /// </summary>  
        private void SetCurrentTest()
        {
            if (m_thisTest != null)
            {
                m_thisTest.Tasks.Clear();

                foreach (var task in TasksList)
                {
                    m_thisTest.Tasks.Add(task);
                }

                m_testManager.SetCurrentBuilderTest(m_thisTest);
            }
        }
    }
}