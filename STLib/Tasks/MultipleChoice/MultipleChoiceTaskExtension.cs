namespace STLib.Tasks.MultipleChoice
{
    public static class MultipleChoiceTaskExtension
    {
        public static MultipleChoiceTask AddName(this MultipleChoiceTask task, string name)
        {
            task.SetName(name);

            return task;
        }

        public static MultipleChoiceTask AddQuestion(this MultipleChoiceTask task, string question)
        {
            task.SetQuestion(question);

            return task;
        }

        public static MultipleChoiceTask AddConsider(this MultipleChoiceTask task, bool consider)
        {
            task.SetConsider(consider);

            return task;
        }

        public static MultipleChoiceTask AddMaxGrade(this MultipleChoiceTask task, int maxGrade)
        {
            task.SetMaxGrade(maxGrade);

            return task;
        }

        public static MultipleChoiceTask AddGrade(this MultipleChoiceTask task, int grade)
        {
            task.SetGrade(grade);

            return task;
        }

        public static MultipleChoiceTask AddCorrectAnswer(this MultipleChoiceTask task, string correctAnswer)
        {
            task.SetCorrectAnswer(correctAnswer);

            return task;
        }
        
        public static MultipleChoiceTask AddAnswerItem(this MultipleChoiceTask task, string answer)
        {
            task.SetAnswersItem(answer);

            return task;
        }
    }
}