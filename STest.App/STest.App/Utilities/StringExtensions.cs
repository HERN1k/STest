using System;

namespace STest.App.Utilities
{
    /// <summary>
    /// Extension methods for <see cref="string"/>
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Converts the first character of the string to uppercase.
        /// </summary>
        public static string ToTitleCase(this string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            text = text.Trim().ToLower();

            if (text.Length == 0)
            {
                return string.Empty;
            }

            char firstChar = char.ToUpperInvariant(text[0]);

            if (text.Length == 1)
            {
                return firstChar.ToString();
            }

            return string.Create(text.Length, (firstChar, text), (span, state) =>
            {
                span[0] = state.firstChar;
                state.text.AsSpan(1).CopyTo(span[1..]);
            });
        }

        /// <summary>
        /// Trims the string to the specified length.
        /// </summary>
        public static string TrimToLength(this string? text, int length)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(length, nameof(length));

            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            text = text.Trim();

            if (text.Length <= length)
            {
                return text;
            }

            return string.Create(length + 3, (text), (span, state) =>
            {
                state.AsSpan(0, length).CopyTo(span);
                span[length] = '.';
                span[length + 1] = '.';
                span[length + 2] = '.';
            });
        }
    }
}