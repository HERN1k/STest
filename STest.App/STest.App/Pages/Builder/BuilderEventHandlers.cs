using Microsoft.UI.Xaml;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System;
using Microsoft.UI.Xaml.Controls;
using STest.App.Utilities;
using STLib.Core.Testing;
using STLib.Tasks.Checkboxes;
using STLib.Tasks.MultipleChoice;
using STLib.Tasks.Text;
using STLib.Tasks.TrueFalse;
using System.Threading.Tasks;

namespace STest.App.Pages.Builder
{
    public sealed partial class BuilderPage : Page
    {
        #region Main events
        /// <summary>
        /// Event handler for the EmptyTestsListButton click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <exception cref="ApplicationException"></exception>
        private async void EmptyTestsListButtonClick(object sender, RoutedEventArgs args)
        {
            await this.ExecuteOnDispatcherQueueAsync(m_logger, async () =>
            {
                if (!Guid.TryParse(m_localData.GetString(Constants.USER_ID_LOCAL_DATA), out Guid userID))
                {
                    throw new ApplicationException($"User ID not found.");
                }
                
                var test = Test.Build(userID)
                    .AddName(T(Constants.THIS_SHOULD_BE_THE_NAME_KEY))
                    .AddDescription(T(Constants.THIS_SHOULD_BE_THE_DESCRIPTION_KEY))
                    .AddInstructions(T(Constants.THIS_SHOULD_BE_THE_INSTRUCTIONS_KEY))
                    .AddTestTime(TimeSpan.FromMinutes(45));

                await Task.Delay(750);

                CreateNewTestButton.Visibility = Visibility.Visible;
                EmptyTestsListButton.Visibility = Visibility.Collapsed;

                TestsList.Add(test);

                OpenTestBuilder(test.TestID);
            });
        }
        /// <summary>
        /// Event handler for the CreateNewTestButton click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void CreateNewTestButtonClick(object sender, RoutedEventArgs args)
        {
            try
            {
                if (!Guid.TryParse(m_localData.GetString(Constants.USER_ID_LOCAL_DATA), out Guid userID))
                {
                    throw new ApplicationException($"User ID not found.");
                }

                if (m_thisTest != null)
                {
                    var dialogResult = await this.ShowDialog(
                        func: r => r == ContentDialogResult.Primary,
                        args: new ShowDialogArgs(
                            title: T(Constants.ARE_YOU_SURE_KEY),
                            message: T(Constants.DATA_WILL_NOT_BE_SAVED_KEY),
                            okButtonText: T(Constants.YES_KEY),
                            cancelButtonText: T(Constants.CANCEL_KEY)
                        )
                    );

                    if (!dialogResult)
                    {
                        return;
                    }
                }

                if (m_thisTest != null && m_thisTest.IsNew)
                {
                    TestsList.Remove(m_thisTest);
                }

                m_thisTest = null;
                TasksList.Clear();

                var test = Test.Build(userID)
                    .AddName(T(Constants.THIS_SHOULD_BE_THE_NAME_KEY))
                    .AddDescription(T(Constants.THIS_SHOULD_BE_THE_DESCRIPTION_KEY))
                    .AddInstructions(T(Constants.THIS_SHOULD_BE_THE_INSTRUCTIONS_KEY))
                    .AddTestTime(TimeSpan.FromMinutes(45));

                TestsList.Add(test);

                OpenTestBuilder(test.TestID);
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
                m_thisTest = TestsList.FirstOrDefault(t => t.TestID == testID);

                if (m_thisTest == null)
                {
                    throw new ArgumentNullException(nameof(testID), $"Test not found.");
                }

                ExecuteAnimation(m_fadeInAnimation, TestBuilderBorder);
                TestBuilderBorder.Visibility = Visibility.Visible;

                TestsBuilderTitle.Text = T(Constants.EDITOR_KEY);

                TestsBuilderName.Header = CreateHeader(Constants.NAME_KEY);
                TestsBuilderName.Text = m_thisTest.Name;

                TestsBuilderDescription.Header = CreateHeader(Constants.DESCRIPTION_KEY);
                TestsBuilderDescription.Text = m_thisTest.Description;

                TestsBuilderInstructions.Header = CreateHeader(Constants.INSTRUCTIONS_KEY);
                TestsBuilderInstructions.Document.SetText(
                    Microsoft.UI.Text.TextSetOptions.None, m_thisTest.Instructions);

                TestsBuilderTime.Header = CreateHeader(Constants.TEST_TIME_KEY);
                TestsBuilderTime.Time = m_thisTest.TestTime;

                AutoSuggestStudentFinder.Header = CreateHeader(Constants.STUDENTS_KEY);
                AutoSuggestStudentFinder.PlaceholderText = T(Constants.ENTER_STUDENT_NAME_KEY);

                PreviewButtonText.Text = T(Constants.PREVIEW_KEY);

                SaveButtonText.Text = T(Constants.SAVE_KEY);
                SendButtonText.Text = T(Constants.SEND_KEY);
                AddNewTaskButtonText.Text = T(Constants.ADD_NEW_TASK_KEY);
                TextTaskFlyout.Text = T(Constants.TEXT_KEY);
                TrueFalseTaskFlyout.Text = T(Constants.TRUE_FALSE_KEY);
                CheckboxesTaskFlyout.Text = T(Constants.CHECKBOXES_KEY);
                MultipleChoiceTaskFlyout.Text = T(Constants.MULTIPLE_CHOICE_KEY);

                StudentsList.AddRange(m_thisTest.Subjects.Select(id => new TestBuilderStudent(id, "Ása Loredana")));
                TasksList.AddRange(m_thisTest.Tasks);

                SetCurrentTest();
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        /// <summary>
        /// Event handler for the TestBuilderCloseButton click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void PreviewButtonClick(object sender, RoutedEventArgs args)
        {
            SetCurrentTest();

            (Application.Current as App)?.ActivateTestPreviewWindow();
        }
        #endregion

        #region Test builder events
        /// <summary>
        /// Event handler for the TestName TextBox text changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
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
                    SetCurrentTest();
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
        /// <summary>
        /// Event handler for the TestDescription TextBox text changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
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
                    SetCurrentTest();
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
        /// <summary>
        /// Event handler for the TestInstructions RichEditBox text changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
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
                    SetCurrentTest();
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
        /// <summary>
        /// Event handler for the TestTime TimePicker value changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
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
                    SetCurrentTest();
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
        /// <summary>
        /// Event handler for the AddNewTask MenuFlyoutItem click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void AddNewTaskClick(object sender, RoutedEventArgs args)
        {
            try
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
                            TasksList.Add(TextTask.Build());
                            break;
                        case TaskType.TrueFalse:
                            TasksList.Add(TrueFalseTask.Build());
                            break;
                        case TaskType.Checkboxes:
                            TasksList.Add(CheckboxesTask.Build());
                            break;
                        case TaskType.MultipleChoice:
                            TasksList.Add(MultipleChoiceTask.Build());
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        /// <summary>
        /// Event handler for the TasksList collection changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void TasksCollectionChanged(object? sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                TasksListView.LayoutUpdated += OnTasksListLayoutUpdated;
            }
            else if (args.Action == NotifyCollectionChangedAction.Reset)
            {
                TasksListView.LayoutUpdated += OnTasksListLayoutUpdatedReopen;
            }
        }
        /// <summary>
        /// Event handler for the TasksListView layout updated event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnTasksListLayoutUpdated(object? sender, object args)
        {
            try
            {
                TasksListView.LayoutUpdated -= OnTasksListLayoutUpdated;

                UpdateTaskLocalization(TasksListView.Items.Count - 1);

                SetCurrentTest();
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        /// <summary>
        /// Event handler for the TasksListView layout updated event when reopening the test builder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnTasksListLayoutUpdatedReopen(object? sender, object args)
        {
            try
            {
                TasksListView.LayoutUpdated -= OnTasksListLayoutUpdatedReopen;

                for (int i = 0; i < TasksListView.Items.Count; i++)
                {
                    UpdateTaskLocalization(i);
                }

                SetCurrentTest();
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        /// <summary>
        /// Event handler for the RemoveTask Button click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void RemoveTaskClick(object sender, RoutedEventArgs args)
        {
            try
            {
                if (sender is Button button)
                {
                    if (button.Tag is Guid taskID)
                    {
                        var task = TasksList
                            .FirstOrDefault(item => item.TaskID.Equals(taskID));

                        if (task != null)
                        {
                            TasksList.Remove(task);

                            SetCurrentTest();

                            for (int i = 0; i < TasksList.Count; i++)
                            {
                                var listViewItem = TasksListView
                                    .ContainerFromIndex(i) as ListViewItem;

                                if (listViewItem?.ContentTemplateRoot is Border border)
                                {
                                    IEnumerable<FrameworkElement>? elements = (border?.Child as StackPanel)
                                        ?.Children.OfType<FrameworkElement>();

                                    if (elements != null)
                                    {
                                        SetTaskHeaderNumber(elements, i + 1);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        /// <summary>
        /// Event handler for the TaskQuestion RichEditBox text changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void TaskQuestionChanged(object sender, RoutedEventArgs args)
        {
            if (m_thisTest == null || sender is not RichEditBox text)
            {
                return;
            }

            try
            {
                var parent = text.Parent as StackPanel;

                if (parent != null)
                {
                    if (parent.Parent is Border border)
                    {
                        var task = TasksList
                            .FirstOrDefault(item => item.TaskID.Equals((Guid)border.Tag));

                        if (task == null)
                        {
                            return;
                        }

                        text.Document.GetText(Microsoft.UI.Text.TextGetOptions.None, out string question);

                        if (question.Equals("\r", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return;
                        }

                        task.SetQuestion(question);
                        SetCurrentTest();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        /// <summary>
        /// Event handler for the TaskCorrectAnswer RichEditBox text changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void TaskCorrectAnswerChanged(object sender, RoutedEventArgs args)
        {
            if (m_thisTest == null || sender is not RichEditBox text)
            {
                return;
            }

            try
            {
                var parent = text.Parent as StackPanel;

                if (parent != null)
                {
                    if (parent.Parent is Border border)
                    {
                        var task = TasksList
                            .FirstOrDefault(item => item.TaskID.Equals((Guid)border.Tag));

                        if (task == null)
                        {
                            return;
                        }

                        text.Document.GetText(Microsoft.UI.Text.TextGetOptions.None, out string correctAnswer);

                        if (correctAnswer.Equals("\r", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return;
                        }

                        task.SetCorrectAnswer(correctAnswer);
                        SetCurrentTest();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        /// <summary>
        /// Event handler for the TaskMaxGrade NumberBox value changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void TaskMaxGradeChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (m_thisTest == null)
            {
                return;
            }

            try
            {
                var parent = (sender.Parent as StackPanel)?.Parent as StackPanel;

                if (parent != null)
                {
                    if (parent.Parent is Border border)
                    {
                        var task = TasksList
                            .FirstOrDefault(item => item.TaskID.Equals((Guid)border.Tag));

                        if (task == null || task.MaxGrade == Convert.ToInt32(args.OldValue))
                        {
                            return;
                        }

                        task.SetMaxGrade(Convert.ToInt32(args.NewValue));
                        SetCurrentTest();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        /// <summary>
        /// Event handler for the TaskConsider ToggleSwitch toggled event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void TaskConsiderToggled(object sender, RoutedEventArgs args)
        {
            try
            {
                if (sender is ToggleSwitch switcher)
                {
                    var parent = (switcher.Parent as StackPanel)?.Parent as StackPanel;

                    if (parent != null)
                    {
                        if (parent.Parent is Border border)
                        {
                            var task = TasksList
                                .FirstOrDefault(item => item.TaskID.Equals((Guid)border.Tag));

                            task?.SetConsider(switcher.IsOn);
                            SetCurrentTest();
                        }

                        StackPanel? maxGradePanel = parent.Children
                            .OfType<FrameworkElement>()
                            .FirstOrDefault(item =>
                                FilterByTag(item, Constants.MAX_GRADE_PANEL_TEST_BUILDER_KEY)) as StackPanel;

                        if (maxGradePanel != null)
                        {
                            var input = maxGradePanel.Children
                                .OfType<FrameworkElement>()
                                .FirstOrDefault(item =>
                                    FilterByTag(item, Constants.MAX_GRADE_INPUT_TEST_BUILDER_KEY)) as NumberBox;

                            if (input != null)
                            {
                                input.IsEnabled = switcher.IsOn;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        /// <summary>
        /// Event handler for the CorrectAnswerRadioButton checked event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void CorrectAnswerRadioButtonChecked(object sender, RoutedEventArgs args)
        {
            try
            {
                if (sender is RadioButton element)
                {
                    var tag = element.Tag.ToString();

                    if (string.IsNullOrEmpty(tag))
                    {
                        return;
                    }

                    var border = ((element.Parent as FrameworkElement)
                        ?.Parent as FrameworkElement)
                            ?.Parent as Border;

                    if (border == null)
                    {
                        return;
                    }

                    var task = TasksList
                        .FirstOrDefault(item => item.TaskID.Equals((Guid)border.Tag));

                    if (task is not TrueFalseTask trueFalseTask)
                    {
                        return;
                    }

                    if (tag.Equals(Constants.CORRECT_ANSWER_RADIO_TRUE_TEST_BUILDER_KEY,
                            StringComparison.InvariantCultureIgnoreCase))
                    {
                        trueFalseTask.AddCorrectAnswer(true.ToString());
                    }
                    else if (tag.Equals(Constants.CORRECT_ANSWER_RADIO_FALSE_TEST_BUILDER_KEY,
                            StringComparison.InvariantCultureIgnoreCase))
                    {
                        trueFalseTask.AddCorrectAnswer(false.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        /// <summary>
        /// Event handler for the AnswerMultipleChoiceTextBox text changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void AnswerMultipleChoiceTextBoxTextChanged(object sender, TextChangedEventArgs args)
        {
            try
            {
                if (sender is TextBox element)
                {
                    Button? button = (element.Parent as StackPanel)?.Children
                        .OfType<FrameworkElement>()
                        .FirstOrDefault(item =>
                            FilterByTag(item, Constants.ADD_ANSWER_ADD_BUTTON_TEST_BUILDER_KEY)) as Button;

                    if (button == null)
                    {
                        return;
                    }

                    if (string.IsNullOrEmpty(element.Text))
                    {
                        button.IsEnabled = false;
                    }
                    else
                    {
                        button.IsEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        /// <summary>
        /// Event handler for the AddAnswerMultipleChoice Button click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void AddAnswerMultipleChoiceClick(object sender, RoutedEventArgs args)
        {
            try
            {
                if (sender is Button element)
                {
                    var inputPanel = element.Parent as StackPanel;
                    var mainPanel = inputPanel?.Parent as StackPanel;
                    var task = (mainPanel?.Parent as Border)?.DataContext as MultipleChoiceTask;

                    ListView? list = mainPanel?.Children
                        .OfType<FrameworkElement>()
                        .FirstOrDefault(item =>
                            FilterByTag(item, Constants.ANSWERS_LIST_TEST_BUILDER_KEY)) as ListView;

                    TextBox? input = inputPanel?.Children
                        .OfType<FrameworkElement>()
                        .FirstOrDefault(item =>
                            FilterByTag(item, Constants.ADD_ANSWER_INPUT_TEST_BUILDER_KEY)) as TextBox;

                    ComboBox? comboBox = mainPanel?.Children
                        .OfType<FrameworkElement>()
                        .FirstOrDefault(item =>
                            FilterByTag(item, Constants.ADD_ANSWER_COMBO_BOX_TEST_BUILDER_KEY)) as ComboBox;

                    if (list != null && inputPanel != null && input != null && comboBox != null && task != null)
                    {
                        if (string.IsNullOrEmpty(input.Text))
                        {
                            return;
                        }

                        var listItems = list.ItemsSource as ExtendedObservableCollection<TestBuilderAnswersListViewItem>;
                        var comboBoxItems = comboBox.ItemsSource as ExtendedObservableCollection<string>;

                        if (listItems == null || comboBoxItems == null)
                        {
                            return;
                        }

                        foreach (var item in listItems)
                        {
                            if (input.Text.Equals(item.Text))
                            {
                                return;
                            }
                        }

                        if (listItems.Count >= 4)
                        {
                            return;
                        }

                        listItems.Add(new TestBuilderAnswersListViewItem(input.Text, task.TaskID));

                        comboBoxItems.Add(input.Text);

                        task.AddAnswerItem(input.Text);
                        SetCurrentTest();

                        input.Text = string.Empty;

                        if (listItems.Count >= 4)
                        {
                            inputPanel.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            inputPanel.Visibility = Visibility.Visible;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        /// <summary>
        /// Event handler for the RemoveAnswerMultipleChoice MenuFlyoutItem click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void RemoveAnswerMultipleChoiceClick(object sender, RoutedEventArgs args)
        {
            try
            {
                if (sender is MenuFlyoutItem element)
                {
                    var item = element.DataContext as TestBuilderAnswersListViewItem;

                    var task = (TasksListView.ItemsSource as ExtendedObservableCollection<CoreTask>)
                        ?.FirstOrDefault(task => task.TaskID.Equals(item?.ID)) as MultipleChoiceTask;

                    var border = (TasksListView.ContainerFromItem(task) as ListViewItem)?.ContentTemplateRoot as Border;

                    var mainPanel = border?.Child as StackPanel;

                    ListView? list = mainPanel?.Children
                        .OfType<FrameworkElement>()
                        .FirstOrDefault(item =>
                            FilterByTag(item, Constants.ANSWERS_LIST_TEST_BUILDER_KEY)) as ListView;

                    StackPanel? inputPanel = mainPanel?.Children
                        .OfType<FrameworkElement>()
                        .FirstOrDefault(item =>
                            FilterByTag(item, Constants.ADD_ANSWER_PANEL_TEST_BUILDER_KEY)) as StackPanel;

                    ComboBox? comboBox = mainPanel?.Children
                        .OfType<FrameworkElement>()
                        .FirstOrDefault(item =>
                            FilterByTag(item, Constants.ADD_ANSWER_COMBO_BOX_TEST_BUILDER_KEY)) as ComboBox;

                    if (task == null || item == null || list == null || comboBox == null || inputPanel == null)
                    {
                        return;
                    }

                    var listItems = list.ItemsSource as ExtendedObservableCollection<TestBuilderAnswersListViewItem>;
                    var comboBoxItems = comboBox.ItemsSource as ExtendedObservableCollection<string>;

                    comboBox.SelectedItem = null;

                    if (listItems == null || comboBoxItems == null)
                    {
                        return;
                    }

                    listItems.Remove(item);
                    comboBoxItems.Remove(item.Text);

                    task.RemoveAnswerItem(item.Text);
                    SetCurrentTest();

                    if (listItems.Count >= 4)
                    {
                        inputPanel.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        inputPanel.Visibility = Visibility.Visible;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        /// <summary>
        /// Event handler for the CorrectAnswerMultipleChoice ComboBox selection changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void CorrectAnswerMultipleChoiceSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            try
            {
                if (sender is ComboBox element)
                {
                    var task = ((element.Parent as FrameworkElement)
                        ?.Parent as FrameworkElement)?.DataContext as MultipleChoiceTask;

                    var selectedItem = element.SelectedItem as string;

                    if (task == null || string.IsNullOrEmpty(selectedItem))
                    {
                        return;
                    }

                    if (task.Answers.Contains(selectedItem))
                    {
                        task.AddCorrectAnswer(selectedItem);
                        SetCurrentTest();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        /// <summary>
        /// Event handler for the AddAnswerCheckboxes Button click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void AddAnswerCheckboxesClick(object sender, RoutedEventArgs args)
        {
            try
            {
                if (sender is Button element)
                {
                    var inputPanel = element.Parent as StackPanel;
                    var mainPanel = inputPanel?.Parent as StackPanel;
                    var task = (mainPanel?.Parent as Border)?.DataContext as CheckboxesTask;

                    ListView? list = mainPanel?.Children
                        .OfType<FrameworkElement>()
                        .FirstOrDefault(item =>
                            FilterByTag(item, Constants.ANSWERS_LIST_TEST_BUILDER_KEY)) as ListView;

                    TextBox? input = inputPanel?.Children
                        .OfType<FrameworkElement>()
                        .FirstOrDefault(item =>
                            FilterByTag(item, Constants.ADD_ANSWER_INPUT_TEST_BUILDER_KEY)) as TextBox;

                    ListView? checkBoxesList = mainPanel?.Children
                        .OfType<FrameworkElement>()
                        .FirstOrDefault(item =>
                            FilterByTag(item, Constants.CHACK_BOX_LIST_TEST_BUILDER_KEY)) as ListView;

                    if (list != null && inputPanel != null && input != null && checkBoxesList != null && task != null)
                    {
                        if (string.IsNullOrEmpty(input.Text))
                        {
                            return;
                        }

                        var listItems = list.ItemsSource as ExtendedObservableCollection<TestBuilderCheckBoxesListViewItem>;
                        var checkBoxesItems = checkBoxesList.ItemsSource as ExtendedObservableCollection<TestBuilderCheckBoxesListViewItem>;

                        if (listItems == null || checkBoxesItems == null)
                        {
                            return;
                        }

                        foreach (var item in listItems)
                        {
                            if (input.Text.Equals(item.Text))
                            {
                                return;
                            }
                        }

                        if (listItems.Count >= 4)
                        {
                            return;
                        }

                        listItems.Add(new TestBuilderCheckBoxesListViewItem(input.Text, task.TaskID));

                        checkBoxesItems.Add(new TestBuilderCheckBoxesListViewItem(input.Text, task.TaskID));

                        task.AddAnswerItem(input.Text);
                        SetCurrentTest();

                        input.Text = string.Empty;

                        if (listItems.Count >= 4)
                        {
                            inputPanel.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            inputPanel.Visibility = Visibility.Visible;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        /// <summary>
        /// Event handler for the RemoveAnswerCheckBoxes MenuFlyoutItem click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void RemoveAnswerCheckBoxesClick(object sender, RoutedEventArgs args)
        {
            try
            {
                if (sender is MenuFlyoutItem element)
                {
                    var item = element.DataContext as TestBuilderCheckBoxesListViewItem;

                    var task = (TasksListView.ItemsSource as ExtendedObservableCollection<CoreTask>)
                        ?.FirstOrDefault(task => task.TaskID.Equals(item?.ID)) as CheckboxesTask;

                    var border = (TasksListView.ContainerFromItem(task) as ListViewItem)?.ContentTemplateRoot as Border;

                    var mainPanel = border?.Child as StackPanel;

                    ListView? list = mainPanel?.Children
                        .OfType<FrameworkElement>()
                        .FirstOrDefault(item =>
                            FilterByTag(item, Constants.ANSWERS_LIST_TEST_BUILDER_KEY)) as ListView;

                    StackPanel? inputPanel = mainPanel?.Children
                        .OfType<FrameworkElement>()
                        .FirstOrDefault(item =>
                            FilterByTag(item, Constants.ADD_ANSWER_PANEL_TEST_BUILDER_KEY)) as StackPanel;

                    ListView? checkBoxesList = mainPanel?.Children
                        .OfType<FrameworkElement>()
                        .FirstOrDefault(item =>
                            FilterByTag(item, Constants.CHACK_BOX_LIST_TEST_BUILDER_KEY)) as ListView;

                    if (task == null || item == null || list == null || checkBoxesList == null || inputPanel == null)
                    {
                        return;
                    }

                    var listItems = list.ItemsSource as ExtendedObservableCollection<TestBuilderCheckBoxesListViewItem>;
                    var comboBoxItems = checkBoxesList.ItemsSource as ExtendedObservableCollection<TestBuilderCheckBoxesListViewItem>;

                    if (listItems == null || comboBoxItems == null)
                    {
                        return;
                    }

                    var removeItem = comboBoxItems.FirstOrDefault(r => r.ID.Equals(item.ID));
                    if (removeItem != null)
                    {
                        comboBoxItems.Remove(removeItem);
                    }

                    listItems.Remove(item);

                    task.RemoveAnswersItem(item.Text);
                    SetCurrentTest();

                    if (listItems.Count >= 4)
                    {
                        inputPanel.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        inputPanel.Visibility = Visibility.Visible;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        /// <summary>
        /// Event handler for the CheckBox check event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void CheckBoxCheck(object sender, RoutedEventArgs args)
        {
            try
            {
                if (sender is CheckBox element)
                {
                    var taskID = (Guid)element.Tag;
                    var text = (element.Content as TextBlock)?.Text;

                    var task = TasksList
                        .FirstOrDefault(item => item.TaskID.Equals(taskID)) as CheckboxesTask;

                    if (task != null && !string.IsNullOrEmpty(text))
                    {
                        task.AddCorrectAnswerItem(text);
                        SetCurrentTest();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        /// <summary>
        /// Event handler for the CheckBox uncheck event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void CheckBoxUncheck(object sender, RoutedEventArgs args)
        {
            try
            {
                if (sender is CheckBox element)
                {
                    var taskID = (Guid)element.Tag;
                    var text = (element.Content as TextBlock)?.Text;

                    var task = TasksList
                        .FirstOrDefault(item => item.TaskID.Equals(taskID)) as CheckboxesTask;

                    if (task != null && !string.IsNullOrEmpty(text))
                    {
                        task.RemoveCorrectAnswerItem(text);
                        SetCurrentTest();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        /// <summary>
        /// Event handler for the AutoSuggestBox text changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void AutoSuggestBoxTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            var students = new List<string>() // Temp
            {
                "Angela Larson",
                "Erica Madden",
                "Mark Conner",
                "Donald Gallegos",
                "Demetrius Franklin"
            };

            try
            {
                if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
                {
                    var suitableItems = new List<string>();
                    var splitText = sender.Text.Split(" ");

                    foreach (var student in students)
                    {
                        var found = splitText.All((key) =>
                        {
                            return student.Contains(key, StringComparison.CurrentCultureIgnoreCase);
                        });

                        if (found)
                        {
                            suitableItems.Add(student);
                        }
                    }

                    if (suitableItems.Count == 0)
                    {
                        suitableItems.Add(T(Constants.STUDENT_NOT_FOUND_KEY));
                    }

                    sender.ItemsSource = suitableItems;
                }
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        /// <summary>
        /// Event handler for the AutoSuggestBox suggestion chosen event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void AutoSuggestBoxSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            try
            {
                var student = args.SelectedItem.ToString();

                if (string.IsNullOrEmpty(student) ||
                    m_thisTest == null ||
                    StudentsList.Any(s => s.DisplayName.Equals(student, StringComparison.CurrentCultureIgnoreCase)))
                {
                    return;
                }

                var studentEntity = new TestBuilderStudent(Guid.NewGuid(), student);

                StudentsList.Add(studentEntity);

                m_thisTest.AddSubject(studentEntity.ID);
                SetCurrentTest();
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        /// <summary>
        /// Event handler for the AutoSuggestBox query submitted event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void AutoSuggestBoxQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            try
            {
                string student = string.Empty;
                if (args.ChosenSuggestion != null)
                {
                    var studentTemp = args.ChosenSuggestion.ToString();

                    if (!string.IsNullOrEmpty(studentTemp))
                    {
                        student = studentTemp;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(args.QueryText))
                    {
                        student = args.QueryText;
                    }
                }

                if (string.IsNullOrEmpty(student) ||
                        m_thisTest == null ||
                        StudentsList.Any(s => s.DisplayName.Equals(student, StringComparison.CurrentCultureIgnoreCase)))
                {
                    return;
                }

                var studentEntity = new TestBuilderStudent(Guid.NewGuid(), student);

                StudentsList.Add(studentEntity);

                m_thisTest.AddSubject(studentEntity.ID);
                SetCurrentTest();
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        /// <summary>
        /// Event handler for the RemoveStudent Button click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void RemoveStudentClick(object sender, RoutedEventArgs args)
        {
            try
            {
                if (sender is MenuFlyoutItem element)
                {
                    var item = element.DataContext as TestBuilderStudent;

                    if (item == null || m_thisTest == null)
                    {
                        return;
                    }

                    StudentsList.Remove(item);

                    m_thisTest.RemoveSubject(item.ID);
                    SetCurrentTest();
                }
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        #endregion
    }
}