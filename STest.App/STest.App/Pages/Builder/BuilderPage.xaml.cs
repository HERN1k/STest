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

namespace STest.App.Pages.Builder
{
    /// <summary>
    /// Builder page
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class BuilderPage : Page
    {
        public ObservableCollection<Test> TestsList { get; set; }

        private readonly ILocalization m_localization;
        private readonly ILogger<HomePage> m_logger;
        private readonly Storyboard m_fadeInAnimation;
        private readonly Storyboard m_fadeOutAnimation;

        public BuilderPage()
        {
            this.InitializeComponent();
            m_localization = ServiceHelper.GetService<ILocalization>();
            m_logger = ServiceHelper.GetLogger<HomePage>();
            m_fadeInAnimation = GetStoryboard("FadeInAnimation");
            m_fadeOutAnimation = GetStoryboard("FadeOutAnimation");
            TestsList = new ObservableCollection<Test>();
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

        private void OpenTestBuilder(Guid testID)
        {
            var test = TestsList.FirstOrDefault(t => t.TestID == testID)
                ?? throw new Exception($"Test not found.");

            ExecuteAnimation(m_fadeInAnimation, TestBuilderBorder);
            TestBuilderBorder.Visibility = Visibility.Visible;

            TestsBuilderName.Header = CreateHeader(Constants.NAME_KEY);
            TestsBuilderName.Text = test.Name;

            TestsBuilderDescription.Header = CreateHeader(Constants.DESCRIPTION_KEY);
            TestsBuilderDescription.Text = test.Description;

            TestsBuilderInstructions.Header = CreateHeader(Constants.INSTRUCTIONS_KEY);
            TestsBuilderInstructions.Document.SetText(
                Microsoft.UI.Text.TextSetOptions.None, test.Instructions);

            TestsBuilderTime.Header = CreateHeader(Constants.TEST_TIME_KEY);
            TestsBuilderTime.Time = test.TestTime;
        }

        private TextBlock CreateHeader(string localizationKey)
        {
            return new TextBlock()
            {
                Text = m_localization.GetString(localizationKey),
                Style = (Style)Application.Current.Resources["SubtitleTextBlockStyle"],
                FontFamily = (FontFamily)Application.Current.Resources["Montserrat"],
                Margin = new Thickness(2, 0, 0, 0),
            };
        }
    }
}