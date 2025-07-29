using Microsoft.UI.Xaml;
using System.Collections.Generic;
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
using Windows.ApplicationModel.DataTransfer;

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

                TestsList.Insert(0, test);

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
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void PreviewButtonClick(object sender, RoutedEventArgs args)
        {
            SetCurrentTest();

            (Application.Current as App)?.ActivateTestPreviewWindow();
        }
        /// <summary>
        /// Event handler for the SaveButton click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void SaveButtonClick(object sender, RoutedEventArgs args)
        {
            try
            {
                if (m_thisTest != null)
                {
                    var dialogResult = await this.ShowDialog(
                        func: r => r == ContentDialogResult.Primary,
                        args: new ShowDialogArgs(
                            title: T(Constants.ARE_YOU_SURE_KEY),
                            message: T(Constants.YOU_CAN_ALWAYS_CHANGE_THE_TEST_DATA_KEY),
                            okButtonText: T(Constants.YES_KEY),
                            cancelButtonText: T(Constants.CANCEL_KEY)
                        )
                    );

                    if (!dialogResult)
                    {
                        return;
                    }
                }

                SetCurrentTest();
                await m_testManager.SaveOrUpdateCurrentBuilderTestAsync();

                m_thisTest = null;
                TasksList.Clear();
                m_testManager.ClearCurrentBuilderTest();

                await SetTestListItemsAsync();

                m_uiUtilities.ExecuteAnimation(m_fadeOutAnimation, TestBuilderBorder);
                await Task.Delay(500);
                TestBuilderBorder.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        /// <summary>
        /// Event handler for the RemoveButton click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void RemoveButtonClick(object sender, RoutedEventArgs args)
        {
            try
            {
                if (m_thisTest == null)
                {
                    return;
                }

                var dialogResult = await this.ShowDialog(
                    func: r => r == ContentDialogResult.Primary,
                    args: new ShowDialogArgs(
                        title: T(Constants.ARE_YOU_SURE_KEY),
                        message: T(Constants.THIS_IS_IRREVERSIBLE_ACTION_KEY),
                        okButtonText: T(Constants.YES_KEY),
                        cancelButtonText: T(Constants.CANCEL_KEY)
                    )
                );

                if (!dialogResult)
                {
                    return;
                }

                if (!m_thisTest.IsNew)
                {
                    SetCurrentTest();

                    await m_testManager.RemoveTestAsync(m_thisTest);
                }

                TestsList.Remove(m_thisTest);
                m_thisTest = null;
                TasksList.Clear();
                m_testManager.ClearCurrentBuilderTest();

                await SetTestListItemsAsync();

                m_uiUtilities.ExecuteAnimation(m_fadeOutAnimation, TestBuilderBorder);
                await Task.Delay(500);
                TestBuilderBorder.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        /// <summary>
        /// Event handler for the OpenTaskButton click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void OpenTestButtonClick(object sender, RoutedEventArgs args)
        {
            if (sender is Button element)
            {
                if (element.DataContext is not Test context)
                {
                    return;
                }

                if (m_thisTest != null)
                {
                    if (m_thisTest.TestID.Equals(context.TestID))
                    {
                        return;
                    }

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
                m_testManager.ClearCurrentBuilderTest();

                OpenTestBuilder(context.TestID);
            }
        }
        /// <summary>
        /// Event handler for the CopyToClipboardButton click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void CopyToClipboardButtonClick(object sender, RoutedEventArgs args)
        {
            try
            {
                if (sender is Button element)
                {
                    string text = element.Tag.ToString() ?? string.Empty;
                    
                    var package = new DataPackage();

                    package.SetText(text);

                    Clipboard.SetContent(package);
                }
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        #endregion

        #region Builder events
        /// <summary>
        /// Event handler for the TaskHeader Loaded event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnTaskHeaderLoaded(object sender, RoutedEventArgs args)
        {
            try
            {
                if (sender is Grid element)
                {
                    CoreTask? task = GetTaskFromDataContext<CoreTask>(element.DataContext);

                    if (task == null)
                    {
                        return;
                    }

                    var index = TasksList.IndexOf(task) + 1;

                    TextBlock? textNumber = m_uiUtilities.FindElementByGridPosition(element, 0, 0) as TextBlock;

                    if (textNumber == null)
                    {
                        return;
                    }

                    textNumber.Text = string.Concat("№", index.ToString());
                }
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        /// <summary>
        /// Event handler for the TaskConsiderText Loaded event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnTaskConsiderTextLoaded(object sender, RoutedEventArgs args)
        {
            if (sender is TextBlock element)
            {
                element.Text = MessageProvider.CONSIDER_IN_THE_ASSESSMENT_KEY;
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
                if (sender is ToggleSwitch element)
                {
                    CoreTask? task = GetTaskFromDataContext<CoreTask>(element.DataContext);

                    if (task == null)
                    {
                        return;
                    }

                    task.SetConsider(element.IsOn);
                    SetCurrentTest();

                    var border = ((element.Parent as FrameworkElement)?.Parent as FrameworkElement)?.Parent as Border;
                    var maxGradeInput = m_uiUtilities.FindElementByTag<NumberBox>(border, task.TaskID.ToString());

                    if (maxGradeInput == null)
                    {
                        return;
                    }

                    maxGradeInput.IsEnabled = element.IsOn;
                }
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        /// <summary>
        /// Event handler for the TaskConsider Loaded event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void TaskConsiderLoaded(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch element)
            {
                element.OffContent = MessageProvider.NO;
                element.OnContent = MessageProvider.YES;

                CoreTask? task = GetTaskFromDataContext<CoreTask>(element.DataContext);

                if (task == null)
                {
                    return;
                }

                element.IsOn = task.Consider;
            }
        }
        /// <summary>
        /// Event handler for the TaskQuestion Loaded event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void TaskQuestionLoaded(object sender, RoutedEventArgs args)
        {
            try
            {
                if (sender is RichEditBox element)
                {
                    CoreTask? task = GetTaskFromDataContext<CoreTask>(element.DataContext);

                    if (task == null)
                    {
                        return;
                    }

                    element.Header = MessageProvider.ToHeaderCenter(MessageProvider.QUESTION_KEY);
                    element.PlaceholderText = MessageProvider.THIS_SHOULD_BE_THE_QUESTION_KEY;

                    if (task.Question.StartsWith(Constants.NULL))
                    {
                        return;
                    }

                    element.Document.SetText(Microsoft.UI.Text.TextSetOptions.None, task.Question);
                }
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        /// <summary>
        /// Event handler for the TaskCorrectAnswer Loaded event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void TaskCorrectAnswerLoaded(object sender, RoutedEventArgs args)
        {
            try
            {
                if (sender is RichEditBox element)
                {
                    CoreTask? task = GetTaskFromDataContext<CoreTask>(element.DataContext);

                    if (task == null)
                    {
                        return;
                    }

                    if (task.CorrectAnswer.StartsWith(Constants.NULL))
                    {
                        return;
                    }

                    element.Document.SetText(Microsoft.UI.Text.TextSetOptions.None, task.Question);
                }
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        /// <summary>
        /// Event handler for the TaskMaxGrade Loaded event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void TaskMaxGradeLoaded(object sender, RoutedEventArgs args)
        {
            try
            {
                if (sender is NumberBox element)
                {
                    CoreTask? task = GetTaskFromDataContext<CoreTask>(element.DataContext);

                    if (task == null)
                    {
                        return;
                    }

                    element.Tag = task.TaskID.ToString();
                    element.Value = Convert.ToDouble(task.MaxGrade);

                    if (task.Consider)
                    {
                        element.IsEnabled = true;
                    }
                    else
                    {
                        element.IsEnabled = false;
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
            try
            {
                CoreTask? task = GetTaskFromDataContext<CoreTask>(sender.DataContext);

                if (task == null)
                {
                    return;
                }

                task.SetMaxGrade(Convert.ToInt32(args.NewValue));
                SetCurrentTest();
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
                    TrueFalseTask? task = GetTaskFromDataContext<TrueFalseTask>(element.DataContext);

                    if (task == null)
                    {
                        return;
                    }

                    TextBlock? content = element.Content as TextBlock;

                    if (content == null)
                    {
                        return;
                    }

                    if (content.Text.Equals(MessageProvider.YES, StringComparison.InvariantCultureIgnoreCase))
                    {
                        task.AddCorrectAnswer(true.ToString());
                    }
                    else if (content.Text.Equals(MessageProvider.NO, StringComparison.InvariantCultureIgnoreCase))
                    {
                        task.AddCorrectAnswer(false.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        /// <summary>
        /// Event handler for the CorrectAnswerRadioButton loaded event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void CorrectAnswerRadioButtonLoaded(object sender, RoutedEventArgs args)
        {
            try
            {
                if (sender is RadioButton element)
                {
                    TrueFalseTask? task = GetTaskFromDataContext<TrueFalseTask>(element.DataContext);

                    if (task == null)
                    {
                        return;
                    }

                    TextBlock? content = element.Content as TextBlock;

                    if (content == null)
                    {
                        return;
                    }

                    if (task.CorrectAnswer.Equals(true.ToString(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (content.Text.Equals(MessageProvider.YES, StringComparison.InvariantCultureIgnoreCase))
                        {
                            element.IsChecked = true;
                        }
                    }
                    else if (task.CorrectAnswer.Equals(false.ToString(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (content.Text.Equals(MessageProvider.NO, StringComparison.InvariantCultureIgnoreCase))
                        {
                            element.IsChecked = true;
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
        /// Event handler for the CheckboxesAnswersListView loaded event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void AnswersListViewLoaded(object sender, RoutedEventArgs args)
        {
            try
            {
                if (sender is ListView element)
                {
                    CheckboxesTask? task = GetTaskFromDataContext<CheckboxesTask>(element.DataContext);

                    if (task == null)
                    {
                        return;
                    }

                    ExtendedObservableCollection<BuilderListItem> listItems = new();

                    listItems.AddRange(task.Answers.Select(item => new BuilderListItem(item, task.TaskID)));

                    element.ItemsSource = listItems;
                }
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        /// <summary>
        /// Event handler for the MultipleChoiceAnswersListView loaded event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void MultipleChoiceAnswersListViewLoaded(object sender, RoutedEventArgs args)
        {
            try
            {
                if (sender is ListView element)
                {
                    MultipleChoiceTask? task = GetTaskFromDataContext<MultipleChoiceTask>(element.DataContext);

                    if (task == null)
                    {
                        return;
                    }

                    ExtendedObservableCollection<BuilderListItem> listItems = new();

                    listItems.AddRange(task.Answers.Select(item => new BuilderListItem(item, task.TaskID)));

                    element.ItemsSource = listItems;
                }
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        /// <summary>
        /// Event handler for the CheckboxesAnswersInputStack loaded event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void AnswersInputStackLoaded(object sender, RoutedEventArgs args)
        {
            try
            {
                if (sender is StackPanel element)
                {
                    CheckboxesTask? task = GetTaskFromDataContext<CheckboxesTask>(element.DataContext);

                    if (task != null && task.Answers.Length >= 4)
                    {
                        element.Visibility = Visibility.Collapsed;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        /// <summary>
        /// Event handler for the CheckboxesCorrectAnswersListView loaded event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void CheckboxesCorrectAnswersListViewLoaded(object sender, RoutedEventArgs args)
        {
            try
            {
                if (sender is ListView element)
                {
                    CheckboxesTask? task = GetTaskFromDataContext<CheckboxesTask>(element.DataContext);

                    if (task == null)
                    {
                        return;
                    }

                    ExtendedObservableCollection<BuilderListItem> listItems = new();

                    listItems.AddRange(task.Answers.Select(item => new BuilderListItem(item, task.TaskID)));

                    if (!task.CorrectAnswer.Equals(Constants.NULL, StringComparison.InvariantCultureIgnoreCase))
                    {
                        foreach (var correctAnswer in task.GetCorrectAnswers())
                        {
                            var item = listItems.FirstOrDefault(item => item.Text.Equals(correctAnswer));

                            if (item != null)
                            {
                                item.IsChecked = true;
                            }
                        }
                    }
                    
                    element.ItemsSource = listItems;
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
        private void AnswerTextBoxTextChanged(object sender, TextChangedEventArgs args)
        {
            try
            {
                if (sender is TextBox element)
                {
                    Button? button = m_uiUtilities.FindNeighbors<Button>(element).ElementAtOrDefault(0);

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
                    TextBox? input = m_uiUtilities.FindNeighbors<TextBox>(element).ElementAtOrDefault(0);
                    List<ListView> listViews = m_uiUtilities.FindNeighbors<ListView>(element.Parent as Panel).ToList();
                    ExtendedObservableCollection<BuilderListItem>? listItems = listViews.ElementAtOrDefault(0)?.ItemsSource as ExtendedObservableCollection<BuilderListItem>;
                    ExtendedObservableCollection<BuilderListItem>? checkBoxesItems = listViews.ElementAtOrDefault(1)?.ItemsSource as ExtendedObservableCollection<BuilderListItem>;
                    CheckboxesTask? task = GetTaskFromDataContext<CheckboxesTask>(element.DataContext);

                    if (input == null || listItems == null || checkBoxesItems == null || task == null || string.IsNullOrEmpty(input.Text))
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

                    listItems.Add(new(input.Text, task.TaskID));
                    checkBoxesItems.Add(new(input.Text, task.TaskID));

                    task.AddAnswerItem(input.Text);
                    SetCurrentTest();

                    input.Text = string.Empty;
                     
                    if (element.Parent is FrameworkElement panel)
                    {
                        if (listItems.Count >= 4)
                        {
                            panel.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            panel.Visibility = Visibility.Visible;
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
                    TextBox? input = m_uiUtilities.FindNeighbors<TextBox>(element).ElementAtOrDefault(0);
                    ExtendedObservableCollection<BuilderListItem>? listItems = m_uiUtilities.FindNeighbors<ListView>(element.Parent as Panel).ElementAtOrDefault(0)?.ItemsSource as ExtendedObservableCollection<BuilderListItem>;
                    ExtendedObservableCollection<string>? comboBoxItems = m_uiUtilities.FindNeighbors<ComboBox>(element.Parent as Panel).ElementAtOrDefault(0)?.ItemsSource as ExtendedObservableCollection<string>;
                    MultipleChoiceTask? task = GetTaskFromDataContext<MultipleChoiceTask>(element.DataContext);

                    if (input == null || listItems == null || comboBoxItems == null || task == null || string.IsNullOrEmpty(input.Text))
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

                    listItems.Add(new(input.Text, task.TaskID));
                    comboBoxItems.Add(input.Text);

                    task.AddAnswerItem(input.Text);
                    SetCurrentTest();

                    input.Text = string.Empty;

                    if (element.Parent is FrameworkElement panel)
                    {
                        if (listItems.Count >= 4)
                        {
                            panel.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            panel.Visibility = Visibility.Visible;
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
        /// Event handler for the MultipleChoiceCorrectAnswer loaded event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void MultipleChoiceCorrectAnswerLoaded(object sender, RoutedEventArgs args)
        {
            if (sender is ComboBox element)
            {
                MultipleChoiceTask? task = GetTaskFromDataContext<MultipleChoiceTask>(element.DataContext);

                if (task == null)
                {
                    return;
                }

                var items = new ExtendedObservableCollection<string>();

                items.AddRange(task.Answers);

                element.ItemsSource = items;

                if (!string.IsNullOrEmpty(task.CorrectAnswer) && items.Contains(task.CorrectAnswer))
                {
                    element.SelectedValue = task.CorrectAnswer;
                }
            }
        }
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

                    SetCurrentTest();
                }
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
        private async void RemoveTaskClick(object sender, RoutedEventArgs args)
        {
            try
            {
                if (sender is Button button)
                {
                    var dialogResult = await this.ShowDialog(
                        func: r => r == ContentDialogResult.Primary,
                        args: new ShowDialogArgs(
                            title: T(Constants.ARE_YOU_SURE_KEY),
                            message: T(Constants.THIS_IS_IRREVERSIBLE_ACTION_KEY),
                            okButtonText: T(Constants.YES_KEY),
                            cancelButtonText: T(Constants.CANCEL_KEY)
                        )
                    );

                    if (!dialogResult)
                    {
                        return;
                    }

                    var task = GetTaskFromDataContext<CoreTask>(button.DataContext);

                    if (task != null)
                    {
                        TasksList.Remove(task);

                        SetCurrentTest();

                        for (int i = 0; i < TasksList.Count; i++)
                        {
                            var item = TasksListView.ContainerFromIndex(i) as ListViewItem;

                            var сontrol = item?.ContentTemplateRoot as ContentControl;

                            сontrol?.ApplyTemplate();

                            var elements = m_uiUtilities.FindChild<Border>(сontrol)?.Child as StackPanel;

                            var header = elements?.Children.OfType<Grid>().ElementAtOrDefault(0);

                            var textNumber = m_uiUtilities.FindElementByGridPosition(header, 0, 0) as TextBlock;

                            if (textNumber != null)
                            {
                                textNumber.Text = string.Concat("№", (i + 1).ToString());
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
                var task = GetTaskFromDataContext<CoreTask>(text.DataContext);

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
                var task = GetTaskFromDataContext<CoreTask>(text.DataContext);

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
                    var task = GetTaskFromDataContext<MultipleChoiceTask>(element.DataContext);

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
                    var taskID = (element.DataContext as BuilderListItem)?.ID;
                    var task = TasksList.FirstOrDefault(t => t.TaskID.Equals(taskID)) as CheckboxesTask;
                    var text = (element.Content as TextBlock)?.Text;

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
                    var taskID = (element.DataContext as BuilderListItem)?.ID;
                    var task = TasksList.FirstOrDefault(t => t.TaskID.Equals(taskID)) as CheckboxesTask;
                    var text = (element.Content as TextBlock)?.Text;

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
                    var context = element.DataContext as BuilderListItem;

                    var task = TasksList.FirstOrDefault(t => t.TaskID.Equals(context?.ID)) as MultipleChoiceTask;

                    var viewItem = TasksListView.ContainerFromItem(task) as ListViewItem;

                    var сontrol = viewItem?.ContentTemplateRoot as ContentControl;

                    сontrol?.ApplyTemplate();

                    var elements = m_uiUtilities.FindChild<Border>(сontrol)?.Child as StackPanel;

                    var contentPresenter = elements?.Children.OfType<ContentPresenter>().ElementAtOrDefault(0);

                    var contentElements = contentPresenter?.Content as StackPanel;

                    var listView = contentElements?.Children.OfType<ListView>().ElementAtOrDefault(0);

                    var inputPanel = contentElements?.Children.OfType<StackPanel>().ElementAtOrDefault(0);

                    var comboBox = contentElements?.Children.OfType<ComboBox>().ElementAtOrDefault(0);

                    var listItems = listView?.ItemsSource as ExtendedObservableCollection<BuilderListItem>;

                    var comboBoxItems = comboBox?.ItemsSource as ExtendedObservableCollection<string>;

                    if (comboBox == null || listItems == null || comboBoxItems == null || inputPanel == null || context == null || task == null)
                    {
                        return;
                    }

                    comboBox.SelectedItem = null;

                    listItems.Remove(context);
                    comboBoxItems.Remove(context.Text);

                    task.RemoveAnswerItem(context.Text);
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
                    var context = element.DataContext as BuilderListItem;

                    var task = TasksList.FirstOrDefault(t => t.TaskID.Equals(context?.ID)) as CheckboxesTask;

                    var viewItem = TasksListView.ContainerFromItem(task) as ListViewItem;

                    var сontrol = viewItem?.ContentTemplateRoot as ContentControl;

                    сontrol?.ApplyTemplate();

                    var elements = m_uiUtilities.FindChild<Border>(сontrol)?.Child as StackPanel;

                    var contentPresenter = elements?.Children.OfType<ContentPresenter>().ElementAtOrDefault(0);

                    var contentElements = contentPresenter?.Content as StackPanel;

                    var listView = contentElements?.Children.OfType<ListView>().ElementAtOrDefault(0);

                    var inputPanel = contentElements?.Children.OfType<StackPanel>().ElementAtOrDefault(0);

                    var checkBoxesList = contentElements?.Children.OfType<ListView>().ElementAtOrDefault(1);

                    var listItems = listView?.ItemsSource as ExtendedObservableCollection<BuilderListItem>;
                    
                    var comboBoxItems = checkBoxesList?.ItemsSource as ExtendedObservableCollection<BuilderListItem>;

                    if (context == null || comboBoxItems == null || listItems == null || task == null || inputPanel == null)
                    {
                        return;
                    }

                    var removeItem = comboBoxItems.FirstOrDefault(r => r.ID.Equals(context.ID));
                    if (removeItem != null)
                    {
                        comboBoxItems.Remove(removeItem);
                    }

                    listItems.Remove(context);

                    task.RemoveAnswersItem(context.Text);
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
        #endregion
    }
}