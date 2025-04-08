using System;
using System.Text.Json.Serialization;
using STLib.Core;

namespace STLib.Tasks.Text
{
    public sealed class TextTask : CoreTask, IComparable, IComparable<TextTask>, IEquatable<TextTask>
    {
        private TextTask(TaskType taskType)
            : base(type: taskType)
        {
        }

        [JsonConstructor]
#pragma warning disable IDE0051
        private TextTask(Guid taskID, string name, string question, string correctAnswer, string answer, TaskType type, bool consider, bool isAnswered, int maxGrade, int grade)
#pragma warning restore IDE0051
            : base(taskID, name, question, correctAnswer, answer, type, consider, isAnswered, maxGrade, grade)
        {
        }

        public static TextTask Build() => new TextTask(TaskType.Text);

        public override bool IsCorrectTask()
        {
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

            this.Answer = answer.Trim();

            this.Grade = CalculateGrade(Answer);

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

            if (answer.Equals(this.CorrectAnswer, StringComparison.InvariantCultureIgnoreCase))
            {
                return this.MaxGrade;
            }
            else
            {
                return default;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is TextTask task)
            {
                return this.TaskID.Equals(task.TaskID);
            }

            return false;
        }

        public bool Equals(TextTask other)
        {
            return this.TaskID.Equals(other.TaskID);
        }

        public int CompareTo(TextTask other)
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