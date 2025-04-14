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
        #region Main
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

                CreateNewTestButton.Visibility = Visibility.Visible;
                EmptyTestsListButton.Visibility = Visibility.Collapsed;

                TestsList.Add(test);

                OpenTestBuilder(test.TestID);
            });
        }

        private void CreateNewTestButtonClick(object sender, RoutedEventArgs args)
        {
            try
            {
                var test = GetCurrentTest();

                if (test != null)
                {
                    test.OnDevSave();

                    // TODO: Send to server

                    m_thisTest = null;
                    TasksList.Clear();
                }

                test = Test.Build(Guid.NewGuid()) // USE USER ID
                    .AddName(m_localization.GetString(Constants.THIS_SHOULD_BE_THE_NAME_KEY))
                    .AddDescription(m_localization.GetString(Constants.THIS_SHOULD_BE_THE_DESCRIPTION_KEY))
                    .AddInstructions(m_localization.GetString(Constants.THIS_SHOULD_BE_THE_INSTRUCTIONS_KEY))
                    .AddTestTime(TimeSpan.FromHours(1));

                TestsList.Add(test);

                OpenTestBuilder(test.TestID);
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }

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

                TestsBuilderName.Header = CreateHeader(Constants.NAME_KEY);
                TestsBuilderName.Text = m_thisTest.Name;

                TestsBuilderDescription.Header = CreateHeader(Constants.DESCRIPTION_KEY);
                TestsBuilderDescription.Text = m_thisTest.Description;

                TestsBuilderInstructions.Header = CreateHeader(Constants.INSTRUCTIONS_KEY);
                TestsBuilderInstructions.Document.SetText(
                    Microsoft.UI.Text.TextSetOptions.None, m_thisTest.Instructions);

                TestsBuilderTime.Header = CreateHeader(Constants.TEST_TIME_KEY);
                TestsBuilderTime.Time = m_thisTest.TestTime;

                TasksList.AddRange(m_thisTest.Tasks);

                SetCurrentTest();
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        #endregion

        #region Test builder
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
        #endregion
    }
}