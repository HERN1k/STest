using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using STest.App.Domain.Interfaces;
using STest.App.Pages.Home;
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
        public ObservableCollection<Test> TestsList { get; set; }
        public ObservableCollection<CoreTask> TasksList { get; set; }

        private readonly ILocalization m_localization;
        private readonly ILogger<HomePage> m_logger;
        private readonly Storyboard m_fadeInAnimation;
        private readonly Storyboard m_fadeOutAnimation;
        private Test? m_thisTest;

        public BuilderPage()
        {
            this.InitializeComponent();
            m_localization = ServiceHelper.GetService<ILocalization>();
            m_logger = ServiceHelper.GetLogger<HomePage>();
            m_fadeInAnimation = GetStoryboard("FadeInAnimation");
            m_fadeOutAnimation = GetStoryboard("FadeOutAnimation");
            TestsList = new ObservableCollection<Test>();
            TasksList = new ObservableCollection<CoreTask>();
            this.DataContext = this;
        }

        #region OnNavigated
        /// <summary>
        /// OnNavigatedTo
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                base.OnNavigatedTo(e);

                SubscribeToEvents();
                SetTestListItems();
                TestsBuilderTitle.Text = m_localization.GetString(Constants.EDITOR_KEY);
                SaveButtonText.Text = m_localization.GetString(Constants.SAVE_KEY);
                SendButtonText.Text = m_localization.GetString(Constants.SEND_KEY);
                AddNewTaskButtonText.Text = m_localization.GetString(Constants.ADD_NEW_TASK_KEY);
                TextTaskFlyout.Text = m_localization.GetString(Constants.TEXT_KEY);
                TrueFalseTaskFlyout.Text = m_localization.GetString(Constants.TRUE_FALSE_KEY);
                CheckboxesTaskFlyout.Text = m_localization.GetString(Constants.CHECKBOXES_KEY);
                MultipleChoiceTaskFlyout.Text = m_localization.GetString(Constants.MULTIPLE_CHOICE_KEY);
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }

        /// <summary>
        /// OnNavigatingFrom
        /// </summary>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            try
            {
                base.OnNavigatingFrom(e);

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

        }

        /// <summary>
        /// Un subscribe to events
        /// </summary>
        private void UnSubscribeToEvents()
        {

        }

        private async void EmptyTestsListButtonClick(object sender, RoutedEventArgs args)
        {
            await this.ExecuteOnDispatcherQueueAsync(m_logger, async () =>
            {
                var test = Test.Build(Guid.NewGuid()) // USE USER ID
                    .AddName(m_localization.GetString(Constants.THIS_SHOULD_BE_THE_NAME_KEY))
                    .AddDescription(m_localization.GetString(Constants.THIS_SHOULD_BE_THE_DESCRIPTION_KEY))
                    .AddInstructions(m_localization.GetString(Constants.THIS_SHOULD_BE_THE_INSTRUCTIONS_KEY))
                    .AddTestTime(TimeSpan.FromHours(1));

                await Task.Delay(750);

                EmptyTestsListButton.Visibility = Visibility.Collapsed;

                TestsList.Add(test);

                OpenTestBuilder(test.TestID);
            });
        }

        #region Builder events
        private void OpenTestBuilder(Guid testID)
        {
            m_thisTest = TestsList.FirstOrDefault(t => t.TestID == testID);

            if (m_thisTest == null)
            {
                throw new ArgumentNullException(nameof(testID), $"Test not found.");
            }

            ExecuteAnimation(m_fadeInAnimation, TestBuilderBorder);
            TestBuilderBorder.Visibility = Visibility.Visible;

            TestsBuilderName.Header = CreateHeader(Constants.NAME_KEY);
            TestsBuilderName.Text = m_thisTest.Name;

            TestsBuilderDescription.Header = CreateHeader(Constants.DESCRIPTION_KEY);
            TestsBuilderDescription.Text = m_thisTest.Description;

            TestsBuilderInstructions.Header = CreateHeader(Constants.INSTRUCTIONS_KEY);
            TestsBuilderInstructions.Document.SetText(
                Microsoft.UI.Text.TextSetOptions.None, m_thisTest.Instructions);

            TestsBuilderTime.Header = CreateHeader(Constants.TEST_TIME_KEY);
            TestsBuilderTime.Time = m_thisTest.TestTime;
        }

        private void TestNameChanged(object sender, TextChangedEventArgs args)
        {
            ArgumentNullException.ThrowIfNull(m_thisTest, nameof(m_thisTest));

            try
            {
                if (sender is TextBox element)
                {
                    if (m_thisTest.Name.Equals(element.Text))
                    {
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(element.Text))
                    {
                        return;
                    }

                    m_thisTest.AddName(element.Text);
                }
                else
                {
                    throw new ArgumentNullException(nameof(sender), $"Sender is not a TextBox.");
                }
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }

        private void TestDescriptionChanged(object sender, TextChangedEventArgs args)
        {
            ArgumentNullException.ThrowIfNull(m_thisTest, nameof(m_thisTest));

            try
            {
                if (sender is TextBox element)
                {
                    if (m_thisTest.Description.Equals(element.Text))
                    {
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(element.Text))
                    {
                        return;
                    }

                    m_thisTest.AddDescription(element.Text);
                }
                else
                {
                    throw new ArgumentNullException(nameof(sender), $"Sender is not a TextBox.");
                }
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }

        private void TestInstructionsChanged(object sender, RoutedEventArgs args)
        {
            ArgumentNullException.ThrowIfNull(m_thisTest, nameof(m_thisTest));

            try
            {
                if (sender is RichEditBox element)
                {
                    element.Document.GetText(Microsoft.UI.Text.TextGetOptions.None, out var elementText);

                    elementText = elementText.Remove(elementText.Length - 1);

                    if (m_thisTest.Instructions.Equals(elementText))
                    {
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(elementText))
                    {
                        return;
                    }

                    m_thisTest.AddInstructions(elementText);
                }
                else
                {
                    throw new ArgumentNullException(nameof(sender), $"Sender is not a RichEditBox.");
                }
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }

        private void TestTestTimeChanged(object sender, TimePickerValueChangedEventArgs args)
        {
            ArgumentNullException.ThrowIfNull(m_thisTest, nameof(m_thisTest));

            try
            {
                if (sender is TimePicker element)
                {
                    if (m_thisTest.TestTime.Equals(element.Time))
                    {
                        return;
                    }

                    if (element.Time == TimeSpan.Zero)
                    {
                        return;
                    }

                    m_thisTest.AddTestTime(element.Time);
                }
                else
                {
                    throw new ArgumentNullException(nameof(sender), $"Sender is not a TimePicker.");
                }
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }

        private void AddNewTaskClick(object sender, RoutedEventArgs args)
        {
            if (sender is MenuFlyoutItem element)
            {
                var tag = element.Tag.ToString();

                if (string.IsNullOrEmpty(tag))
                {
                    return;
                }

                if (!Enum.TryParse<TaskType>(tag, true, out var type))
                {
                    return;
                }

                switch (type)
                {
                    case TaskType.Text:
                        TasksList.Add(TextTask.Build().AddName("Text"));
                        break;
                    case TaskType.TrueFalse:
                        TasksList.Add(TrueFalseTask.Build().AddName("TrueFalse"));
                        break;
                    case TaskType.Checkboxes:
                        TasksList.Add(CheckboxesTask.Build().AddName("Checkboxes"));
                        break;
                    case TaskType.MultipleChoice:
                        TasksList.Add(MultipleChoiceTask.Build().AddName("MultipleChoice"));
                        break;
                }
            }
        }
        #endregion

        private void SetTestListItems()
        {
            var test = Test.Build(Guid.NewGuid())
                .AddName("Base name for test, is simply dummy text of the printing")
                .AddDescription("Lorem Ipsum is simply dummy text of the printing and typesetting industry.")
                .AddInstructions("Instructions")
                .AddTestTime(TimeSpan.FromMinutes(30));

            for (int i = 0; i < 10; i++)
            {
                TestsList.Add(test);
            }

            TestsList.Clear();

            if (TestsList.Count == 0)
            {
                EmptyTestsListTitle.Text = m_localization.GetString(Constants.CREATE_NEW_TEST_KEY);
                EmptyTestsListDescription.Text = m_localization.GetString(Constants.LIST_OF_RECENT_TESTS_IS_EMPTY_KEY);
                EmptyTestsListButton.Visibility = Visibility.Visible;
            }
        }

        private TextBlock CreateHeader(string localizationKey)
        {
            return new TextBlock()
            {
                Text = m_localization.GetString(localizationKey),
                Style = (Style)Application.Current.Resources["SubtitleTextBlockStyle"],
                Margin = new Thickness(2, 0, 0, 0),
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

        private static void ExecuteAnimation(Storyboard storyboard, UIElement element)
        {
            if (storyboard == null || element == null)
            {
                return;
            }

            for (int i = 0; i < storyboard.Children.Count; i++)
            {
                Storyboard.SetTarget(storyboard.Children[i], element);
            }

            storyboard.Begin();
        }
    }
}