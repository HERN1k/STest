using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using STLib.Tasks.Checkboxes;
using STLib.Tasks.MultipleChoice;
using STLib.Tasks.Text;
using STLib.Tasks.TrueFalse;

namespace STest.App.Utilities
{
    public partial class TaskTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TextTaskTemplate { get; set; } = null!;
        public DataTemplate TrueFalseTaskTemplate { get; set; } = null!;
        public DataTemplate CheckboxesTaskTemplate { get; set; } = null!;
        public DataTemplate MultipleChoiceTaskTemplate { get; set; } = null!;

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is TextTask)
            {
                return TextTaskTemplate;
            }
            else if (item is TrueFalseTask)
            {
                return TrueFalseTaskTemplate;
            }
            else if (item is CheckboxesTask)
            {
                return CheckboxesTaskTemplate;
            }
            else if (item is MultipleChoiceTask)
            {
                return MultipleChoiceTaskTemplate;
            }

            return base.SelectTemplateCore(item);
        }
    }
}