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
        public static string EMAIL_KEY => T(Constants.EMAIL_KEY);
        public static string PASSWORD_KEY => T(Constants.PASSWORD_KEY);
        public static string SEND_KEY => T(Constants.SEND_KEY);
        public static string PROFILE_KEY => T(Constants.PROFILE_KEY); 
        public static string SETTINGS_KEY => T(Constants.SETTINGS_KEY);

        private static readonly ResourceManager m_resourceManager;
        
        static MessageProvider()
        {
            m_resourceManager = new(Constants.RESOURCE_MANAGER_NAME, Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// Get the localized string for the given key.
        /// </summary>
        /// <param name="key"></param>
        private static string T(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return Constants.KEY_NOT_FOUND;
            }

            return m_resourceManager.GetString(key, CultureInfo.CurrentCulture) ?? Constants.KEY_NOT_FOUND;
        }
    }
}