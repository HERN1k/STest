using System.Collections.Generic;

namespace STLib.Tasks.Checkboxes
{
    public static class CheckboxesTaskExtension
    {
        public static CheckboxesTask AddName(this CheckboxesTask task, string name)
        {
            task.SetName(name);

            return task;
        }

        public static CheckboxesTask AddQuestion(this CheckboxesTask task, string question)
        {
            task.SetQuestion(question);

            return task;
        }

        public static CheckboxesTask AddConsider(this CheckboxesTask task, bool consider)
        {
            task.SetConsider(consider);

            return task;
        }

        public static CheckboxesTask AddMaxGrade(this CheckboxesTask task, int maxGrade)
        {
            task.SetMaxGrade(maxGrade);

            return task;
        }

        public static CheckboxesTask AddGrade(this CheckboxesTask task, int grade)
        {
            task.SetGrade(grade);

            return task;
        }

        public static CheckboxesTask AddCorrectAnswers(this CheckboxesTask task, List<string> correctAnswers)
        {
            task.SetCorrectAnswers(correctAnswers);

            return task;
        }

        public static CheckboxesTask AddAnswerItem(this CheckboxesTask task, string answer)
        {
            task.SetAnswersItem(answer);

            return task;
        }
    }
}