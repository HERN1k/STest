using System;
using STest.App.Utilities;
using STest.App.Services;

namespace STest.App.Domain.Enums
{
    /// <summary>
    /// Represents the user rank of the application.
    /// </summary>
    public enum UserRank
    {
        /// <summary>
        /// Represents a student
        /// </summary>
        Student = 0,
        /// <summary>
        /// Represents a teacher
        /// </summary>
        Teacher = 2,
        /// <summary>
        /// Represents a admin
        /// </summary>
        Admin = 4
    }

    public static class UserRankExtensions
    {
        private static readonly string m_studentString = UserRank.Student.ToString();
        private static readonly string m_teacherString = UserRank.Teacher.ToString();
        private static readonly string m_adminString = UserRank.Admin.ToString();

        /// <summary>
        /// Converts the <see cref="UserRank"/> to a localization key string
        /// </summary>
        public static string ToStringLocalizationKey(this UserRank userRank)
        {
            return userRank switch
            {
                UserRank.Student => Constants.STUDENT_KEY,
                UserRank.Teacher => Constants.TEACHER_KEY,
                UserRank.Admin => Constants.ADMIN_KEY,
                _ => throw new ArgumentOutOfRangeException(nameof(userRank), userRank, null)
            };
        }

        /// <summary>
        /// Converts the <see cref="LocalData"/> key to a localization key string
        /// </summary>
        public static string ToStringLocalizationKey(this string userRank)
        {
            return userRank switch
            {
                nameof(UserRank.Student) => Constants.STUDENT_KEY,
                nameof(UserRank.Teacher) => Constants.TEACHER_KEY,
                nameof(UserRank.Admin) => Constants.ADMIN_KEY,
                _ => throw new ArgumentOutOfRangeException(nameof(userRank), userRank, null)
            };
        }

        /// <summary>
        /// Parses a string to a <see cref="UserRank"/> enum value
        /// </summary>
        public static UserRank ParseUserRank(this string userRank)
        {
            try
            {
                if (Enum.TryParse(userRank.AsSpan(), out UserRank parsedRank))
                {
                    return parsedRank;
                }
                else
                {
                    throw new ArgumentException($"Invalid user rank: {userRank}", nameof(userRank));
                }
            }
            catch (Exception ex)
            {
                ex.Show<App>();

                return UserRank.Student;
            }
        }
    }
}