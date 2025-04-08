using System;
using System.Text;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using STLib.Core;
using System.Linq;

namespace STLib.Tasks.MultipleChoice
{
    public sealed class MultipleChoiceTask : CoreTask, IComparable, IComparable<MultipleChoiceTask>, IEquatable<MultipleChoiceTask>
    {
        public string[] Answers
        {
            get => m_answers.ToArray();
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(Answers));
                }

                if (value.Length > m_maxAnswers)
                {
                    throw new ArgumentOutOfRangeException(nameof(Answers), $"The number of answers cannot exceed {m_maxAnswers}.");
                }

                m_answers.Clear();
                m_answers.AddRange(value);
            }
        }
        
        private readonly List<string> m_answers = new List<string>();
        private readonly int m_maxAnswers = 4;

        private MultipleChoiceTask(TaskType taskType)
            : base(type: taskType)
        {
        }

        [JsonConstructor]
#pragma warning disable IDE0051
        private MultipleChoiceTask(Guid taskID, string name, string question, string[] answers, string correctAnswer, string answer, TaskType type, bool consider, bool isAnswered, int maxGrade, int grade)
#pragma warning restore IDE0051
            : base(taskID, name, question, correctAnswer, answer, type, consider, isAnswered, maxGrade, grade)
        {
            Answers = answers;
        }

        public static MultipleChoiceTask Build() => new MultipleChoiceTask(TaskType.MultipleChoice);

        public override bool IsCorrectTask()
        {
            if (m_answers.Count == 0)
            {
                return false;
            }

            if (m_answers.Count > m_maxAnswers)
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

        public void SetAnswersItem(string answer)
        {
            answer = answer.Trim();

            if (m_answers.Contains(answer))
            {
                throw new ArgumentException($"The answer \"{answer}\" is already in the list of answers.");
            }

            if (m_answers.Count == m_maxAnswers)
            {
                throw new ArgumentOutOfRangeException(nameof(Answers), $"The number of answers cannot exceed {m_maxAnswers}.");
            }

            m_answers.Add(answer);
        }

        public override void SetCorrectAnswer(string correctAnswer)
        {
            correctAnswer = correctAnswer.Trim();

            if (!m_answers.Contains(correctAnswer))
            {
                throw new ArgumentException($"The answer \"{correctAnswer}\" is not in the list of answers.");
            }

            this.CorrectAnswer = correctAnswer;
        }

        public override bool Equals(object obj)
        {
            if (obj is MultipleChoiceTask task)
            {
                return this.TaskID.Equals(task.TaskID);
            }

            return false;
        }

        public bool Equals(MultipleChoiceTask other)
        {
            return this.TaskID.Equals(other.TaskID);
        }

        public int CompareTo(MultipleChoiceTask other)
        {
            if (other == null)
            {
                return 1;
            }

            return this.Grade.CompareTo(other.Grade);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(string.Concat(base.ToString(), ", "));
            sb.AppendLine($"MaxAnswers: {m_maxAnswers}, ");
            sb.AppendLine($"Answers: ");

            for (int i = 0; i < m_maxAnswers; i++)
            {
                sb.AppendLine($"\t{i + 1}: {m_answers.ElementAtOrDefault(i)}");
            }

            return sb.ToString();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}