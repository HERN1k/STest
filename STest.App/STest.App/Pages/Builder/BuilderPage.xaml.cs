using System;
using System.Linq;
using System.Runtime.Versioning;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly IMemoryCache m_memoryCache;
        private readonly ILogger<BuilderPage> m_logger;
        private readonly Storyboard m_fadeInAnimation;
        private readonly Storyboard m_fadeOutAnimation;
        private Test? m_thisTest;

        public BuilderPage()
        {
            this.InitializeComponent();
            m_localization = ServiceHelper.GetService<ILocalization>();
            m_localData = ServiceHelper.GetService<ILocalData>();
            m_memoryCache = ServiceHelper.GetMemoryCache();
            m_logger = ServiceHelper.GetLogger<BuilderPage>();
            m_fadeInAnimation = GetStoryboard("FadeInAnimation");
            m_fadeOutAnimation = GetStoryboard("FadeOutAnimation");
            TestsList = new ExtendedObservableCollection<Test>();
            TasksList = new ExtendedObservableCollection<CoreTask>();
            this.DataContext = this;
        }

        #region OnNavigated
        /// <summary>
        /// OnNavigatedTo
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            try
            {
                base.OnNavigatedTo(args);

                SubscribeToEvents();
                TestsBuilderTitle.Text = m_localization.GetString(Constants.EDITOR_KEY);
                SaveButtonText.Text = m_localization.GetString(Constants.SAVE_KEY);
                SendButtonText.Text = m_localization.GetString(Constants.SEND_KEY);
                AddNewTaskButtonText.Text = m_localization.GetString(Constants.ADD_NEW_TASK_KEY);
                TextTaskFlyout.Text = m_localization.GetString(Constants.TEXT_KEY);
                TrueFalseTaskFlyout.Text = m_localization.GetString(Constants.TRUE_FALSE_KEY);
                CheckboxesTaskFlyout.Text = m_localization.GetString(Constants.CHECKBOXES_KEY);
                MultipleChoiceTaskFlyout.Text = m_localization.GetString(Constants.MULTIPLE_CHOICE_KEY);
                ReopenTest();
                SetTestListItems();
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }

        /// <summary>
        /// OnNavigatingFrom
        /// </summary>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs args)
        {
            try
            {
                base.OnNavigatingFrom(args);

                UnSubscribeToEvents();
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        #endregion

        /// <summary>
        /// Subscribe to events
        /// </summary>
        private void SubscribeToEvents()
        {
            TasksList.CollectionChanged += TasksCollectionChanged;
        }

        /// <summary>
        /// Un subscribe to events
        /// </summary>
        private void UnSubscribeToEvents()
        {
            TasksList.CollectionChanged -= TasksCollectionChanged;
        }

        /// <summary> 
        /// Sets the items in the test list.  
        /// Displays a message if the list is empty or shows the "Create New Test" button otherwise.  
        /// </summary>  
        private void SetTestListItems()
        {
            try
            {
                // TODO: Get tests from server  

                if (TestsList.Count == 0)
                {
                    EmptyTestsListTitle.Text = m_localization.GetString(Constants.CREATE_NEW_TEST_KEY);
                    EmptyTestsListDescription.Text = m_localization.GetString(Constants.LIST_OF_RECENT_TESTS_IS_EMPTY_KEY);
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
        /// Reopens the current test if it exists in the memory cache.  
        /// Adds the test to the list if the list is empty and opens the test builder.  
        /// </summary>  
        private void ReopenTest()
        {
            var test = GetCurrentTest();

            if (test != null)
            {
                if (TestsList.Count == 0)
                {
                    TestsList.Add(test);
                }

                OpenTestBuilder(test.TestID);
            }
        }

        /// <summary>  
        /// Retrieves the current test from the memory cache.  
        /// Returns null if no test is found.  
        /// </summary>  
        /// <returns>The current test or null if not found.</returns>  
        private Test? GetCurrentTest()
        {
            if (m_memoryCache.TryGetValue<Test>(Constants.CURRENT_TEST_IN_BUILDER, out var test))
            {
                return test;
            }

            return null;
        }

        /// <summary>  
        /// Sets the current test in the memory cache.  
        /// Removes existing tasks and adds updated tasks from the task list.  
        /// </summary>  
        private void SetCurrentTest()
        {
            if (m_thisTest != null)
            {
                m_thisTest.RemoveTasks(m_thisTest.Tasks.Select(task => task.TaskID));

                m_thisTest.AddTasks(TasksList);

                m_memoryCache.Set<Test>(Constants.CURRENT_TEST_IN_BUILDER, m_thisTest, TimeSpan.FromMinutes(15));
            }
        }

        /// <summary>  
        /// Creates a header TextBlock with the specified localization key.  
        /// The text is aligned to the left by default.  
        /// </summary>  
        /// <param name="localizationKey">The key for localized text.</param>  
        /// <returns>A TextBlock with the localized text.</returns>  
        private TextBlock CreateHeader(string localizationKey)
        {
            return new TextBlock()
            {
                Text = m_localization.GetString(localizationKey),
                Style = (Style)Application.Current.Resources["SubtitleTextBlockStyle"],
                Margin = new Thickness(2, 0, 0, 0),
                TextAlignment = TextAlignment.Left
            };
        }

        /// <summary>  
        /// Creates a header TextBlock with the specified localization key and text alignment.  
        /// </summary>  
        /// <param name="localizationKey">The key for localized text.</param>  
        /// <param name="alignment">The text alignment for the header.</param>  
        /// <returns>A TextBlock with the localized text and specified alignment.</returns>  
        private TextBlock CreateHeader(string localizationKey, TextAlignment alignment)
        {
            return new TextBlock()
            {
                Text = m_localization.GetString(localizationKey),
                Style = (Style)Application.Current.Resources["SubtitleTextBlockStyle"],
                Margin = new Thickness(2, 0, 0, 0),
                TextAlignment = alignment
            };
        }

        /// <summary>  
        /// Retrieves a storyboard resource by its name.  
        /// Throws an exception if the resource is not found or is not a storyboard.  
        /// </summary>  
        /// <param name="name">The name of the storyboard resource.</param>  
        /// <returns>The storyboard resource.</returns>  
        /// <exception cref="InvalidOperationException">Thrown if the storyboard is not found.</exception>  
        /// <exception cref="InvalidCastException">Thrown if the resource is not a storyboard.</exception>  
        private Storyboard GetStoryboard(string name)
        {
            if (this.Resources.TryGetValue(name, out var storyboard))
            {
                return storyboard as Storyboard
                    ?? throw new InvalidCastException($"Type \"{name}\" is not storyboard.");
            }
            else
            {
                throw new InvalidOperationException($"Storyboard with name \"{name}\" not found in resources.");
            }
        }

        /// <summary>  
        /// Executes the specified storyboard animation on a UI element.  
        /// Stops the storyboard if it is already running and sets the target element for each animation.  
        /// </summary>  
        /// <param name="storyboard">The storyboard to execute.</param>  
        /// <param name="element">The UI element to animate.</param>  
        private void ExecuteAnimation(Storyboard storyboard, UIElement element)
        {
            if (storyboard == null || element == null)
            {
                return;
            }

            try
            {
                storyboard.Stop();

                for (int i = 0; i < storyboard.Children.Count; i++)
                {
                    Storyboard.SetTarget(storyboard.Children[i], element);
                }

                storyboard.Begin();
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }

        /// <summary>  
        /// Filters a FrameworkElement by its tag.  
        /// Returns true if the element's tag matches the specified tag.  
        /// </summary>  
        /// <param name="element">The element to filter.</param>  
        /// <param name="tag">The tag to match.</param>  
        /// <returns>True if the tag matches; otherwise, false.</returns>  
        private static bool FilterByTag(FrameworkElement element, string tag)
        {
            if (element == null || string.IsNullOrEmpty(tag))
            {
                return false;
            }

            if (element.Tag is not string elementTag)
            {
                return false;
            }

            return elementTag.Equals(tag, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}