namespace STLib.Tasks.Text
{
    public static class TextTaskExtension
    {
        public static TextTask AddName(this TextTask task, string name)
        {
            task.SetName(name);

            return task;
        }

        public static TextTask AddQuestion(this TextTask task, string question)
        {
            task.SetQuestion(question);

            return task;
        }

        public static TextTask AddConsider(this TextTask task, bool consider)
        {
            task.SetConsider(consider);

            return task;
        }

        public static TextTask AddMaxGrade(this TextTask task, int maxGrade)
        {
            task.SetMaxGrade(maxGrade);

            return task;
        }

        public static TextTask AddGrade(this TextTask task, int grade)
        {
            task.SetGrade(grade);

            return task;
        }

        public static TextTask AddCorrectAnswer(this TextTask task, string correctAnswer)
        {
            task.SetCorrectAnswer(correctAnswer);

            return task;
        }
    }
}