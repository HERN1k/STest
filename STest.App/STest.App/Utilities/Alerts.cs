using System;
using System.Text;
using System.Diagnostics;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using STest.App.Domain.Interfaces;
using STest.App.AppWindows;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using System.Linq;
using STest.App.Pages.DialogContent;
using STest.App.Services;

namespace STest.App.Utilities
{
    /// <summary>
    /// A class that provides methods for displaying alerts to the user
    /// </summary>
    public static class Alerts
    {
        /// <summary>
        /// Constructor
        /// </summary>
        static Alerts()
        {
            new WindowsSystemDispatcherQueueHelper()
                .EnsureWindowsSystemDispatcherQueueController();
        }

        /// <summary>
        /// Show a dialog to the user
        /// </summary>
        /// <param name="page"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task<ContentDialogResult> ShowDialog(this Page page, ShowDialogArgs? args = null)
        {
            var dialog = new ContentDialog();
            
            dialog.XamlRoot = page.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Content = new ContentDialogContent(args?.Message);

            dialog.Title = args?.Title ?? nameof(args.Title);
            dialog.PrimaryButtonText = args?.OkButtonText ?? nameof(args.OkButtonText);
            dialog.CloseButtonText = args?.CancelButtonText ?? nameof(args.CancelButtonText);

            dialog.DefaultButton = ContentDialogButton.Primary;
            
            return await dialog.ShowAsync();
        }

        /// <summary>
        /// Show a dialog to the user and execute a function based on the result
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <param name="func"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task<T> ShowDialog<T>(this Page page, Func<ContentDialogResult, T> func, ShowDialogArgs? args = null)
        {
            ArgumentNullException.ThrowIfNull(func, nameof(func));

            var dialog = new ContentDialog();

            dialog.XamlRoot = page.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Content = new ContentDialogContent(args?.Message);

            dialog.Title = args?.Title ?? nameof(args.Title);
            dialog.PrimaryButtonText = args?.OkButtonText ?? nameof(args.OkButtonText);
            dialog.CloseButtonText = args?.CancelButtonText ?? nameof(args.CancelButtonText);

            dialog.DefaultButton = ContentDialogButton.Primary;

            var result = await dialog.ShowAsync();

            return func(result);
        }

        /// <summary>
        /// Execute the method in the dispatcher queue and in case of an error, reflect it in the UI
        /// </summary>
        public static Task<bool> ExecuteOnDispatcherQueueAsync<T>(ILogger<T>? logger, DispatcherAsyncHandler handler) where T : class
        {
            if (handler == null)
            {
                return Task.FromResult(false);
            }

            try
            {
                var tcs = new TaskCompletionSource<bool>();

                var result = DispatcherQueue.GetForCurrentThread().TryEnqueue(async () =>
                {
                    try
                    {
                        await handler();

                        tcs.SetResult(true);
                    }
                    catch (Exception ex)
                    {
                        ex.DispatcherQueueExceptionHendler(logger, tcs);
                    }
                });

                if (!result)
                {
                    tcs.SetResult(false);
                }

                return tcs.Task;
            }
            catch (Exception ex)
            {
                ex.DispatcherQueueExceptionHendler(logger);

                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// Execute the method in the dispatcher queue and in case of an error, reflect it in the UI
        /// </summary>
        public static Task<bool> ExecuteOnDispatcherQueueAsync<T>(this Page page, ILogger<T> logger, DispatcherAsyncHandler handler) where T : class
        {
            if (handler == null)
            {
                return Task.FromResult(false);
            }

            try
            {
                var tcs = new TaskCompletionSource<bool>();

                var result = DispatcherQueue.GetForCurrentThread().TryEnqueue(async () =>
                {
                    try
                    {
                        await handler();

                        tcs.SetResult(true);
                    }
                    catch (Exception ex)
                    {
                        ex.DispatcherQueueExceptionHendler(page, logger, tcs);
                    }
                });

                if (!result)
                {
                    tcs.SetResult(false);
                }

                return tcs.Task;
            }
            catch (Exception ex)
            {
                ex.DispatcherQueueExceptionHendler(page, logger);

                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// Show an exception to the UI
        /// </summary>
        public static void Show<T>(this Exception exception, Page page, ILogger<T> logger) where T : class
        {
            logger.LogError(exception, "An unexpected error occurred: {Message}", exception.Message);

            ShowAlert(
                page: page, 
                title: Constants.AN_UNEXPECTED_ERROR_OCCURRED,
                message: exception.Message,
                severity: InfoBarSeverity.Error);
        }

        /// <summary>
        /// Show an exception to the UI
        /// </summary>
        public static void Show<T>(this Exception exception, ILogger<T>? logger = null) where T : class
        {
            logger?.LogError(exception, "An unexpected error occurred: {Message}", exception.Message);

            ShowCriticalErrorWindow(exception);
        }

        /// <summary>
        /// Show an alert to the user
        /// </summary> 
        /// <exception cref="NotSupportedException"></exception>
        public static void ShowAlert(this Page page, string title, string message, InfoBarSeverity severity)
        {
            var infoBar = new InfoBar()
            {
                Title = title ?? string.Empty,
                Message = message ?? string.Empty,
                Severity = severity,
                IsOpen = true
            };

            bool isEnqueued = page.DispatcherQueue.TryEnqueue(() =>
            {
                if (page.Content is Grid grid)
                {
                    grid.Children.Add(infoBar);
                }
                else if (page.Content is StackPanel stack)
                {
                    stack.Children.Add(infoBar);
                }
            });
        }

        /// <summary>
        /// Show an alert to the user with a button
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        public static void ShowAlertWithButton(this Page page, string title, string message, Button button, InfoBarSeverity severity)
        {
            var infoBar = new InfoBar()
            {
                Title = title ?? string.Empty,
                Message = message ?? string.Empty,
                Severity = severity,
                IsOpen = true,
                ActionButton = button
            };

            bool isEnqueued = page.DispatcherQueue.TryEnqueue(() =>
            {
                if (page.Content is Grid grid)
                {
                    grid.Children.Add(infoBar);
                }
                else if (page.Content is StackPanel stack)
                {
                    stack.Children.Add(infoBar);
                }
            });
#if DEBUG
            if (!isEnqueued)
            {
                Debug.WriteLine(Constants.FAILED_TO_ADD_TASK_TO_UI_THREAD);
            }
#endif
        }

        /// <summary>
        /// Show an alert to the user with an exception
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        public static void ShowAlertException(this Page page, Exception ex, ILocalization? localization = null)
        {
            string title = localization?.GetString(Constants.ERROR_KEY) ?? Constants.ERROR_KEY;
            
            var infoBar = new InfoBar()
            {
                Title = title,
                Message = ex.Message ?? localization?.GetString(Constants.MESSAGE_KEY) ?? Constants.MESSAGE_KEY,
                Severity = InfoBarSeverity.Error,
                IsOpen = true
            };

            bool isEnqueued = page.DispatcherQueue.TryEnqueue(() =>
            {
                if (page.Content is Grid grid)
                {
                    grid.Children.Add(infoBar);
                }
                else if (page.Content is StackPanel stack)
                {
                    stack.Children.Add(infoBar);
                }
            });
#if DEBUG
            if (!isEnqueued)
            {
                Debug.WriteLine(Constants.FAILED_TO_ADD_TASK_TO_UI_THREAD);
            }
#endif
        }

        /// <summary>
        /// Show an alert to the user with an exception and stack trace
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void ShowAlertExceptionWithTrace(this Page page, Exception ex, ILocalization? localization = null)
        {
            bool isEnqueued = page.DispatcherQueue.TryEnqueue(async () =>
            {
                StringBuilder sb = new();
                string title = localization?.GetString(Constants.ERROR_KEY) ?? Constants.ERROR_KEY;
                string cancel = localization?.GetString(Constants.OK_KEY) ?? Constants.OK_KEY;
                sb.Append(localization?.GetString(Constants.MESSAGE_KEY) ?? Constants.MESSAGE_KEY);
                sb.Append(":    ");
                sb.Append(ex.Message);
                sb.Append($"\n\n{localization?.GetString(Constants.STACK_TRACE_KEY) ?? Constants.STACK_TRACE_KEY}:\n");
                sb.Append(ex.StackTrace);

                var dialog = new ContentDialog
                {
                    Title = title,
                    Content = sb.ToString(),
                    CloseButtonText = cancel,
                    XamlRoot = page.Content.XamlRoot
                };

                await dialog.ShowAsync();
            });
#if DEBUG
            if (!isEnqueued)
            {
                Debug.WriteLine(Constants.FAILED_TO_ADD_TASK_TO_UI_THREAD);
            }
#endif
        }

        /// <summary>
        /// Show a critical error window to the user
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void ShowCriticalErrorWindow(string message)
        {
            var dispatcher = DispatcherQueue.GetForCurrentThread();

            dispatcher.TryEnqueue(() =>
            {
                StringBuilder sb = new();
                string messageText = string.Concat(
                    Constants.MESSAGE_KEY,
                    ":\t", 
                    message);
                sb.AppendLine(messageText);

                var errorWindow = new ErrorWindow(sb.ToString());

                errorWindow.Activate();
            });
        }

        /// <summary>
        /// Show a critical error window to the user
        /// </summary>
        /// <param name="ex"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void ShowCriticalErrorWindow(Exception ex)
        {
            var appWindowDispatcherQueue = DispatcherQueue.GetForCurrentThread()
                ?? throw new InvalidOperationException(Constants.UNABLE_TO_ACCESS_DISPATCHERQUEUE);

            appWindowDispatcherQueue.TryEnqueue(() =>
            {
                StringBuilder sb = new();
                string messageText = string.Concat(
                    Constants.MESSAGE_KEY,
                    ":\t", 
                    ex.Message);
                sb.AppendLine(messageText);
                sb.AppendLine();
                sb.AppendLine($"{Constants.STACK_TRACE_KEY}:");
                sb.AppendLine(ex.StackTrace);

                var errorWindow = new ErrorWindow(sb.ToString());

                errorWindow.Activate();
            });
        }

        /// <summary>
        /// Handle exceptions in the dispatcher queue
        /// </summary>
        private static void DispatcherQueueExceptionHendler<T>(this Exception ex, ILogger<T>? logger, TaskCompletionSource<bool>? tcs = null) where T : class
        {
            if (Application.Current is App app)
            {
                var lastLog = app.LoggerTarget.Logs
                    .ElementAtOrDefault(^1);

                if ($"An unexpected error occurred: {ex.Message}"
                        .Equals(lastLog?.FormattedMessage, StringComparison.InvariantCultureIgnoreCase))
                {
                    if ((DateTime.Now - lastLog.TimeStamp) < TimeSpan.FromMilliseconds(500))
                    {
                        return;
                    }
                }

                ex.Show(logger);

                tcs?.SetResult(false);
            }
        }

        /// <summary>
        /// Handle exceptions in the dispatcher queue
        /// </summary>
        private static void DispatcherQueueExceptionHendler<T>(this Exception ex, Page page, ILogger<T> logger, TaskCompletionSource<bool>? tcs = null) where T : class
        {
            if (Application.Current is App app)
            {
                var lastLog = app.LoggerTarget.Logs
                    .ElementAtOrDefault(^1);

                if ($"An unexpected error occurred: {ex.Message}"
                        .Equals(lastLog?.FormattedMessage, StringComparison.InvariantCultureIgnoreCase))
                {
                    if ((DateTime.Now - lastLog.TimeStamp) < TimeSpan.FromMilliseconds(500))
                    {
                        return;
                    }
                }

                ex.Show(page, logger);

                tcs?.SetResult(false);
            }
        }
    }
}