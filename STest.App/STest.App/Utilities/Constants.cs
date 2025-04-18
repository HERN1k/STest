﻿using System;
using Microsoft.UI.Xaml;
using STest.App.Services;

namespace STest.App.Utilities
{
    /// <summary>
    /// Constants strings for the application
    /// </summary>
    public sealed class Constants
    {
        /* 
            Exceptions 
         */
        /// <summary>
        /// <see cref="Exception"/> message <see cref="string"/>
        /// </summary>
        public const string AN_UNEXPECTED_ERROR_OCCURRED = "An unexpected error occurred";
        /// <summary>
        /// <see cref="Exception"/> message <see cref="string"/>
        /// </summary>
        public const string MAIN_WINDOW_RESOLUTION_ERROR_MESSAGE = "Failed to get MainWindow instance from dependency container";
        /// <summary>
        /// <see cref="Exception"/> message <see cref="string"/>
        /// </summary>
        public const string INTRODUCTION_WINDOW_RESOLUTION_ERROR_MESSAGE = "Failed to get IntroductionWindow instance from dependency container";
        /// <summary>
        /// <see cref="Exception"/> message <see cref="string"/>
        /// </summary>
        public const string LOGGER_RESOLUTION_ERROR_MESSAGE = "Failed to get ILogger<App> instance from dependency container";
        /// <summary>
        /// <see cref="Exception"/> message <see cref="string"/>
        /// </summary>
        public const string FAILED_TO_RETRIEVE_THE_WINDOWS_VERSION = "Failed to retrieve the Windows version from AnalyticsInfo.VersionInfo.DeviceFamilyVersion";
        /// <summary>
        /// <see cref="Exception"/> message <see cref="string"/>
        /// </summary>
        public const string INVALID_HEX_FORMAT = "Invalid hex format. Use #RRGGBB or #RRGGBBAA";
        /// <summary>
        /// <see cref="Exception"/> message <see cref="string"/>
        /// </summary>
        public const string APPLICATION_RESOURCES_ARE_NOT_INITIALIZED = "Application resources are not initialized";
        /// <summary>
        /// <see cref="Exception"/> message <see cref="string"/>
        /// </summary>
        public const string PAGE_IS_NOT_REGISTER_IN_DI = "page is not registered in the DI container";
        /// <summary>
        /// <see cref="Exception"/> message <see cref="string"/>
        /// </summary>
        public const string FAILED_SET_APPLICATION_CULTURE = "Failed to set application culture";
        /// <summary>
        /// <see cref="Exception"/> message <see cref="string"/>
        /// </summary>
        public const string USER_NOT_FOUND = "User data not found";
        /// <summary>
        /// <see cref="Exception"/> message <see cref="string"/>
        /// </summary>
        public const string CULTURE_CODE_NOT_FOUND_OR_INVALID = "Culture code not found or invalid";
        /// <summary>
        /// <see cref="Exception"/> message <see cref="string"/>
        /// </summary>
        public const string FAILED_TO_LOAD_RESOURCES_FOR_NEW_CULTURE = "Failed to load resources for new culture";
        /// <summary>
        /// <see cref="Exception"/> message <see cref="string"/>
        /// </summary>
        public const string FAILED_TO_ADD_TASK_TO_UI_THREAD = "Failed to add task to UI thread";
        /// <summary>
        /// <see cref="Exception"/> message <see cref="string"/>
        /// </summary>
        public const string UNABLE_TO_ACCESS_DISPATCHERQUEUE = "Unable to access DispatcherQueue";
        /// <summary>
        /// <see cref="Exception"/> message <see cref="string"/>
        /// </summary>
        public const string REQUEST_FAILED_AFTER_MULTIPLE_ATTEMPTS = "Request failed after multiple attempts";
        /// <summary>
        /// <see cref="Exception"/> message <see cref="string"/>
        /// </summary>
        public const string NO_DATA_RECEIVED = "No data received";
        /// <summary>
        /// <see cref="Exception"/> message <see cref="string"/>
        /// </summary>
        public const string INVALID_FILE_NAME = "Invalid file name";
        /// <summary>
        /// <see cref="Exception"/> message <see cref="string"/>
        /// </summary>
        public const string FILE_NOT_EXISTS = "File not exists";
        /// <summary>
        /// <see cref="Exception"/> message <see cref="string"/>
        /// </summary>
        public const string ERROR_READING_FILE = "Error reading file";
        /// <summary>
        /// <see cref="Exception"/> message <see cref="string"/>
        /// </summary>
        public const string INVALID_DIRECTORY_PATH = "Invalid directory path";
        /// <summary>
        /// <see cref="Exception"/> message <see cref="string"/>
        /// </summary>
        public const string ERROR_IMPORTING_DUMP = "Error importing dump";

        /* 
            Main
         */
        /// <summary>
        /// <see cref="Application"/> message <see cref="string"/>
        /// </summary>
        public const string APP_DISPLAY_NAME = "Student Testing Platform";
        /// <summary>
        /// <see cref="Application"/> message <see cref="string"/>
        /// </summary>
        public const string CURRENT_CULTURE = "CurrentCulture";
        /// <summary>
        /// <see cref="Application"/> message <see cref="string"/>
        /// </summary>
        public const string APP_LOCAL_DATA_FILE_NAME = "local-data.json";
        /// <summary>
        /// <see cref="Application"/> message <see cref="string"/>
        /// </summary>
        public const string APP_LOGS_FILE_NAME = "logs.txt";
        /// <summary>
        /// <see cref="Application"/> message <see cref="string"/>
        /// </summary>
        public const string APP_INITIALIZED = "The application has been initialized";
        /// <summary>
        /// <see cref="Application"/> message <see cref="string"/>
        /// </summary>
        public const string ENGLISH_LANGUAGE = "en-US";
        /// <summary>
        /// <see cref="Application"/> message <see cref="string"/>
        /// </summary>
        public const string UKRAINIAN_LANGUAGE = "uk-UA";
        /// <summary>
        /// <see cref="Application"/> message <see cref="string"/>
        /// </summary>
        public const string RUSSIAN_LANGUAGE = "ru-RU";
        /// <summary>
        /// <see cref="Application"/> message <see cref="string"/>
        /// </summary>
        public const string POLISH_LANGUAGE = "pl-PL";
        /// <summary>
        /// <see cref="Application"/> message <see cref="string"/>
        /// </summary>
        public const string GERMAN_LANGUAGE = "de-DE";
        /// <summary>
        /// <see cref="Application"/> message <see cref="string"/>
        /// </summary>
        public const string HEBREW_LANGUAGE = "he-IL";
        /// <summary>
        /// <see cref="Application"/> message <see cref="string"/>
        /// </summary>
        public const string NULL = nameof(NULL);
        /// <summary>
        /// <see cref="Application"/> message <see cref="string"/>
        /// </summary>
        public const string CURRENT_TEST_IN_BUILDER = nameof(CURRENT_TEST_IN_BUILDER);
        /// <summary>
        /// <see cref="Application"/> message <see cref="string"/>
        /// </summary>
        public const string HEADER_PANEL_TEST_BUILDER_KEY = "HEADER_PANEL";
        /// <summary>
        /// <see cref="Application"/> message <see cref="string"/>
        /// </summary>
        public const string HEADER_NUMBER_TEST_BUILDER_KEY = "HEADER_NUMBER";
        /// <summary>
        /// <see cref="Application"/> message <see cref="string"/>
        /// </summary>
        public const string HEADER_CLOSE_TEST_BUILDER_KEY = "HEADER_CLOSE";
        /// <summary>
        /// <see cref="Application"/> message <see cref="string"/>
        /// </summary>
        public const string QUESTION_TEST_BUILDER_KEY = "QUESTION";
        /// <summary>
        /// <see cref="Application"/> message <see cref="string"/>
        /// </summary>
        public const string CORRECT_ANSWER_TEST_BUILDER_KEY = "CORRECT_ANSWER";
        /// <summary>
        /// <see cref="Application"/> message <see cref="string"/>
        /// </summary>
        public const string CORRECT_ANSWER_PANEL_TEST_BUILDER_KEY = "CORRECT_ANSWER_PANEL";
        /// <summary>
        /// <see cref="Application"/> message <see cref="string"/>
        /// </summary>
        public const string CORRECT_ANSWER_RADIO_TRUE_TEST_BUILDER_KEY = "CORRECT_ANSWER_RADIO_TRUE";
        /// <summary>
        /// <see cref="Application"/> message <see cref="string"/>
        /// </summary>
        public const string CORRECT_ANSWER_RADIO_FALSE_TEST_BUILDER_KEY = "CORRECT_ANSWER_RADIO_FALSE";
        /// <summary>
        /// <see cref="Application"/> message <see cref="string"/>
        /// </summary>
        public const string MAX_GRADE_TEST_BUILDER_KEY = "MAX_GRADE";
        /// <summary>
        /// <see cref="Application"/> message <see cref="string"/>
        /// </summary>
        public const string MAX_GRADE_PANEL_TEST_BUILDER_KEY = "MAX_GRADE_PANEL";
        /// <summary>
        /// <see cref="Application"/> message <see cref="string"/>
        /// </summary>
        public const string MAX_GRADE_INPUT_TEST_BUILDER_KEY = "MAX_GRADE_INPUT";
        /// <summary>
        /// <see cref="Application"/> message <see cref="string"/>
        /// </summary>
        public const string CONSIDER_IN_THE_ASSESSMENT_TEST_BUILDER_KEY = "CONSIDER_IN_THE_ASSESSMENT";
        /// <summary>
        /// <see cref="Application"/> message <see cref="string"/>
        /// </summary>
        public const string CONSIDER_IN_THE_ASSESSMENT_SWITCHER_TEST_BUILDER_KEY = "CONSIDER_IN_THE_ASSESSMENT_SWITCHER";
        /// <summary>
        /// <see cref="Application"/> message <see cref="string"/>
        /// </summary>
        public const string CONSIDER_IN_THE_ASSESSMENT_PANEL_TEST_BUILDER_KEY = "CONSIDER_IN_THE_ASSESSMENT_PANEL";
        /// <summary>
        /// <see cref="Application"/> message <see cref="string"/>
        /// </summary>
        public const string ANSWERS_LIST_TEST_BUILDER_KEY = "ANSWERS_LIST";
        /// <summary>
        /// <see cref="Application"/> message <see cref="string"/>
        /// </summary>
        public const string ADD_ANSWER_PANEL_TEST_BUILDER_KEY = "ADD_ANSWER_PANEL";
        /// <summary>
        /// <see cref="Application"/> message <see cref="string"/>
        /// </summary>
        public const string ADD_ANSWER_INPUT_TEST_BUILDER_KEY = "ADD_ANSWER_INPUT";
        /// <summary>
        /// <see cref="Application"/> message <see cref="string"/>
        /// </summary>
        public const string ADD_ANSWER_ADD_BUTTON_TEST_BUILDER_KEY = "ADD_ANSWER_ADD_BUTTON";
        /// <summary>
        /// <see cref="Application"/> message <see cref="string"/>
        /// </summary>
        public const string ADD_ANSWER_COMBO_BOX_TEST_BUILDER_KEY = "ADD_ANSWER_COMBO_BOX";
        /// <summary>
        /// <see cref="Application"/> message <see cref="string"/>
        /// </summary>
        public const string CHACK_BOX_LIST_TEST_BUILDER_KEY = "CHACK_BOX_LIST";

        /*
            Localization keys
         */
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string APP_DISPLAY_NAME_KEY = nameof(APP_DISPLAY_NAME_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string ERROR_KEY = nameof(ERROR_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string FAILED_ADD_TASK_TO_UI_THREAD_KEY = nameof(FAILED_ADD_TASK_TO_UI_THREAD_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string MESSAGE_KEY = nameof(MESSAGE_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string OK_KEY = nameof(OK_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string STACK_TRACE_KEY = nameof(STACK_TRACE_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string METHOD_KEY = nameof(METHOD_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string PROPERTY_NOT_CHANGED_KEY = nameof(PROPERTY_NOT_CHANGED_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string EMAIL_KEY = nameof(EMAIL_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string PASSWORD_KEY = nameof(PASSWORD_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string SEND_KEY = nameof(SEND_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string SUCCESS_KEY = nameof(SUCCESS_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string COPIED_KEY = nameof(COPIED_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string SETTINGS_KEY = nameof(SETTINGS_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string APPLICATION_LANGUAGE_KEY = nameof(APPLICATION_LANGUAGE_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string ENGLISH_KEY = nameof(ENGLISH_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string UKRAINIAN_KEY = nameof(UKRAINIAN_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string PROFILE_KEY = nameof(PROFILE_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string STUDENT_KEY = nameof(STUDENT_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string TEACHER_KEY = nameof(TEACHER_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string ADMIN_KEY = nameof(ADMIN_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string TESTS_CONFIGURATOR_KEY = nameof(TESTS_CONFIGURATOR_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string LAST_TEST_KEY = nameof(LAST_TEST_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string LIST_OF_RECENT_TESTS_IS_EMPTY_KEY = nameof(LIST_OF_RECENT_TESTS_IS_EMPTY_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string CREATE_NEW_TEST_KEY = nameof(CREATE_NEW_TEST_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string THIS_SHOULD_BE_THE_NAME_KEY = nameof(THIS_SHOULD_BE_THE_NAME_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string THIS_SHOULD_BE_THE_DESCRIPTION_KEY = nameof(THIS_SHOULD_BE_THE_DESCRIPTION_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string THIS_SHOULD_BE_THE_INSTRUCTIONS_KEY = nameof(THIS_SHOULD_BE_THE_INSTRUCTIONS_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string THIS_SHOULD_BE_THE_QUESTION_KEY = nameof(THIS_SHOULD_BE_THE_QUESTION_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string THIS_SHOULD_BE_THE_CORRECT_ANSWER_KEY = nameof(THIS_SHOULD_BE_THE_CORRECT_ANSWER_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string THIS_SHOULD_BE_THE_ANSWER_KEY = nameof(THIS_SHOULD_BE_THE_ANSWER_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string NAME_KEY = nameof(NAME_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string DESCRIPTION_KEY = nameof(DESCRIPTION_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string INSTRUCTIONS_KEY = nameof(INSTRUCTIONS_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string EDITOR_KEY = nameof(EDITOR_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string TEST_TIME_KEY = nameof(TEST_TIME_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string SAVE_KEY = nameof(SAVE_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string ADD_NEW_TASK_KEY = nameof(ADD_NEW_TASK_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string TEXT_KEY = nameof(TEXT_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string TRUE_FALSE_KEY = nameof(TRUE_FALSE_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string CHECKBOXES_KEY = nameof(CHECKBOXES_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string MULTIPLE_CHOICE_KEY = nameof(MULTIPLE_CHOICE_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string QUESTION_KEY = nameof(QUESTION_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string CORRECT_ANSWER_KEY = nameof(CORRECT_ANSWER_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string CONSIDER_IN_THE_ASSESSMENT_KEY = nameof(CONSIDER_IN_THE_ASSESSMENT_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string YES_KEY = nameof(YES_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string NO_KEY = nameof(NO_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string ASSESSMENT_FOR_CORRECT_ANSWER_KEY = nameof(ASSESSMENT_FOR_CORRECT_ANSWER_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string ARE_YOU_SURE_KEY = nameof(ARE_YOU_SURE_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string CANCEL_KEY = nameof(CANCEL_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string DATA_WILL_NOT_BE_SAVED_KEY = nameof(DATA_WILL_NOT_BE_SAVED_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string ANSWERS_KEY = nameof(ANSWERS_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string EACH_CORRECT_ANS_WORTH_ONE_POINT_IN_FINAL_ASSESSMENT_KEY = nameof(EACH_CORRECT_ANS_WORTH_ONE_POINT_IN_FINAL_ASSESSMENT_KEY);
        /// <summary>
        /// <see cref="Localization"/> key <see cref="string"/>
        /// </summary>
        public const string CORRECT_ANSWERS_KEY = nameof(CORRECT_ANSWERS_KEY);
        
        /* 
            Local data
         */
        /// <summary>
        /// <see cref="LocalData"/> key <see cref="string"/>
        /// </summary>
        public const string PREFERRED_LANGUAGE_LOCAL_DATA = nameof(PREFERRED_LANGUAGE_LOCAL_DATA);
        /// <summary>
        /// <see cref="LocalData"/> key <see cref="string"/>
        /// </summary>
        public const string USER_ID_LOCAL_DATA = nameof(USER_ID_LOCAL_DATA);
        /// <summary>
        /// <see cref="LocalData"/> key <see cref="string"/>
        /// </summary>
        public const string USER_EMAIL_LOCAL_DATA = nameof(USER_EMAIL_LOCAL_DATA);
        /// <summary>
        /// <see cref="LocalData"/> key <see cref="string"/>
        /// </summary>
        public const string USER_RANK_LOCAL_DATA = nameof(USER_RANK_LOCAL_DATA);
        /// <summary>
        /// <see cref="LocalData"/> key <see cref="string"/>
        /// </summary>
        public const string USER_NAME_LOCAL_DATA = nameof(USER_NAME_LOCAL_DATA);
    }
}