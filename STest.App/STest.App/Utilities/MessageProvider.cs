using System.Globalization;
using System.Reflection;
using System.Resources;

namespace STest.App.Utilities
{
    public static class MessageProvider
    {
        public static string APP_DISPLAY_NAME_KEY => T(Constants.APP_DISPLAY_NAME_KEY);
        public static string YES => T(Constants.YES_KEY);
        public static string NO => T(Constants.NO_KEY);
        public static string THIS_SHOULD_BE_THE_ANSWER_KEY => T(Constants.THIS_SHOULD_BE_THE_ANSWER_KEY);
        public static string START_KEY => T(Constants.START_KEY);
        public static string COMPLETE_KEY => T(Constants.COMPLETE_KEY);

        private static readonly ResourceManager m_resourceManager;
        private static string T(string key) => m_resourceManager.GetString(key ?? string.Empty, CultureInfo.CurrentCulture) ?? Constants.KEY_NOT_FOUND;

        static MessageProvider()
        {
            m_resourceManager = new(Constants.RESOURCE_MANAGER_NAME, Assembly.GetExecutingAssembly());
        }
    }
}