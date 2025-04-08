namespace STLib.Tasks.TrueFalse
{
    public static class TrueFalseTaskExtension
    {
        public static TrueFalseTask AddName(this TrueFalseTask task, string name)
        {
            task.SetName(name);

            return task;
        }

        public static TrueFalseTask AddQuestion(this TrueFalseTask task, string question)
        {
            task.SetQuestion(question);

            return task;
        }

        public static TrueFalseTask AddConsider(this TrueFalseTask task, bool consider)
        {
            task.SetConsider(consider);

            return task;
        }

        public static TrueFalseTask AddMaxGrade(this TrueFalseTask task, int maxGrade)
        {
            task.SetMaxGrade(maxGrade);

            return task;
        }

        public static TrueFalseTask AddGrade(this TrueFalseTask task, int grade)
        {
            task.SetGrade(grade);

            return task;
        }

        public static TrueFalseTask AddCorrectAnswer(this TrueFalseTask task, string correctAnswer)
        {
            task.SetCorrectAnswer(correctAnswer);

            return task;
        }
    }
}