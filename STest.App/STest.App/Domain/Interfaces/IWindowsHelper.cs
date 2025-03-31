namespace STest.App.Domain.Interfaces
{
    /// <summary>
    /// OS Windows helper
    /// </summary>
    public interface IWindowsHelper : IService
    {
        /// <summary>
        /// Check if the current OS is Windows 11 or higher
        /// </summary>
        bool IsWindows11OrHigher();

        /// <summary>
        /// Check if the current OS is Windows 10 or higher
        /// </summary>
        bool IsWindowsVersionAtLeast(int major, int minor, int build);

        /// <summary>
        /// Check if the internet is available
        /// </summary>
        bool IsInternetAvailable();
    }
}