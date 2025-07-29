using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using STest.App.Controls;
using STest.App.Domain.Interfaces;
using STest.App.Utilities;
using STLib.Core.Entities;
using STLib.Core.Testing;
using STLib.Queries;                // Don't remove 
using STLib.Tasks.Checkboxes;
using STLib.Tasks.MultipleChoice;        

namespace STest.App.Pages.Assessments
{
    public sealed partial class AssessmentsPage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ExtendedObservableCollection<Test> TestsList { get; set; }
        public ExtendedObservableCollection<User> StudentsList { get; set; }
        public ExtendedObservableCollection<Attention> AttentionsList { get; set; }
        public ExtendedObservableCollection<CoreTask> TasksList { get; set; }
        public Visibility IsTestVisible
        {
            get => m_isTestVisible;
            set
            {
                if (m_isTestVisible != value)
                {
                    m_isTestVisible = value;
                    OnPropertyChanged(nameof(IsTestVisible));
                }
            }
        }

        private readonly ILocalization m_localization;
        private readonly ILocalData m_localData;
        private readonly ITestManager m_testManager;
        private readonly ILogger<AssessmentsPage> m_logger;
        private Test? m_currentTest = null;
        private User? m_currentStudent = null;
        private DateTimeOffset m_from = DateTimeOffset.UtcNow - TimeSpan.FromDays(7);
        private DateTimeOffset m_to = DateTimeOffset.UtcNow;
        private Visibility m_isTestVisible = Visibility.Collapsed;

        public AssessmentsPage()
        {
            this.InitializeComponent();
            m_localization = ServiceHelper.GetService<ILocalization>();
            m_localData = ServiceHelper.GetService<ILocalData>();
            m_testManager = ServiceHelper.GetService<ITestManager>();
            m_logger = ServiceHelper.GetLogger<AssessmentsPage>();
            TestsList = new ExtendedObservableCollection<Test>();
            StudentsList = new ExtendedObservableCollection<User>();
            AttentionsList = new ExtendedObservableCollection<Attention>();
            TasksList = new ExtendedObservableCollection<CoreTask>();
            FromDateElement.Date = m_from;
            ToDateElement.Date = m_to;
            this.DataContext = this;
        }

        /// <inheritdoc />
        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            try
            {
                base.OnNavigatedTo(args);

                await SetTestsList();
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
        /// FromDateElementDateChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void FromDateElementDateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if (args.NewDate.HasValue)
            {
                try
                {
                    DateTimeOffset value = args.NewDate.Value;

                    if (m_from == args.NewDate.Value)
                    {
                        return;
                    }

                    if (value > m_to)
                    {
                        if (args.OldDate.HasValue)
                        {
                            sender.Date = args.OldDate.Value;
                        }

                        return;
                    }

                    m_from = value;

                    CloseTest();
                    m_currentTest = null;
                    m_currentStudent = null;
                    StudentsList.Clear();
                }
                catch (Exception ex)
                {
                    ex.Show(m_logger);
                }
            }
        }

        /// <summary>
        /// ToDateElementDateChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ToDateElementDateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if (args.NewDate.HasValue)
            {
                try
                {
                    DateTimeOffset value = args.NewDate.Value;

                    if (m_to == args.NewDate.Value)
                    {
                        return;
                    }

                    if (value < m_from)
                    {
                        if (args.OldDate.HasValue)
                        {
                            sender.Date = args.OldDate.Value;
                        }

                        return;
                    }

                    m_to = value;

                    CloseTest();
                    m_currentTest = null;
                    m_currentStudent = null;
                    StudentsList.Clear();
                }
                catch (Exception ex)
                {
                    ex.Show(m_logger);
                }
            }
        }

        /// <summary>
        /// Event handler for the LoadStudentsButton click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void LoadStudentsButtonClick(object sender, RoutedEventArgs args)
        {
            try
            {
                var testId = Guid.Parse($"{(sender as ConfirmationButton)?.Tag}");

                if (testId == Guid.Empty)
                {
                    return;
                }

                var test = (await m_testManager
                    .GetBuilderTestsAsync())
                    .FirstOrDefault(t => t.TestID == testId);

                if (test == null)
                {
                    return;
                }

                CloseTest();

                List<User>? users = null;
#if DEBUG
                User[] tempUsers = [
                    new User(firstName: "John", lastName: "Doe", middleName: "Michael"),
                    new User(firstName: "Олена", lastName: "Шевченко", middleName: "Іванівна"),
                    new User(firstName: "Carlos", lastName: "Ramirez", middleName: "Antonio"),
                    new User(firstName: "Anna", lastName: "Kowalska", middleName: "Maria"),
                    new User(firstName: "Иван", lastName: "Петров", middleName: "Сергеевич"),
                    new User(firstName: "Liu", lastName: "Wei", middleName: "Jing"),
                    new User(firstName: "Sarah", lastName: "Connor", middleName: "Jane"),
                    new User(firstName: "Ahmed", lastName: "Hassan", middleName: "Omar"),
                    new User(firstName: "Наталя", lastName: "Коваль", middleName: "Олександрівна"),
                    new User(firstName: "Tom", lastName: "Henderson", middleName: "Lee")
                ];

                Random.Shared.Shuffle(tempUsers);

                users = new List<User>(tempUsers);
#else
                users = await new QueryBuilder()
                    .Assessments
                    .UsersWhoAnsweredTest
                    .SetTest(test)
                    .SetFromDate(m_from)
                    .SetToDate(m_to)
                    .ExecuteAsync();
#endif
                if (users == null)
                {
                    return;
                }

                m_currentTest = test;

                StudentsList.Clear();
                StudentsList.AddRange(users);

                if (StudentsList.Count > 0)
                {
                    StudentsListElement.SelectedIndex = 0;
                    m_currentStudent = StudentsList[0];
                }
            }
            catch (Exception ex)
            {
                ex.Show(m_logger);
            }
        }

        /// <summary>
        /// Event handler for the LoadTestButton click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        #pragma warning disable CS1998
        private async void LoadTestButtonClick(object sender, RoutedEventArgs args)
        #pragma warning restore
        {
            if (m_currentTest == null || m_currentStudent == null)
            {
                return;
            }

            try
            {
                if (!Guid.TryParse(m_localData.GetString(Constants.USER_ID_LOCAL_DATA), out Guid teacherID))
                {
                    return;
                }

                Test? test = null;
#if DEBUG
                test = m_testManager.Deserialize(
                    m_localData.GetString(Constants.DEV_PASSED_TEST)
                );
#else
                test = await new QueryBuilder()
                    .Assessments
                    .SinglePassedTest
                    .SetUserID(m_currentStudent.ID)
                    .SetTestID(m_currentTest.TestID)
                    .SetTeacherID(teacherID)
                    .ExecuteAsync();
#endif
                OpenTest(test);
            }
            catch (Exception ex)
            {
                ex.Show(m_logger);
            }
        }

        /// <summary>
        /// Prepares and displays the details of a test for the current student.
        /// </summary>
        private void OpenTest(Test? test = null)
        {
            try
            {
                if (test != null)
                {
                    m_currentTest = test;
                }

                ArgumentNullException.ThrowIfNull(m_currentTest);
                ArgumentNullException.ThrowIfNull(m_currentStudent);

                bool isTestCompletedOnTime = (m_currentTest.EndTime - m_currentTest.StartTime) < m_currentTest.TestTime;

                TestNameElement.Text = string.Concat(T(Constants.TEST_NAME_KEY), ": ", m_currentTest.Name);
                UserNameElement.Text = string.Concat(T(Constants.STUDENT_NAME_KEY), ": ", m_currentStudent.FullName);
                TestCompletedOnTimeElement.Text = string.Concat(T(Constants.TEST_COMPLETED_ON_TIME_KEY), ": ");
                TestCompletedOnTimeAnsverElement.Text = isTestCompletedOnTime
                    ? T(Constants.YES_KEY)
                    : T(Constants.NO_KEY);
                TestCompletedOnTimeAnsverElement.Foreground = isTestCompletedOnTime
                    ? new SolidColorBrush(Colors.Green)
                    : new SolidColorBrush(Colors.Red);
                TestEndTimeElement.Text = string.Concat(
                    T(Constants.END_TIME_KEY),
                    ": ",
                    m_currentTest.EndTime.ToLocalTime().ToString("d MMMM yyyy HH:mm:ss")
                );
                GradeElement.Text = string.Concat(T(Constants.GRADE_KEY), ": ", m_currentTest.Grade.ToString("0.##"));
                AttentionElement.Text = string.Concat(T(Constants.ATTENTION_KEY), "!");
                AttentionsList.AddRange(m_currentTest.Attentions);

                if (m_currentTest.Tasks.Count > 0)
                {
                    List<CoreTask> firstTasks = new();
                    List<CoreTask> tasks = new();

                    foreach (var task in m_currentTest.Tasks)
                    {
                        task.SetName(T(Constants.TOTAL_POINTS_KEY));

                        if (task.Consider)
                        {
                            tasks.Add(task);
                        }
                        else
                        {
                            firstTasks.Add(task);
                        }
                    }

                    firstTasks.AddRange(tasks);

                    TasksList.AddRange(firstTasks);
                }

                IsTestVisible = Visibility.Visible;
            }
            catch (Exception ex)
            {
                ex.Show(m_logger);
            }
        }

        /// <summary>
        /// Closes the current test and resets related state.
        /// </summary>
        private void CloseTest()
        {
            try
            {
                IsTestVisible = Visibility.Collapsed;
                AttentionsList.Clear();
                TasksList.Clear();
                m_currentStudent = null;
                m_currentTest = null;
            }
            catch (Exception ex)
            {
                ex.Show(m_logger);
            }
        }

        /// <summary>
        /// Asynchronously retrieves and updates the list of tests, ordered by their creation time relative to the
        /// current time.
        /// </summary>
        private async Task SetTestsList()
        {
            try
            {
                var tests = await m_testManager.GetBuilderTestsAsync();
                var now = DateTime.Now;

                TestsList.Clear();
                TestsList.AddRange(tests.OrderBy(t => Math.Abs((t.Created - now).Ticks)));
            }
            catch (Exception ex)
            {
                ex.Show(m_logger);
            }
        }

        /// <summary>
        /// Handles the loading of a <see cref="CheckboxesTask"/> into a <see cref="StackPanel"/> by dynamically
        /// creating and adding <see cref="CheckBox"/> elements for each answer in the task.
        /// </summary>
        private void CheckboxesTaskLoaded(object sender, RoutedEventArgs args)
        {
            if (sender is StackPanel element)
            {
                if (element.DataContext is not CheckboxesTask task)
                {
                    return;
                }

                var answers = JsonSerializer.Deserialize<List<string>>(task.Answer) ?? new List<string>();

                foreach (var item in task.Answers)
                {
                    var checkBox = new CheckBox()
                    {
                        Content = item
                    };

                    if (answers.Contains(item))
                    {
                        checkBox.IsChecked = true;
                    }

                    element.Children.Add(checkBox);
                }

                if (element.Children.Count == 0)
                {
                    element.Children.Add(new CheckBox() { Content = Constants.NO_ANSWER_KEY });
                }
            }
        }

        /// <summary>
        /// Handles the loading of a multiple-choice task by populating a <see cref="RadioButtons"/> control with the
        /// available answers from the task's data context.
        /// </summary>
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
                    var button = new RadioButton() { Content = item };

                    if (item.Equals(task.Answer))
                    {
                        button.IsChecked = true;
                    }

                    element.Items.Add(button);
                }

                if (element.Items.Count == 0 || string.IsNullOrEmpty(task.Answer))
                {
                    element.Items.Add(new RadioButton() { Content = Constants.NO_ANSWER_KEY });
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event to notify listeners that a property value has changed.
        /// </summary>
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}