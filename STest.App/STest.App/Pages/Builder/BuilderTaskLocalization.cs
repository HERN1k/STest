using Microsoft.UI.Xaml;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml.Controls;
using System;
using STest.App.Utilities;
using STLib.Core.Testing;
using STLib.Tasks.Text;

namespace STest.App.Pages.Builder
{
    public sealed partial class BuilderPage : Page
    {
        #region Main
        private void UpdateTaskLocalization(int index)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(index, nameof(index));

            if (index >= TasksListView.Items.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), $"Index is out of range.");
            }

            var listViewItem = TasksListView
                    .ContainerFromIndex(index) as ListViewItem;

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

                        break;
                    case TaskType.Checkboxes:

                        break;
                    case TaskType.MultipleChoice:

                        break;
                }
            }
        }

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

        #region Default localization
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
                question.PlaceholderText = m_localization.GetString(Constants.THIS_SHOULD_BE_THE_QUESTION_KEY);

                if (!string.IsNullOrEmpty(data))
                {
                    question.Document.SetText(Microsoft.UI.Text.TextSetOptions.None, data);
                }
            }
        }

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
                correctAnswer.PlaceholderText = m_localization.GetString(Constants.THIS_SHOULD_BE_THE_CORRECT_ANSWER_KEY);

                if (!string.IsNullOrEmpty(data))
                {
                    correctAnswer.Document.SetText(Microsoft.UI.Text.TextSetOptions.None, data);
                }
            }
        }

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
                    maxGradeHeader.Text = m_localization.GetString(Constants.ASSESSMENT_FOR_CORRECT_ANSWER_KEY);
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
                    considerHeader.Text = m_localization.GetString(Constants.CONSIDER_IN_THE_ASSESSMENT_KEY);
                }

                ToggleSwitch? consider = considerElements.FirstOrDefault(item =>
                    FilterByTag(item, Constants.CONSIDER_IN_THE_ASSESSMENT_SWITCHER_TEST_BUILDER_KEY)) as ToggleSwitch;

                if (consider != null)
                {
                    consider.OffContent = m_localization.GetString(Constants.NO_KEY);
                    consider.OnContent = m_localization.GetString(Constants.YAS_KEY);

                    if (data != null)
                    {
                        consider.IsOn = (bool)data;
                    }
                }
            }
        }
        #endregion

        #region Localization and data
        
        #endregion
    }
}