using Microsoft.UI.Xaml;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml.Controls;
using System;
using STest.App.Utilities;
using STLib.Core.Testing;
using STLib.Tasks.MultipleChoice;
using STLib.Tasks.Checkboxes;

namespace STest.App.Pages.Builder
{
    public sealed partial class BuilderPage : Page
    {
        #region Main
        /// <summary>
        /// Update localization of the task in the list view.
        /// </summary>
        /// <param name="index"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void UpdateTaskLocalization(int index)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(index, nameof(index));

            if (index >= TasksListView.Items.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), $"Index is out of range.");
            }

            var listViewItem = TasksListView.ContainerFromIndex(index) as ListViewItem;

            if (listViewItem?.ContentTemplateRoot is Border border)
            {
                if (listViewItem.Content is not CoreTask task)
                {
                    return;
                }

                switch (task.Type)
                {
                    case TaskType.Text:
                        if (task.IsNew)
                        {
                            SetDefaultTextTaskLocalization(border);
                        }
                        else
                        {
                            SetTextTaskLocalization(border, task, index);
                        }
                        break;
                    case TaskType.TrueFalse:
                        if (task.IsNew)
                        {
                            SetDefaultTrueFalseTaskLocalization(border);
                        }
                        else
                        {
                            SetTrueFalseTaskLocalization(border, task, index);
                        }
                        break;
                    case TaskType.Checkboxes:
                        if (task.IsNew)
                        {
                            SetDefaultCheckboxesTaskLocalization(border);
                        }
                        else
                        {
                            SetCheckboxesTaskLocalization(border, (CheckboxesTask)task, index);
                        }
                        break;
                    case TaskType.MultipleChoice:
                        if (task.IsNew)
                        {
                            SetDefaultMultipleСhoiceTaskLocalization(border);
                        }
                        else
                        {
                            SetMultipleСhoiceTaskLocalization(border, (MultipleChoiceTask)task, index);
                        }
                        break;
                }
            }
        }
        #endregion

        #region Text task
        /// <summary>
        /// Set default localization for the text task.
        /// </summary>
        /// <param name="border"></param>
        private void SetDefaultTextTaskLocalization(Border border)
        {
            if (border == null)
            {
                return;
            }

            try
            {
                UIElementCollection? uIElements = (border?.Child as StackPanel)?.Children;
                IEnumerable<FrameworkElement>? elements = uIElements?.OfType<FrameworkElement>();

                if (elements == null)
                {
                    return;
                }

                SetTaskHeaderNumber(elements, TasksList.Count);

                SetTaskQuestionLocalization(elements);

                SetTaskCorrectAnswerLocalization(elements);

                SetTaskMaxGradeLocalization(elements);

                SetTaskConsiderLocalization(elements);
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        /// <summary>
        /// Set localization for the text task.
        /// </summary>
        /// <param name="border"></param>
        /// <param name="task"></param>
        /// <param name="index"></param>
        private void SetTextTaskLocalization(Border border, CoreTask task, int index)
        {
            if (border == null || task == null)
            {
                return;
            }

            try
            {
                UIElementCollection? uIElements = (border?.Child as StackPanel)?.Children;
                IEnumerable<FrameworkElement>? elements = uIElements?.OfType<FrameworkElement>();

                if (elements == null)
                {
                    return;
                }
                
                SetTaskHeaderNumber(elements, index + 1);

                SetTaskQuestionLocalization(elements, task.Question);

                SetTaskCorrectAnswerLocalization(elements, task.CorrectAnswer);

                SetTaskMaxGradeLocalization(elements, task.MaxGrade);

                SetTaskConsiderLocalization(elements, task.Consider);
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        #endregion

        #region True/False task
        /// <summary>
        /// Set default localization for the true/false task.
        /// </summary>
        /// <param name="border"></param>
        private void SetDefaultTrueFalseTaskLocalization(Border border)
        {
            if (border == null)
            {
                return;
            }

            try
            {
                UIElementCollection? uIElements = (border?.Child as StackPanel)?.Children;
                IEnumerable<FrameworkElement>? elements = uIElements?.OfType<FrameworkElement>();

                if (elements == null)
                {
                    return;
                }

                SetTaskHeaderNumber(elements, TasksList.Count);

                SetTaskQuestionLocalization(elements);

                SetTrueFalseTaskCorrectAnswerLocalization(elements);

                SetTaskMaxGradeLocalization(elements);

                SetTaskConsiderLocalization(elements);
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        /// <summary>
        /// Set localization for the true/false task.
        /// </summary>
        /// <param name="border"></param>
        /// <param name="task"></param>
        /// <param name="index"></param>
        private void SetTrueFalseTaskLocalization(Border border, CoreTask task, int index)
        {
            if (border == null || task == null)
            {
                return;
            }

            try
            {
                UIElementCollection? uIElements = (border?.Child as StackPanel)?.Children;
                IEnumerable<FrameworkElement>? elements = uIElements?.OfType<FrameworkElement>();

                if (elements == null)
                {
                    return;
                }

                SetTaskHeaderNumber(elements, index + 1);

                SetTaskQuestionLocalization(elements, task.Question);

                SetTrueFalseTaskCorrectAnswerLocalization(elements, task.CorrectAnswer);

                SetTaskMaxGradeLocalization(elements, task.MaxGrade);

                SetTaskConsiderLocalization(elements, task.Consider);
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        #endregion

        #region Multiple сhoice task
        /// <summary>
        /// Set default localization for the multiple choice task.
        /// </summary>
        /// <param name="border"></param>
        private void SetDefaultMultipleСhoiceTaskLocalization(Border border)
        {
            if (border == null)
            {
                return;
            }

            try
            {
                UIElementCollection? uIElements = (border?.Child as StackPanel)?.Children;
                IEnumerable<FrameworkElement>? elements = uIElements?.OfType<FrameworkElement>();

                if (elements == null)
                {
                    return;
                }

                SetTaskHeaderNumber(elements, TasksList.Count);

                SetTaskQuestionLocalization(elements);

                SetMultipleChoiceTaskAnswersLocalization(elements);

                SetTaskMaxGradeLocalization(elements);

                SetTaskConsiderLocalization(elements);
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        /// <summary>
        /// Set localization for the multiple choice task.
        /// </summary>
        /// <param name="border"></param>
        /// <param name="task"></param>
        /// <param name="index"></param>
        private void SetMultipleСhoiceTaskLocalization(Border border, MultipleChoiceTask task, int index)
        {
            if (border == null || task == null)
            {
                return;
            }

            try
            {
                UIElementCollection? uIElements = (border?.Child as StackPanel)?.Children;
                IEnumerable<FrameworkElement>? elements = uIElements?.OfType<FrameworkElement>();

                if (elements == null)
                {
                    return;
                }

                SetTaskHeaderNumber(elements, index + 1);

                SetTaskQuestionLocalization(elements, task.Question);

                SetMultipleChoiceTaskAnswersLocalization(elements, task);

                SetTaskMaxGradeLocalization(elements, task.MaxGrade);

                SetTaskConsiderLocalization(elements, task.Consider);
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        #endregion

        #region Checkboxes task
        /// <summary>
        /// Set default localization for the checkboxes task.
        /// </summary>
        /// <param name="border"></param>
        private void SetDefaultCheckboxesTaskLocalization(Border border)
        {
            if (border == null)
            {
                return;
            }

            try
            {
                UIElementCollection? uIElements = (border?.Child as StackPanel)?.Children;
                IEnumerable<FrameworkElement>? elements = uIElements?.OfType<FrameworkElement>();

                if (elements == null)
                {
                    return;
                }

                SetTaskHeaderNumber(elements, TasksList.Count);

                SetTaskQuestionLocalization(elements);

                SetCheckBoxesTaskAnswersLocalization(elements);

                SetCheckBoxesTaskMaxGradeLocalization(elements);

                SetTaskConsiderLocalization(elements);
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        /// <summary>
        /// Set localization for the checkboxes task.
        /// </summary>
        /// <param name="border"></param>
        /// <param name="task"></param>
        /// <param name="index"></param>
        private void SetCheckboxesTaskLocalization(Border border, CheckboxesTask task, int index)
        {
            if (border == null || task == null)
            {
                return;
            }

            try
            {
                UIElementCollection? uIElements = (border?.Child as StackPanel)?.Children;
                IEnumerable<FrameworkElement>? elements = uIElements?.OfType<FrameworkElement>();

                if (elements == null)
                {
                    return;
                }

                SetTaskHeaderNumber(elements, index + 1);

                SetTaskQuestionLocalization(elements, task.Question);

                SetCheckBoxesTaskAnswersLocalization(elements, task);

                SetCheckBoxesTaskMaxGradeLocalization(elements);

                SetTaskConsiderLocalization(elements, task.Consider);
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        #endregion

        #region Default localization
        /// <summary>
        /// Set default localization for the task header number.
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="number"></param>
        private static void SetTaskHeaderNumber(IEnumerable<FrameworkElement> elements, int number)
        {
            if (elements == null || number < 0)
            {
                return;
            }

            Grid? headerPanel = elements.FirstOrDefault(item =>
                FilterByTag(item, Constants.HEADER_PANEL_TEST_BUILDER_KEY)) as Grid;

            if (headerPanel != null)
            {
                IEnumerable<FrameworkElement>? headerElements = headerPanel.Children.OfType<FrameworkElement>();

                TextBlock? numberElement = headerElements.FirstOrDefault(item =>
                    FilterByTag(item, Constants.HEADER_NUMBER_TEST_BUILDER_KEY)) as TextBlock;

                if (numberElement != null)
                {
                    numberElement.Text = string.Concat("№", number);
                }
            }
        }
        /// <summary>
        /// Set default localization for the task question.
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="data"></param>
        private void SetTaskQuestionLocalization(IEnumerable<FrameworkElement> elements, string? data = null)
        {
            if (elements == null)
            {
                return;
            }

            RichEditBox? question = elements.FirstOrDefault(item =>
                FilterByTag(item, Constants.QUESTION_TEST_BUILDER_KEY)) as RichEditBox;

            if (question != null)
            {
                question.Header = CreateHeader(Constants.QUESTION_KEY, TextAlignment.Center);
                question.PlaceholderText = T(Constants.THIS_SHOULD_BE_THE_QUESTION_KEY);

                if (!string.IsNullOrEmpty(data))
                {
                    if (data.StartsWith(Constants.NULL))
                    {
                        return;
                    }

                    question.Document.SetText(Microsoft.UI.Text.TextSetOptions.None, data);
                }
            }
        }
        /// <summary>
        /// Set default localization for the task correct answer.
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="data"></param>
        private void SetTaskCorrectAnswerLocalization(IEnumerable<FrameworkElement> elements, string? data = null)
        {
            if (elements == null)
            {
                return;
            }

            RichEditBox? correctAnswer = elements.FirstOrDefault(item =>
                FilterByTag(item, Constants.CORRECT_ANSWER_TEST_BUILDER_KEY)) as RichEditBox;

            if (correctAnswer != null)
            {
                correctAnswer.Header = CreateHeader(Constants.CORRECT_ANSWER_KEY, TextAlignment.Center);
                correctAnswer.PlaceholderText = T(Constants.THIS_SHOULD_BE_THE_CORRECT_ANSWER_KEY);

                if (!string.IsNullOrEmpty(data))
                {
                    if (data.StartsWith(Constants.NULL))
                    {
                        return;
                    }

                    correctAnswer.Document.SetText(Microsoft.UI.Text.TextSetOptions.None, data);
                }
            }
        }
        /// <summary>
        /// Set default localization for the task max grade.
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="data"></param>
        private void SetTaskMaxGradeLocalization(IEnumerable<FrameworkElement> elements, int? data = null)
        {
            if (elements == null)
            {
                return;
            }

            StackPanel? maxGradePanel = elements.FirstOrDefault(item =>
                FilterByTag(item, Constants.MAX_GRADE_PANEL_TEST_BUILDER_KEY)) as StackPanel;

            if (maxGradePanel != null)
            {
                var maxGradeHeader = maxGradePanel.Children
                    .OfType<FrameworkElement>()
                    .FirstOrDefault(item =>
                        FilterByTag(item, Constants.MAX_GRADE_TEST_BUILDER_KEY)) as TextBlock;

                if (maxGradeHeader != null)
                {
                    maxGradeHeader.Text = T(Constants.ASSESSMENT_FOR_CORRECT_ANSWER_KEY);
                }

                if (data != null && data > 0)
                {
                    var input = maxGradePanel.Children
                        .OfType<FrameworkElement>()
                        .FirstOrDefault(item =>
                            FilterByTag(item, Constants.MAX_GRADE_INPUT_TEST_BUILDER_KEY)) as NumberBox;

                    if (input != null)
                    {
                        input.Value = Convert.ToDouble((int)data);
                    }
                }
            }
        }
        /// <summary>
        /// Set default localization for the task consider.
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="data"></param>
        private void SetTaskConsiderLocalization(IEnumerable<FrameworkElement> elements, bool? data = null)
        {
            if (elements == null)
            {
                return;
            }

            StackPanel? considerPanel = elements.FirstOrDefault(item =>
                FilterByTag(item, Constants.CONSIDER_IN_THE_ASSESSMENT_PANEL_TEST_BUILDER_KEY)) as StackPanel;

            if (considerPanel != null)
            {
                IEnumerable<FrameworkElement>? considerElements = considerPanel.Children.OfType<FrameworkElement>();

                TextBlock? considerHeader = considerElements.FirstOrDefault(item =>
                    FilterByTag(item, Constants.CONSIDER_IN_THE_ASSESSMENT_TEST_BUILDER_KEY)) as TextBlock;

                if (considerHeader != null)
                {
                    considerHeader.Text = T(Constants.CONSIDER_IN_THE_ASSESSMENT_KEY);
                }

                ToggleSwitch? consider = considerElements.FirstOrDefault(item =>
                    FilterByTag(item, Constants.CONSIDER_IN_THE_ASSESSMENT_SWITCHER_TEST_BUILDER_KEY)) as ToggleSwitch;

                if (consider != null)
                {
                    consider.OffContent = T(Constants.NO_KEY);
                    consider.OnContent = T(Constants.YES_KEY);

                    if (data != null)
                    {
                        consider.IsOn = (bool)data;
                    }
                }
            }
        }
        #endregion

        #region True/False task localization
        /// <summary>
        /// Set default localization for the true/false task correct answer.
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="data"></param>
        private void SetTrueFalseTaskCorrectAnswerLocalization(IEnumerable<FrameworkElement> elements, string? data = null)
        {
            if (elements == null)
            {
                return;
            }

            TextBlock? header = elements.FirstOrDefault(item =>
                    FilterByTag(item, Constants.CORRECT_ANSWER_TEST_BUILDER_KEY)) as TextBlock;

            if (header != null)
            {
                header.Text = string.Concat(T(Constants.CORRECT_ANSWER_KEY), ":");
            }

            StackPanel? radioButtonsPanel = elements.FirstOrDefault(item =>
                FilterByTag(item, Constants.CORRECT_ANSWER_PANEL_TEST_BUILDER_KEY)) as StackPanel;

            if (radioButtonsPanel != null)
            {
                IEnumerable<FrameworkElement>? panelElements = radioButtonsPanel.Children.OfType<FrameworkElement>();

                RadioButton? trueButton = panelElements.FirstOrDefault(item =>
                    FilterByTag(item, Constants.CORRECT_ANSWER_RADIO_TRUE_TEST_BUILDER_KEY)) as RadioButton;

                RadioButton? falseButton = panelElements.FirstOrDefault(item =>
                    FilterByTag(item, Constants.CORRECT_ANSWER_RADIO_FALSE_TEST_BUILDER_KEY)) as RadioButton;

                var groupName = Guid.NewGuid().ToString();

                if (trueButton != null)
                {
                    trueButton.Content = T(Constants.YES_KEY);
                    trueButton.GroupName = groupName;
                }

                if (falseButton != null)
                {
                    falseButton.Content = T(Constants.NO_KEY);
                    falseButton.GroupName = groupName;
                }

                if (!string.IsNullOrEmpty(data))
                {
                    if (trueButton != null && falseButton != null)
                    {
                        if (Convert.ToBoolean(data))
                        {
                            trueButton.IsChecked = true;
                        }
                        else
                        {
                            falseButton.IsChecked = true;
                        }
                    }
                }
            }
        }
        #endregion

        #region Multiple сhoice task localization
        /// <summary>
        /// Set default localization for the multiple choice task correct answer.
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="task"></param>
        private void SetMultipleChoiceTaskAnswersLocalization(IEnumerable<FrameworkElement> elements, MultipleChoiceTask? task = null)
        {
            if (elements == null)
            {
                return;
            }

            ListView? answers = elements.FirstOrDefault(item =>
                FilterByTag(item, Constants.ANSWERS_LIST_TEST_BUILDER_KEY)) as ListView;

            if (answers != null)
            {
                answers.Header = CreateHeader(Constants.ANSWERS_KEY, TextAlignment.Center);
                answers.ItemsSource = new ExtendedObservableCollection<TestBuilderAnswersListViewItem>();

                if (task != null)
                {
                    (answers.ItemsSource as ExtendedObservableCollection<TestBuilderAnswersListViewItem>)
                        ?.AddRange(task.Answers.Select(item => new TestBuilderAnswersListViewItem(item, task.TaskID)));
                }
            }

            StackPanel? answersPanel = elements.FirstOrDefault(item =>
                FilterByTag(item, Constants.ADD_ANSWER_PANEL_TEST_BUILDER_KEY)) as StackPanel;

            if (answersPanel != null)
            {
                if (task != null && task.Answers.Length >= 4)
                {
                    answersPanel.Visibility = Visibility.Collapsed;
                }

                TextBox? input = answersPanel.Children
                    .OfType<FrameworkElement>()
                    .FirstOrDefault(item =>
                        FilterByTag(item, Constants.ADD_ANSWER_INPUT_TEST_BUILDER_KEY)) as TextBox;

                if (input != null)
                {
                    input.PlaceholderText = T(Constants.THIS_SHOULD_BE_THE_ANSWER_KEY);
                }
            }

            ComboBox? comboBox = elements.FirstOrDefault(item =>
                FilterByTag(item, Constants.ADD_ANSWER_COMBO_BOX_TEST_BUILDER_KEY)) as ComboBox;

            if (comboBox != null)
            {
                comboBox.Header = CreateHeader(Constants.CORRECT_ANSWER_KEY, TextAlignment.Center);
                comboBox.PlaceholderText = T(Constants.THIS_SHOULD_BE_THE_CORRECT_ANSWER_KEY);
                comboBox.ItemsSource = new ExtendedObservableCollection<string>();

                if (task != null)
                {
                    (comboBox.ItemsSource as ExtendedObservableCollection<string>)?.AddRange(task.Answers);

                    if (!string.IsNullOrEmpty(task.CorrectAnswer))
                    {
                        comboBox.SelectedValue = task.CorrectAnswer;
                    }
                }
            }
        }
        #endregion

        #region Checkboxes task localization
        /// <summary>
        /// Set default localization for the checkboxes task correct answer.
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="task"></param>
        private void SetCheckBoxesTaskAnswersLocalization(IEnumerable<FrameworkElement> elements, CheckboxesTask? task = null)
        {
            if (elements == null)
            {
                return;
            }

            ListView? answers = elements.FirstOrDefault(item =>
                FilterByTag(item, Constants.ANSWERS_LIST_TEST_BUILDER_KEY)) as ListView;

            if (answers != null)
            {
                answers.Header = CreateHeader(Constants.ANSWERS_KEY, TextAlignment.Center);
                answers.ItemsSource = new ExtendedObservableCollection<TestBuilderCheckBoxesListViewItem>();

                if (task != null)
                {
                    (answers.ItemsSource as ExtendedObservableCollection<TestBuilderCheckBoxesListViewItem>)
                        ?.AddRange(task.Answers.Select(item => new TestBuilderCheckBoxesListViewItem(item, task.TaskID)));
                }
            }

            StackPanel? answersPanel = elements.FirstOrDefault(item =>
                FilterByTag(item, Constants.ADD_ANSWER_PANEL_TEST_BUILDER_KEY)) as StackPanel;

            if (answersPanel != null)
            {
                if (task != null && task.Answers.Length >= 4)
                {
                    answersPanel.Visibility = Visibility.Collapsed;
                }

                TextBox? input = answersPanel.Children
                    .OfType<FrameworkElement>()
                    .FirstOrDefault(item =>
                        FilterByTag(item, Constants.ADD_ANSWER_INPUT_TEST_BUILDER_KEY)) as TextBox;

                if (input != null)
                {
                    input.PlaceholderText = T(Constants.THIS_SHOULD_BE_THE_ANSWER_KEY);
                }
            }

            ListView? checkBoxesList = elements.FirstOrDefault(item =>
                FilterByTag(item, Constants.CHACK_BOX_LIST_TEST_BUILDER_KEY)) as ListView;

            if (checkBoxesList != null)
            {
                checkBoxesList.Header = CreateHeader(Constants.CORRECT_ANSWERS_KEY, TextAlignment.Center);
                checkBoxesList.ItemsSource = new ExtendedObservableCollection<TestBuilderCheckBoxesListViewItem>();

                if (task != null)
                {
                    var checkBoxes = checkBoxesList.ItemsSource as ExtendedObservableCollection<TestBuilderCheckBoxesListViewItem>;
                    
                    if (checkBoxes == null)
                    {
                        return;
                    }

                    checkBoxes.AddRange(task.Answers.Select(item => new TestBuilderCheckBoxesListViewItem(item, task.TaskID)));

                    var correctAnswers = task.GetCorrectAnswers();

                    if (correctAnswers.Count > 0)
                    {
                        foreach (var correctAnswer in correctAnswers)
                        {
                            var checkBoxItem = checkBoxes
                                ?.FirstOrDefault(item => item.Text.Equals(correctAnswer));

                            if (checkBoxItem != null)
                            {
                                checkBoxItem.IsChecked = true;
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Set default localization for the checkboxes task max grade.
        /// </summary>
        /// <param name="elements"></param>
        private void SetCheckBoxesTaskMaxGradeLocalization(IEnumerable<FrameworkElement> elements)
        {
            if (elements == null)
            {
                return;
            }

            TextBlock? element = elements.FirstOrDefault(item =>
                FilterByTag(item, Constants.MAX_GRADE_TEST_BUILDER_KEY)) as TextBlock;

            if (element != null)
            {
                element.Text = T(Constants.EACH_CORRECT_ANS_WORTH_ONE_POINT_IN_FINAL_ASSESSMENT_KEY);
            }
        }
        #endregion
    }
}