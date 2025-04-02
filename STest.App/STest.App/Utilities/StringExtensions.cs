using System;

using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using NLog;

using STest.App.Domain.Enums;

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

        /// <summary>
        /// Parses the language code to <see cref="Language"/>.
        /// </summary>
        public static Language ParseLanguageCode(this string? code)
        {
            return code switch
            {
                Constants.ENGLISH_LANGUAGE => Language.English,
                Constants.UKRAINIAN_LANGUAGE => Language.Ukrainian,
                Constants.HEBREW_LANGUAGE => Language.Hebrew,
                Constants.POLISH_LANGUAGE => Language.Polish,
                Constants.GERMAN_LANGUAGE => Language.German,
                Constants.RUSSIAN_LANGUAGE => Language.Russian,
                _ => Language.English,
            };
        }

        /// <summary>
        /// Gets the ISO code of the language
        /// </summary>
        public static string GetISOCode(this Language language)
        {
            return language switch
            {
                Language.English => Constants.ENGLISH_LANGUAGE.Split('-')[0],
                Language.Ukrainian => Constants.UKRAINIAN_LANGUAGE.Split('-')[0],
                Language.Hebrew => Constants.HEBREW_LANGUAGE.Split('-')[0],
                Language.Polish => Constants.POLISH_LANGUAGE.Split('-')[0],
                Language.German => Constants.GERMAN_LANGUAGE.Split('-')[0],
                Language.Russian => Constants.RUSSIAN_LANGUAGE.Split('-')[0],
                _ => Constants.ENGLISH_LANGUAGE.Split('-')[0]
            };
        }

        /// <summary>
        /// Formats the log event to a <see cref="Paragraph"/>.
        /// </summary>
        public static Paragraph FormattedLog(LogEventInfo log)
        {
            string date = log.TimeStamp.ToString("HH:mm:ss");
            string level = string.Concat(" [", log.Level.ToString().ToUpperInvariant(), "] ");
            string type = string.Concat(log.LoggerName.Split('.')[^1] ?? "Null", " ");
            string message = log.FormattedMessage;
            string stackTrace = string.Empty;

            if (log.Exception != null && (log.Level == NLog.LogLevel.Error || log.Level == NLog.LogLevel.Fatal))
            {
                stackTrace = string.Concat(
                    Environment.NewLine,
                    "\tStackTrace: ",
                    !string.IsNullOrEmpty(log.Exception.StackTrace)
                        ? log.Exception.StackTrace
                        : log.Exception.InnerException?.StackTrace ?? "Null");
            }

            var paragraph = new Paragraph()
            {
                Inlines =
                {
                    new Run() {
                        Text = date,
                        FontWeight = FontWeights.SemiBold,
                        Foreground = new SolidColorBrush(Colors.Black) { Opacity = 0.8 },
                    },
                    new Run() {
                        Text = level,
                        FontWeight = FontWeights.SemiBold,
                        Foreground = new SolidColorBrush(log.Level.Ordinal switch
                        {
                            0 => Colors.Gray,
                            1 => Colors.DimGray,
                            2 => Colors.YellowGreen,
                            3 => Colors.Orange,
                            4 => Colors.Red,
                            5 => Colors.DarkRed,
                            _ => Colors.Black
                        }) { Opacity = 1 }
                    },
                    new Run() {
                        Text = type,
                        FontWeight = FontWeights.SemiBold,
                        Foreground = new SolidColorBrush(Colors.Black) { Opacity = 0.8 }
                    },
                    new Run()
                    {
                        Text = message,
                        FontWeight = FontWeights.Light,
                        Foreground = new SolidColorBrush(Colors.Black) { Opacity = 1 }
                    }
                }
            };

            if (string.IsNullOrEmpty(stackTrace))
            {
                return paragraph;
            }

            paragraph.Inlines.Add(new Run()
            {
                Text = stackTrace,
                FontWeight = FontWeights.Light,
                Foreground = new SolidColorBrush(Colors.Black) { Opacity = 0.8 }
            });

            return paragraph;
        }
    }
}