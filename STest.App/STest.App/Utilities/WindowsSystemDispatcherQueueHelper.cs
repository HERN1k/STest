using System.Runtime.InteropServices;

namespace STest.App.Utilities
{
    /// <summary>
    /// Helper class to create a Windows system dispatcher queue controller
    /// </summary>
    public sealed class WindowsSystemDispatcherQueueHelper
    {
        /// <summary>
        /// Struct to hold the dispatcher queue options
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        struct DispatcherQueueOptions
        {
            internal int dwSize;
            internal int threadType;
            internal int apartmentType;
        }

        /// <summary>
        /// Create a dispatcher queue controller
        /// </summary>
        [DllImport("CoreMessaging.dll")]
        private static extern int CreateDispatcherQueueController([In] DispatcherQueueOptions options, [In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object dispatcherQueueController);

        /// <summary>
        /// The dispatcher queue controller
        /// </summary>
        private object? m_dispatcherQueueController = null;

        /// <summary>
        /// Ensure that a Windows system dispatcher queue controller is created
        /// </summary>
        public void EnsureWindowsSystemDispatcherQueueController()
        {
            if (Windows.System.DispatcherQueue.GetForCurrentThread() != null)
            {
                return;
            }

            if (m_dispatcherQueueController == null)
            {
                DispatcherQueueOptions options;
                options.dwSize = Marshal.SizeOf(typeof(DispatcherQueueOptions));
                options.threadType = 2;
                options.apartmentType = 2;
#pragma warning disable CS8601
#pragma warning disable IL2050
#pragma warning disable CA1806
                CreateDispatcherQueueController(options, ref m_dispatcherQueueController);
#pragma warning restore
            }
        }
    }
}