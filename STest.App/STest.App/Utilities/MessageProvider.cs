using System.Globalization;
using System.Reflection;
using System.Resources;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;

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
        public static string SAVE_KEY => T(Constants.SAVE_KEY);
        public static string ADD_NEW_TASK_KEY => T(Constants.ADD_NEW_TASK_KEY);
        public static string TEXT_KEY => T(Constants.TEXT_KEY);
        public static string TRUE_FALSE_KEY => T(Constants.TRUE_FALSE_KEY);
        public static string CHECKBOXES_KEY => T(Constants.CHECKBOXES_KEY);
        public static string MULTIPLE_CHOICE_KEY => T(Constants.MULTIPLE_CHOICE_KEY);
        public static string PREVIEW_KEY => T(Constants.PREVIEW_KEY);
        public static string EDITOR_KEY => T(Constants.EDITOR_KEY);
        public static string NAME_KEY => T(Constants.NAME_KEY);
        public static string DESCRIPTION_KEY => T(Constants.DESCRIPTION_KEY);
        public static string INSTRUCTIONS_KEY => T(Constants.INSTRUCTIONS_KEY);
        public static string TEST_TIME_KEY => T(Constants.TEST_TIME_KEY);
        public static string CREATE_NEW_TEST_KEY => T(Constants.CREATE_NEW_TEST_KEY);
        public static string LIST_OF_RECENT_TESTS_IS_EMPTY_KEY => T(Constants.LIST_OF_RECENT_TESTS_IS_EMPTY_KEY);
        public static string CONSIDER_IN_THE_ASSESSMENT_KEY => T(Constants.CONSIDER_IN_THE_ASSESSMENT_KEY);
        public static string THIS_SHOULD_BE_THE_QUESTION_KEY => T(Constants.THIS_SHOULD_BE_THE_QUESTION_KEY);
        public static string QUESTION_KEY => T(Constants.QUESTION_KEY);
        public static string CORRECT_ANSWER_KEY => T(Constants.CORRECT_ANSWER_KEY);
        public static string THIS_SHOULD_BE_THE_CORRECT_ANSWER_KEY => T(Constants.THIS_SHOULD_BE_THE_CORRECT_ANSWER_KEY);
        public static string ASSESSMENT_FOR_CORRECT_ANSWER_KEY => T(Constants.ASSESSMENT_FOR_CORRECT_ANSWER_KEY);
        public static string ANSWERS_KEY => T(Constants.ANSWERS_KEY);
        public static string CORRECT_ANSWERS_KEY => T(Constants.CORRECT_ANSWERS_KEY);
        public static string EACH_CORRECT_ANS_WORTH_ONE_POINT_IN_FINAL_ASSESSMENT_KEY => T(Constants.EACH_CORRECT_ANS_WORTH_ONE_POINT_IN_FINAL_ASSESSMENT_KEY);
        public static string REMOVE_KEY => T(Constants.REMOVE_KEY);
        public static string ACCESS_CODE_KEY => T(Constants.ACCESS_CODE_KEY);

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

        /// <summary>  
        /// Creates a header TextBlock with the specified localization key.  
        /// The text is aligned to the left by default.  
        /// </summary>  
        /// <param name="localizationKey">The key for localized text.</param>  
        /// <returns>A TextBlock with the localized text.</returns>  
        public static TextBlock ToHeader(this string text)
        {
            return new TextBlock()
            {
                Text = text,
                Style = (Style)Application.Current.Resources["SubtitleTextBlockStyle"],
                Margin = new Thickness(2, 0, 0, 0),
                TextAlignment = TextAlignment.Left
            };
        }

        /// <summary>
        /// Creates a header TextBlock with the specified localization key.
        /// </summary>
        /// <param name="localizationKey"></param>
        public static TextBlock ToHeaderCenter(this string text)
        {
            return new TextBlock()
            {
                Text = text,
                Style = (Style)Application.Current.Resources["SubtitleTextBlockStyle"],
                Margin = new Thickness(2, 0, 0, 0),
                TextAlignment = TextAlignment.Center
            };
        }
    }
}