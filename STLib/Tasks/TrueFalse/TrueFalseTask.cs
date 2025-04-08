using System;
using System.Text.Json.Serialization;
using STLib.Core;

namespace STLib.Tasks.TrueFalse
{
    public sealed class TrueFalseTask : CoreTask, IComparable, IComparable<TrueFalseTask>, IEquatable<TrueFalseTask>
    {
        private readonly string m_trueAnswer = "True";
        private readonly string m_falseAnswer = "False";

        private TrueFalseTask(TaskType taskType)
            : base(type: taskType)
        {
        }

        [JsonConstructor]
#pragma warning disable IDE0051
        private TrueFalseTask(Guid taskID, string name, string question, string correctAnswer, string answer, TaskType type, bool consider, bool isAnswered, int maxGrade, int grade)
#pragma warning restore IDE0051
            : base(taskID, name, question, correctAnswer, answer, type, consider, isAnswered, maxGrade, grade)
        {
        }

        public static TrueFalseTask Build() => new TrueFalseTask(TaskType.TrueFalse);

        public override bool IsCorrectTask()
        {
            if (!m_trueAnswer.Equals(this.CorrectAnswer, StringComparison.InvariantCulture) &&
                !m_falseAnswer.Equals(this.CorrectAnswer, StringComparison.InvariantCulture))
            {
                return false;
            }

            return base.IsCorrectTask();
        }

        public override bool IsCorrect()
        {
            return base.IsCorrect();
        }

        public override void SetAnswer(string answer)
        {
            if (string.IsNullOrEmpty(answer))
            {
                throw new ArgumentNullException(nameof(answer));
            }

            if (!m_trueAnswer.Equals(answer, StringComparison.InvariantCulture) &&
                !m_falseAnswer.Equals(answer, StringComparison.InvariantCulture))
            {
                throw new ArgumentException("Answer must be either \"True\" or \"False\".", nameof(answer));
            }

            this.Answer = answer.Trim();

            this.Grade = CalculateGrade(this.Answer);

            this.IsAnswered = true;
        }

        protected override int CalculateGrade(string answer)
        {
            if (!this.Consider)
            {
                return default;
            }

            if (string.IsNullOrWhiteSpace(answer))
            {
                return default;
            }

            if (answer.Equals(this.CorrectAnswer, StringComparison.InvariantCulture))
            {
                return this.MaxGrade;
            }
            else
            {
                return default;
            }
        }

        public override void SetCorrectAnswer(string correctAnswer)
        {
            correctAnswer = correctAnswer.Trim();

            if (!m_trueAnswer.Equals(correctAnswer, StringComparison.InvariantCulture) &&
                !m_falseAnswer.Equals(correctAnswer, StringComparison.InvariantCulture))
            {
                throw new ArgumentException("Correct answer must be either \"True\" or \"False\".", nameof(correctAnswer));
            }

            this.CorrectAnswer = correctAnswer;
        }

        public override bool Equals(object obj)
        {
            if (obj is TrueFalseTask task)
            {
                return this.TaskID.Equals(task.TaskID);
            }

            return false;
        }

        public bool Equals(TrueFalseTask other)
        {
            return this.TaskID.Equals(other.TaskID);
        }

        public int CompareTo(TrueFalseTask other)
        {
            if (other == null)
            {
                return 1;
            }

            return this.Grade.CompareTo(other.Grade);
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}