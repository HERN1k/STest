using System;
using System.Collections.ObjectModel;
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
using STLib.Tasks.Checkboxes;
using STLib.Tasks.MultipleChoice;
using STLib.Tasks.Text;
using STLib.Tasks.TrueFalse;

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
        private readonly IMemoryCache m_memoryCache;
        private readonly ILogger<BuilderPage> m_logger;
        private readonly Storyboard m_fadeInAnimation;
        private readonly Storyboard m_fadeOutAnimation;
        private Test? m_thisTest;

        private Guid m_temp;

        public BuilderPage()
        {
            this.InitializeComponent();
            m_localization = ServiceHelper.GetService<ILocalization>();
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

        private void SetTestListItems()
        {
            try
            {
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

        private Test? GetCurrentTest()
        {
            if (m_memoryCache.TryGetValue<Test>(Constants.CURRENT_TEST_IN_BUILDER, out var test))
            {
                return test;
            }

            return null;
        }

        private void SetCurrentTest()
        {
            if (m_thisTest != null)
            {
                m_thisTest.RemoveTasks(m_thisTest.Tasks.Select(task => task.TaskID));

                m_thisTest.AddTasks(TasksList);

                m_memoryCache.Set<Test>(Constants.CURRENT_TEST_IN_BUILDER, m_thisTest, TimeSpan.FromMinutes(15));
            }
        }

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