using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.IO;
using System.Text.Json;

namespace STLib.Core
{
    public sealed class Test : IComparable, IComparable<Test>, IEquatable<Test>
    {
        public Guid TestID
        {
            get => m_testID;
            private set
            {
                if (value.Equals(Guid.Empty))
                {
                    throw new ArgumentNullException(nameof(TestID));
                }

                m_testID = value;
            }
        }
        public DateTime Created
        {
            get => m_created;
            private set
            {
                if (value.Equals(DateTime.MinValue) || value.Equals(DateTime.MaxValue))
                {
                    throw new ArgumentNullException(nameof(Created));
                }

                m_created = value;
            }
        }
        public List<CoreTask> Tasks
        {
            get => new List<CoreTask>(m_tasks);
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(Tasks));
                }

                if (value.Count == 0)
                {
                    throw new ArgumentException("Tasks cannot be empty.", nameof(Tasks));
                }

                if (value.Count > MaxTasks)
                {
                    throw new ArgumentOutOfRangeException(nameof(Tasks), $"Tasks cannot exceed {MaxTasks}.");
                }

                m_tasks.Clear();

                foreach (var task in value)
                {
                    if (task == null)
                    {
                        throw new ArgumentNullException(nameof(Tasks), "Task cannot be null.");
                    }

                    m_tasks.Add(task);
                }
            }
        }
        public int MaxTasks
        {
            get => m_maxTasks;
        }
        public int MaxGrade
        {
            get => m_maxGrade;
            private set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(MaxGrade), "MaxGrade cannot be negative.");
                }

                m_maxGrade = value;
            }
        }
        public int Grade
        {
            get => m_grade;
            private set
            {
                if (value < 0 || value > MaxGrade)
                {
                    throw new ArgumentOutOfRangeException(nameof(Grade), "Grade must be between 0 and MaxGrade.");
                }

                m_grade = value;
            }
        }
        public bool IsFinished
        {
            get => m_isFinished;
            private set => m_isFinished = value;
        }
        public TimeSpan TestTime
        {
            get => m_testTime;
            private set
            {
                if (value < TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException(nameof(TestTime), "TestTime cannot be negative.");
                }

                if (value > TimeSpan.FromHours(6))
                {
                    throw new ArgumentOutOfRangeException(nameof(TestTime), "TestTime cannot exceed 6 hours.");
                }

                m_testTime = value;
            }
        }
        public DateTime StartTime
        {
            get => m_startTime;
            private set
            {
                if (value.Equals(DateTime.MinValue) || value.Equals(DateTime.MaxValue))
                {
                    throw new ArgumentNullException(nameof(StartTime));
                }

                if (value < Created)
                {
                    throw new ArgumentOutOfRangeException(nameof(StartTime), "StartTime cannot be earlier than Created.");
                }

                m_startTime = value;
            }
        }
        public DateTime EndTime
        {
            get => m_endTime;
            private set
            {
                if (value.Equals(DateTime.MinValue) || value.Equals(DateTime.MaxValue))
                {
                    throw new ArgumentNullException(nameof(EndTime));
                }

                if (value < StartTime)
                {
                    throw new ArgumentOutOfRangeException(nameof(EndTime), "EndTime cannot be earlier than StartTime.");
                }

                m_endTime = value;
            }
        }
        public DateTime LastModified
        {
            get => m_lastModified;
            private set
            {
                if (value.Equals(DateTime.MinValue) || value.Equals(DateTime.MaxValue))
                {
                    throw new ArgumentNullException(nameof(LastModified));
                }

                if (value < Created)
                {
                    throw new ArgumentOutOfRangeException(nameof(LastModified), "LastModified cannot be earlier than Created.");
                }

                m_lastModified = value;
            }
        }
        public List<Guid> Modifiers
        {
            get => new List<Guid>(m_modifiers);
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(Modifiers));
                }

                m_modifiers.Clear();
                m_modifiers.AddRange(value);
            }
        }
        public List<Attention> Attentions
        {
            get => new List<Attention>(m_attentions);
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(Attentions));
                }

                m_attentions.Clear();
                m_attentions.AddRange(value);
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
        public string Description
        {
            get => m_description;
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(Description));
                }

                if (value.Length > 512)
                {
                    throw new ArgumentOutOfRangeException(nameof(Description), "Description cannot exceed 512 characters.");
                }

                if (value.Contains("NULL"))
                {
                    throw new ArgumentException("Description cannot contain \"NULL\".", nameof(Description));
                }

                m_description = value;
            }
        }
        public string Instructions
        {
            get => m_instructions;
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(Instructions));
                }

                if (value.Length > 512)
                {
                    throw new ArgumentOutOfRangeException(nameof(Instructions), "Instructions cannot exceed 512 characters.");
                }

                if (value.Contains("NULL"))
                {
                    throw new ArgumentException("Instructions cannot contain \"NULL\".", nameof(Instructions));
                }

                m_instructions = value;
            }
        }
        public Guid Creator
        {
            get => m_creator;
            private set
            {
                if (value.Equals(Guid.Empty))
                {
                    throw new ArgumentNullException(nameof(Creator));
                }

                m_creator = value;
            }
        }
        public List<Guid> Subjects
        {
            get => new List<Guid>(m_subjects);
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(Subjects));
                }

                if (value.Count == 0)
                {
                    throw new ArgumentException("Subjects cannot be empty.", nameof(Subjects));
                }

                m_subjects.Clear();
                m_subjects.AddRange(value);
            }
        }
        public bool IsReadOnly
        {
            get => m_isReadOnly;
            private set => m_isReadOnly = value;
        }

        private Guid m_testID = Guid.NewGuid();
        private DateTime m_created = DateTime.UtcNow;
        private readonly HashSet<CoreTask> m_tasks = new HashSet<CoreTask>();
        private readonly int m_maxTasks = 100;                                      // Dont change this value
        private int m_maxGrade = default;
        private int m_grade = default;
        private bool m_isFinished = default;
        private TimeSpan m_testTime = TimeSpan.Zero;
        private DateTime m_startTime = default;
        private DateTime m_endTime = default;
        private DateTime m_lastModified = DateTime.UtcNow;
        private readonly List<Guid> m_modifiers = new List<Guid>();
        private readonly List<Attention> m_attentions = new List<Attention>();
        private string m_name = "NULL";
        private string m_description = "NULL";
        private string m_instructions = "NULL";
        private Guid m_creator = Guid.Empty;
        private readonly List<Guid> m_subjects = new List<Guid>();
        private bool m_isReadOnly = default;
        private readonly JsonSerializerOptions m_jsonSerializerOptions = new JsonSerializerOptions
        {
#if DEBUG
            WriteIndented = true,
#else
            WriteIndented = false,
#endif
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never,
            IncludeFields = true
        };

        private Test(Guid creator)
        {
            Creator = creator;

            AddLastModifier(Creator);
        }

        [JsonConstructor]
#pragma warning disable IDE0051
        private Test(
            Guid testID, 
            DateTime created,
            List<CoreTask> tasks,
            int maxGrade,
            int grade,
            bool isFinished,
            TimeSpan testTime,
            DateTime startTime,
            DateTime endTime,
            DateTime lastModified,
            List<Guid> modifiers,
            List<Attention> attentions,
            string name,
            string description,
            string instructions,
            Guid creator,
            List<Guid> subjects)
#pragma warning restore IDE0051
        {
            TestID = testID;
            Created = created;
            Tasks = tasks;
            MaxGrade = maxGrade;
            Grade = grade;
            IsFinished = isFinished;
            TestTime = testTime;
            StartTime = startTime;
            EndTime = endTime;
            LastModified = lastModified;
            Modifiers = modifiers;
            Attentions = attentions;
            Name = name;
            Description = description;
            Instructions = instructions;
            Creator = creator;
            Subjects = subjects;
        }

        public static Test Build(Guid creator) => new Test(creator);

        public bool IsCorrect()
        {
            if (m_testID.Equals(Guid.Empty))
            {
                return false;
            }

            if (m_created.Equals(DateTime.MinValue) || m_created.Equals(DateTime.MaxValue))
            {
                return false;
            }

            if (m_tasks.Count == 0)
            {
                return false;
            }

            if (m_tasks.Count > m_maxTasks)
            {
                return false;
            }

            if (m_testTime <= TimeSpan.Zero)
            {
                return false;
            }

            if (m_modifiers.Count <= 0)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(m_name))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(m_description))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(m_instructions))
            {
                return false;
            }

            if (m_creator.Equals(Guid.Empty))
            {
                return false;
            }

            if (m_subjects.Count == 0)
            {
                return false;
            }

            return true;
        }

        public void OnStart(Guid modifier)
        {
            if (modifier.Equals(Guid.Empty))
            {
                throw new ArgumentNullException(nameof(modifier));
            }

            if (!Subjects.Contains(modifier))
            {
                throw new ArgumentException("You is not a subject of the test.", nameof(modifier));
            }

            if (IsReadOnly)
            { 
                throw new InvalidOperationException("Test is read-only.");
            }

            if (IsFinished)
            {
                throw new InvalidOperationException("Test is already finished.");
            }

            if (m_tasks.Count == 0)
            {
                throw new InvalidOperationException("Test has no tasks.");
            }

            foreach (var task in m_tasks)
            {
                task.Reset();

                if (!task.IsCorrect())
                {
                    throw new InvalidProgramException("Task is not valid.");
                }
            }

            StartTime = DateTime.UtcNow;

            AddLastModifier(modifier);
        }

        public void OnDevSave()
        {
            if (!IsCorrect())
            {
                throw new InvalidProgramException("Test is not valid.");
            }

            foreach (var task in m_tasks)
            {
                if (task.Consider)
                {
                    MaxGrade += task.MaxGrade;
                }

                task.Reset();

                if (!task.IsCorrect())
                {
                    throw new InvalidProgramException("Task is not valid.");
                }
            }

            Reset();

            AddLastModifier(Creator);
        }

        public void OnIntermediateStart(Guid modifier)
        {
            if (modifier.Equals(Guid.Empty))
            {
                throw new ArgumentNullException(nameof(modifier));
            }

            if (!Subjects.Contains(modifier))
            {
                throw new ArgumentException("You is not a subject of the test.", nameof(modifier));
            }

            if (IsReadOnly)
            {
                throw new InvalidOperationException("Test is read-only.");
            }

            if (IsFinished)
            {
                throw new InvalidOperationException("Test is already finished.");
            }

            if (m_tasks.Count == 0)
            {
                throw new InvalidOperationException("Test has no tasks.");
            }

            if (DateTime.UtcNow >= (StartTime + TestTime))
            {
                OnTimeOut(modifier);

                return;
            }

            AddLastModifier(modifier);
        }

        public void OnIntermediateSave(Guid modifier)
        {
            if (modifier.Equals(Guid.Empty))
            {
                throw new ArgumentNullException(nameof(modifier));
            }

            AddLastModifier(modifier);
        }

        public void OnSave(Guid modifier)
        {
            if (modifier.Equals(Guid.Empty))
            {
                throw new ArgumentNullException(nameof(modifier));
            }

            if (!Subjects.Contains(modifier))
            {
                throw new ArgumentException("You is not a subject of the test.", nameof(modifier));
            }

            if (IsReadOnly)
            {
                throw new InvalidOperationException("Test is read-only.");
            }

            if (IsFinished)
            {
                throw new InvalidOperationException("Test is already finished.");
            }

            if (m_tasks.Count == 0)
            {
                throw new InvalidOperationException("Test has no tasks.");
            }

            EndTime = DateTime.UtcNow;

            Grade = CalculateGrade();

            IsFinished = true;

            AddLastModifier(modifier);
        }

        public void OnTimeOut(Guid modifier)
        {
            if (modifier.Equals(Guid.Empty))
            {
                throw new ArgumentNullException(nameof(modifier));
            }

            if (!Subjects.Contains(modifier))
            {
                throw new ArgumentException("You is not a subject of the test.", nameof(modifier));
            }

            if (IsReadOnly)
            {
                throw new InvalidOperationException("Test is read-only.");
            }

            if (IsFinished)
            {
                throw new InvalidOperationException("Test is already finished.");
            }

            if (m_tasks.Count == 0)
            {
                throw new InvalidOperationException("Test has no tasks.");
            }

            EndTime = DateTime.UtcNow;

            Grade = CalculateGrade();

            IsFinished = true;

            Attentions.Add(new Attention("Test time is over / Час тестування закінчився", AttentionType.Critical));

            AddLastModifier(modifier);
        }

        public void AddLastModifier(Guid modifier)
        {
            if (modifier.Equals(Guid.Empty))
            {
                throw new ArgumentNullException(nameof(modifier));
            }

            var lastModifier = Modifiers.ElementAtOrDefault(m_modifiers.Count - 1);

            if (lastModifier.Equals(modifier))
            {
                return;
            }

            LastModified = DateTime.UtcNow;

            Modifiers.Add(modifier);
        }

        public void Reset()
        {
            Grade = default;
            IsFinished = default;
            StartTime = default;
            EndTime = default;
            Attentions = new List<Attention>();
            IsReadOnly = default;
        }

        private int CalculateGrade()
        {
            if (m_tasks.Count == 0)
            {
                return default;
            }

            var grade = 0;

            foreach (var task in m_tasks)
            {
                if (task.Consider)
                {
                    grade += task.Grade;
                }
            }

            return grade;
        }

        public string SerializeToJson()
        {
            return JsonSerializer.Serialize<Test>(this, m_jsonSerializerOptions);
        }

        public byte[] SerializeToByteArray()
        {
            using var stream = new MemoryStream();

            JsonSerializer.Serialize<Test>(stream, this, m_jsonSerializerOptions);

            return stream.ToArray();
        }

        public static Test? DeserializeFromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentNullException(nameof(json));
            }

            return JsonSerializer.Deserialize<Test>(json, new JsonSerializerOptions
            {
#if DEBUG
                WriteIndented = true,
#else
                WriteIndented = false,
#endif
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never,
                IncludeFields = true
            });
        }

        public static Test? DeserializeFromByteArray(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            using var stream = new MemoryStream(bytes);

            return JsonSerializer.Deserialize<Test>(stream, new JsonSerializerOptions
            {
#if DEBUG
                WriteIndented = true,
#else
                WriteIndented = false,
#endif
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never,
                IncludeFields = true
            });
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public void SetDescription(string description)
        {
            Description = description;
        }

        public void SetInstructions(string instructions)
        {
            Instructions = instructions;
        }
        
        public void SetGrade(int grade)
        {
            Grade = grade;
        }

        public void SetSubject(Guid subject)
        {
            var subjects = Subjects;

            subjects.Add(subject);

            Subjects = subjects;
        }

        public void SetTestTime(TimeSpan time)
        {
            TestTime = time;
        }

        public void SetAttention(Attention attention, Guid modifier)
        {
            var attentions = Attentions;

            attentions.Add(attention);

            Attentions = attentions;

            AddLastModifier(modifier);
        }

        public void SetTask(CoreTask task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            if (m_tasks.Contains(task))
            {
                throw new ArgumentException("Task already exists in the test.", nameof(task));
            }

            if (m_tasks.Count >= MaxTasks)
            {
                throw new InvalidOperationException("Cannot add more tasks to the test. Maximum limit reached.");
            }

            m_tasks.Add(task);
        }

        public void UnSetTask(Guid taskID)
        {
            if (taskID.Equals(Guid.Empty))
            {
                throw new ArgumentNullException(nameof(taskID));
            }

            var task = m_tasks.FirstOrDefault(t => t.TaskID.Equals(taskID));

            if (task == null)
            {
                return;
            }

            m_tasks.Remove(task);
        }

        public override bool Equals(object obj)
        {
            if (obj is Test test)
            {
                return TestID.Equals(test.TestID);
            }

            return false;
        }

        public bool Equals(Test other)
        {
            return TestID.Equals(other.TestID);
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;

            if (obj is Test otherTask)
            {
                return Created.CompareTo(otherTask.Created);
            }
            else
            {
                throw new ArgumentException("Object is not a Test");
            }
        }

        public int CompareTo(Test other)
        {
            if (other == null)
            {
                return 1;
            }

            return Created.CompareTo(other.Created);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"TestID: {TestID}, ");
            sb.AppendLine($"Tasks count: {Tasks.Count}, ");
            sb.AppendLine($"MaxTasks: {MaxTasks}, ");
            sb.AppendLine($"MaxGrade: {MaxGrade}, ");
            sb.AppendLine($"Grade: {Grade}, ");
            sb.AppendLine($"IsFinished: {IsFinished}, ");
            sb.AppendLine($"TestTime: {TestTime}, ");
            sb.AppendLine($"StartTime: {StartTime}, ");
            sb.AppendLine($"EndTime: {EndTime}, ");
            sb.AppendLine($"LastModified: {LastModified}, ");
            sb.AppendLine($"Modifiers count: {Modifiers.Count}, ");
            sb.AppendLine($"Attentions count: {Attentions.Count}, ");
            sb.AppendLine($"Name: {Name}, ");
            sb.AppendLine($"Description: {Description}, ");
            sb.AppendLine($"Instructions: {Instructions}, ");
            sb.AppendLine($"Creator: {Creator}, ");
            sb.AppendLine($"Subjects count: {Subjects.Count}, ");
            sb.Append($"IsReadOnly: {IsReadOnly}");
            
            return sb.ToString();
        }

        public override int GetHashCode()
        {
            return TestID.GetHashCode();
        }
    }
}