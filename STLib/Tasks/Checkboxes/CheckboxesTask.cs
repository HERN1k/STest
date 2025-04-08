using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using STLib.Core;

namespace STLib.Tasks.Checkboxes
{
    public sealed class CheckboxesTask : CoreTask, IComparable, IComparable<CheckboxesTask>, IEquatable<CheckboxesTask>
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
        public string[] UserAnswers
        {
            get => m_userAnswers.ToArray();
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(UserAnswers));
                }

                if (value.Length > m_maxAnswers)
                {
                    throw new ArgumentOutOfRangeException(nameof(UserAnswers), $"The number of user answers cannot exceed {m_maxAnswers}.");
                }

                m_userAnswers.Clear();
                m_userAnswers.AddRange(value);
            }
        }

        private readonly List<string> m_answers = new List<string>();
        private readonly List<string> m_userAnswers = new List<string>();
        private readonly int m_maxAnswers = 4;

        private CheckboxesTask(TaskType taskType)
            : base(type: taskType)
        {
        }

        [JsonConstructor]
#pragma warning disable IDE0051
        private CheckboxesTask(Guid taskID, string name, string question, string[] answers, string[] userAnswers, string correctAnswer, string answer, TaskType type, bool consider, bool isAnswered, int maxGrade, int grade)
#pragma warning restore IDE0051
            : base(taskID, name, question, correctAnswer, answer, type, consider, isAnswered, maxGrade, grade)
        {
            Answers = answers;
            UserAnswers = userAnswers;
        }

        public static CheckboxesTask Build() => new CheckboxesTask(TaskType.Checkboxes);

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

            var correctAnswersList = GetCorrectAnswers();

            if (correctAnswersList.Count == 0)
            {
                return false;
            }

            if (correctAnswersList.Count > m_maxAnswers)
            {
                return false;
            }

            foreach (var answer in correctAnswersList)
            {
                if (!m_answers.Contains(answer))
                {
                    return false;
                }
            }

            if (this.MaxGrade != correctAnswersList.Count)
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

            answer = answer.Trim();

            var userAnswersList = GetUserAnswers();

            if (userAnswersList.Contains(answer))
            {
                throw new ArgumentException($"The answer \"{answer}\" is already selected.");
            }

            if (userAnswersList.Count >= m_maxAnswers)
            {
                throw new ArgumentOutOfRangeException(nameof(answer), $"The number of answers cannot exceed {m_maxAnswers}.");
            }

            if (!m_answers.Contains(answer))
            {
                throw new ArgumentException($"The answer \"{answer}\" is not in the list of answers.");
            }

            userAnswersList.Add(answer);

            SetUserAnswers(userAnswersList);

            this.Grade = CalculateGrade(null);

            this.IsAnswered = true;
        }

        protected override int CalculateGrade(string? _)
        {
            if (!this.Consider)
            {
                return default;
            }

            var userAnswersList = GetUserAnswers();
            var correctAnswersList = GetCorrectAnswers();

            if (userAnswersList.Count == 0 || correctAnswersList.Count == 0)
            {
                return default;
            }

            int grade = 0;

            foreach (var answer in userAnswersList)
            {
                if (correctAnswersList.Contains(answer))
                {
                    grade += 1;
                }
            }

            return grade;
        }

        public override void SetCorrectAnswer(string correctAnswer)
        {
            base.SetCorrectAnswer(correctAnswer);
        }

        public void SetCorrectAnswers(List<string> correctAnswersList)
        {
            if (correctAnswersList == null)
            {
                throw new ArgumentNullException(nameof(correctAnswersList));
            }

            foreach (var answer in correctAnswersList)
            {
                if (!m_answers.Contains(answer))
                {
                    throw new ArgumentException($"The answer \"{answer}\" is not in the list of answers.");
                }
            }

            if (correctAnswersList.Count > m_maxAnswers)
            {
                throw new ArgumentOutOfRangeException(nameof(correctAnswersList), $"The number of correct answers cannot exceed {m_maxAnswers}.");
            }

            this.MaxGrade = correctAnswersList.Count;

            SetCorrectAnswer(JsonSerializer.Serialize<List<string>>(correctAnswersList));
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

        private List<string> GetUserAnswers()
        {
            var userAnswersList = JsonSerializer.Deserialize<List<string>>(this.Answer);

            if (userAnswersList == null)
            {
                userAnswersList = new List<string>();

                SetUserAnswers(userAnswersList);
            }

            return userAnswersList;
        }

        private void SetUserAnswers(List<string> userAnswersList)
        {
            if (userAnswersList == null)
            {
                throw new ArgumentNullException(nameof(userAnswersList));
            }

            this.Answer = JsonSerializer.Serialize<List<string>>(userAnswersList);
        }

        private List<string> GetCorrectAnswers()
        {
            var correctAnswersList = JsonSerializer.Deserialize<List<string>>(this.CorrectAnswer);

            if (correctAnswersList == null)
            {
                correctAnswersList = new List<string>();

                SetCorrectAnswers(correctAnswersList);
            }

            return correctAnswersList;
        }

        public override bool Equals(object obj)
        {
            if (obj is CheckboxesTask task)
            {
                return this.TaskID.Equals(task.TaskID);
            }

            return false;
        }

        public bool Equals(CheckboxesTask other)
        {
            return this.TaskID.Equals(other.TaskID);
        }

        public int CompareTo(CheckboxesTask other)
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
            var userAnswersList = GetUserAnswers();
            var correctAnswersList = GetCorrectAnswers();

            sb.AppendLine(string.Concat(base.ToString(), ", "));
            sb.AppendLine($"MaxAnswers: {m_maxAnswers}, ");
            sb.AppendLine($"Answers: ");

            for (int i = 0; i < m_maxAnswers; i++)
            {
                sb.AppendLine($"\t{i + 1}: {m_answers.ElementAtOrDefault(i)}");
            }

            sb.AppendLine($"CorrectAnswers: ");

            for (int i = 0; i < correctAnswersList.Count; i++)
            {
                sb.AppendLine($"\t{i + 1}: {correctAnswersList.ElementAtOrDefault(i)}");
            }

            sb.AppendLine($"UserAnswers: ");

            for (int i = 0; i < userAnswersList.Count; i++)
            {
                sb.AppendLine($"\t{i + 1}: {userAnswersList.ElementAtOrDefault(i)}");
            }

            return sb.ToString();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}