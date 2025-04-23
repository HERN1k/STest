using Microsoft.UI.Xaml;

namespace STest.App.Domain.Interfaces
{
    /// <summary>
    /// OS Windows helper
    /// </summary>
    public interface IWindowsHelper : IService
    {
        /// <summary>
        /// Check if the internet is available
        /// </summary>
        bool IsInternetAvailable();

        /// <summary>
        /// Configure title bar for the current window
        /// </summary>
        void ConfigureTitleBar(Window currentWindow);
    }
}