using System;
using System.Text;
using System.Text.Json.Serialization;

namespace STLib.Core
{
    public abstract class CoreTask : IComparable, IComparable<CoreTask>, IEquatable<CoreTask>
    {
        public Guid TaskID 
        {
            get => m_taskID;
            private set
            {
                if (value.Equals(Guid.Empty))
                {
                    throw new ArgumentNullException(nameof(TaskID));
                }

                m_taskID = value;
            }
        }
        public string Name
        {
            get => m_name;
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(Name));
                }

                if (value.Length > 100)
                {
                    throw new ArgumentOutOfRangeException(nameof(Name), "Name cannot exceed 100 characters.");
                }

                if (value.Contains("NULL"))
                {
                    throw new ArgumentException("Name cannot contain \"NULL\".", nameof(Name));
                }

                m_name = value;
            }
        }
        public string Question
        {
            get => m_question;
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(Question));
                }

                if (value.Length > 256)
                {
                    throw new ArgumentOutOfRangeException(nameof(Question), "Question cannot exceed 256 characters.");
                }

                if (value.Contains("NULL"))
                {
                    throw new ArgumentException("Question cannot contain \"NULL\".", nameof(Question));
                }

                m_question = value;
            }
        }
        public string CorrectAnswer
        {
            get => m_correctAnswer;
            protected set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(CorrectAnswer));
                }

                m_correctAnswer = value;
            }
        }
        public string Answer
        {
            get => m_answer;
            protected set
            {
                m_answer = value ?? throw new ArgumentNullException(nameof(Answer));
            }
        }
        public TaskType Type
        {
            get => m_type;
            private set
            {
                if (value == TaskType.Unknown)
                {
                    throw new ArgumentException("The type of question cannot be unknown");
                }

                m_type = value;
            }
        }
        public bool Consider
        {
            get => m_consider;
            protected set => m_consider = value;
        }
        public bool IsAnswered
        {
            get => m_isAnswered;
            protected set => m_isAnswered = value;
        }
        public int MaxGrade
        {
            get => m_maxGrade;
            protected set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(MaxGrade));
                }

                m_maxGrade = value;
            }
        }
        public int Grade
        {
            get => m_grade;
            protected set
            {
                if (value < 0 || value > MaxGrade)
                {
                    throw new ArgumentOutOfRangeException(nameof(Grade));
                }

                m_grade = value;
            }
        }
        
        private Guid m_taskID = Guid.NewGuid();
        private string m_name = "NULL";
        private string m_question = "NULL";
        private string m_correctAnswer = "NULL";
        private string m_answer = string.Empty;
        private TaskType m_type = TaskType.Unknown;
        private bool m_consider = true;
        private bool m_isAnswered = default;
        private int m_maxGrade = 1;
        private int m_grade = default;

        protected CoreTask(TaskType type)
        {
            Type = type;
        }

        [JsonConstructor]
        protected CoreTask(Guid taskID, string name, string question, string correctAnswer, string answer, TaskType type, bool consider, bool isAnswered, int maxGrade, int grade)
        {
            TaskID = taskID;
            Name = name;
            Question = question;
            Type = type;
            Consider = consider;
            IsAnswered = isAnswered;
            MaxGrade = maxGrade;
            Grade = grade;
            CorrectAnswer = correctAnswer;
            Answer = answer;
        }

        public abstract void SetAnswer(string answer);

        protected abstract int CalculateGrade(string answer);

        public virtual bool IsCorrectTask()
        {
            if (string.IsNullOrWhiteSpace(m_name) ||
                m_name.Equals("NULL", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(m_question) ||
                m_question.Equals("NULL", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(m_correctAnswer) ||
                m_correctAnswer.Equals("NULL", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            if (m_type == TaskType.Unknown)
            {
                return false;
            }

            return true;
        }

        public virtual bool IsCorrect()
        {
            if (IsAnswered)
            {
                if (Grade > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public virtual void SetName(string name)
        {
            Name = name.Trim();
        }

        public virtual void SetQuestion(string question)
        {
            Question = question.Trim();
        }

        public virtual void SetCorrectAnswer(string correctAnswer)
        {
            CorrectAnswer = correctAnswer.Trim();
        }

        public virtual void SetConsider(bool consider)
        {
            Consider = consider;
        }

        public virtual void SetMaxGrade(int maxGrade)
        {
            MaxGrade = maxGrade;
        }

        public virtual void SetGrade(int grade)
        {
            Grade = grade;
        }

        public virtual void Reset()
        {
            Answer = string.Empty;
            IsAnswered = false;
            Grade = 0;

        }

        public override bool Equals(object obj)
        {
            if (obj is CoreTask task)
            {
                return TaskID.Equals(task.TaskID);
            }

            return false;
        }

        public bool Equals(CoreTask other)
        {
            return TaskID.Equals(other.TaskID);
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;

            if (obj is CoreTask otherTask)
            {
                return Grade.CompareTo(otherTask.Grade);
            }
            else
            {
                throw new ArgumentException("Object is not a CoreTask");
            }
        }

        public int CompareTo(CoreTask other)
        {
            if (other == null)
            {
                return 1;
            }

            return Grade.CompareTo(other.Grade);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"TaskID: {TaskID}, ");
            sb.AppendLine($"Name: {Name}, ");
            sb.AppendLine($"Question: {Question}, ");
            sb.AppendLine($"Type: {Type}, ");
            sb.AppendLine($"Consider: {Consider}, ");
            sb.AppendLine($"IsAnswered: {IsAnswered}, ");
            sb.AppendLine($"MaxGrade: {MaxGrade}, ");
            sb.AppendLine($"Grade: {Grade}, ");
            sb.AppendLine($"CorrectAnswer: {CorrectAnswer}, ");
            sb.Append($"Answer: {Answer}");

            return sb.ToString();
        }

        public override int GetHashCode()
        {
            return TaskID.GetHashCode();
        }
    }
}