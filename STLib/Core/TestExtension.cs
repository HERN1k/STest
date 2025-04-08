using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace STLib.Core
{
    public static class TestExtension
    {
        public static Test AddName(this Test test, string name)
        {
            test.SetName(name);

            return test;
        }

        public static Test AddDescription(this Test test, string description)
        {
            test.SetDescription(description);

            return test;
        }

        public static Test AddInstructions(this Test test, string instructions)
        {
            test.SetInstructions(instructions);

            return test;
        }

        public static Test AddGrade(this Test test, int grade)
        {
            test.SetGrade(grade);

            return test;
        }

        public static Test AddSubject(this Test test, Guid subject)
        {
            test.SetSubject(subject);

            return test;
        }

        public static Test AddTestTime(this Test test, TimeSpan time)
        {
            test.SetTestTime(time);

            return test;
        }

        public static Test AddAttention(this Test test, Attention attention, Guid modifier)
        {
            test.SetAttention(attention, modifier);

            return test;
        }

        public static Test AddTask(this Test test, CoreTask task)
        {
            test.SetTask(task);

            return test;
        }

        public static Test AddTasks(this Test test, IEnumerable<CoreTask> task)
        {
            foreach (var t in task)
            {
                test.SetTask(t);
            }

            return test;
        }

        public static Test RemoveTask(this Test test, Guid taskID)
        {
            test.UnSetTask(taskID);

            return test;
        }

        public static Test RemoveTasks(this Test test, IEnumerable<Guid> IDs)
        {
            foreach (var t in IDs)
            {
                test.UnSetTask(t);
            }

            return test;
        }
    }
}