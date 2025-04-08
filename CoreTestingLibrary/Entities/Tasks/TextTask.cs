using System.Runtime.Serialization;
using CoreTestingLibrary.Core;

namespace CoreTestingLibrary.Entities.Tasks
{
    [Serializable]
    public sealed class TextTask : CoreTask, ISerializable
    {
        public string CorrectAnswer 
        {
            get => m_correctAnswer;
            private set
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(CorrectAnswer));

                m_correctAnswer = value;
            }
        }
        public string Answer 
        {
            get => m_answer;
            private set
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(Answer));

                m_answer = value;
            }
        }
        
        private string m_correctAnswer;
        private string m_answer;

        private TextTask(string name = "NULL", string question = "NULL", string correctAnswer = "NULL", bool consider = true, int maxGrade = 1) 
            : base(name, question, TaskType.Text, consider, maxGrade)
        {
            m_correctAnswer = "NULL";
            m_answer = string.Empty;

            CorrectAnswer = correctAnswer;
            Answer = string.Empty;
        }

#pragma warning disable CS8618
        private TextTask(SerializationInfo info, StreamingContext context)
            : base(info, context)
#pragma warning restore CS8618
        {
            CorrectAnswer = info.GetString(nameof(CorrectAnswer)) ?? "NULL";
            Answer = info.GetString(nameof(Answer)) ?? "NULL";
        }

        public static TextTask Build() => new TextTask();

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(CorrectAnswer), CorrectAnswer);
            info.AddValue(nameof(Answer), Answer);
        }

        public override bool IsCorrectTask()
        {
            if (string.IsNullOrWhiteSpace(m_correctAnswer) ||
                m_correctAnswer.Equals("NULL", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            return base.IsCorrectTask();
        }

        public override bool IsCorrect()
        {
            return base.IsCorrect();
        }

        public override void SetAnswer<T>(T answer)
        {
            ArgumentNullException.ThrowIfNull(answer, nameof(answer));

            if (answer is string answerStr)
            {
                this.Answer = answerStr.Trim();

                this.Grade = CalculateGrade(this.Answer);

                this.IsAnswered = true;
            }
            else
            {
                throw new ArgumentException($"Invalid answer type: {answer.GetType().Name}. Expected string.", nameof(answer));
            }
        }

        protected override int CalculateGrade<T>(T answer)
        {
            if (answer is string answerStr)
            {
                if (answerStr.Equals(CorrectAnswer.Trim(), StringComparison.InvariantCultureIgnoreCase))
                {
                    return MaxGrade;
                }
                else
                {
                    return default;
                }
            }

            return default;
        }

        public void SetCorrectAnswer(string correctAnswer) => CorrectAnswer = correctAnswer;
    }

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