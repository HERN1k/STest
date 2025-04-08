using System;
using STest.App.Domain.Interfaces;
using STest.App.Utilities;
using System.Collections.Generic;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Controls;
using STLib.Tasks.Text;
using System.IO;
using System.Text.Json;
using STLib.Core;
using STLib.Tasks.TrueFalse;
using STLib.Tasks.MultipleChoice;
using STLib.Tasks.Checkboxes;

namespace STest.App.Pages.Home
{
    /// <summary>
    /// Home page
    /// </summary>
    public sealed partial class HomePage : Page
    {
        /// <summary>
        /// <see cref="ILocalization"/> instance
        /// </summary>
        private readonly ILocalization m_localization;
        /// <summary>
        /// <see cref="ILogger"/> instance
        /// </summary>
        private readonly ILogger<HomePage> m_logger;

        /// <summary>
        /// Constructor
        /// </summary>
        public HomePage()
        {
            this.InitializeComponent();
            m_localization = ServiceHelper.GetService<ILocalization>();
            m_logger = ServiceHelper.GetLogger<HomePage>();
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

                var list = new List<CoreTask>
                {
                    TextTask.Build()
                        .AddName("Name")
                        .AddQuestion("Question?")
                        .AddCorrectAnswer("Correct answer"),

                    TrueFalseTask.Build()
                        .AddName("Name")
                        .AddQuestion("Question?")
                        .AddCorrectAnswer("True"),

                    MultipleChoiceTask.Build()
                        .AddName("Name")
                        .AddQuestion("Question?")
                        .AddAnswerItem("Answer 1")
                        .AddAnswerItem("Answer 2")
                        .AddAnswerItem("Answer 3")
                        .AddAnswerItem("Answer 4")
                        .AddCorrectAnswer("Answer 2"),

                    CheckboxesTask.Build()
                        .AddName("Name")
                        .AddQuestion("Question?")
                        .AddAnswerItem("Answer 1")
                        .AddAnswerItem("Answer 2")
                        .AddAnswerItem("Answer 3")
                        .AddAnswerItem("Answer 4")
                        .AddCorrectAnswers(new List<string>() 
                        {
                            "Answer 2",
                            "Answer 4"
                        }),
                };

                var test = Test.Build(Guid.NewGuid())
                    .AddName("Test")
                    .AddDescription("Description")
                    .AddInstructions("Instructions")
                    .AddTestTime(TimeSpan.FromMinutes(30))
                    .AddSubject(Guid.NewGuid())
                    .AddSubject(Guid.NewGuid())
                    .AddSubject(Guid.NewGuid())
                    .AddTasks(list);

                m_logger.LogInformation("Test: {test}", string.Concat(Environment.NewLine, test.SerializeToJson()));

                // Исправить типизацию List<CoreTask> Tasks в Test для сериализации
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
    }
}