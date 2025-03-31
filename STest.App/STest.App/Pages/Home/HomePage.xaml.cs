using System;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using STest.App.Domain.Interfaces;
using STest.App.Utilities;

namespace STest.App.Pages.Home
{
    /// <summary>
    /// Home page
    /// </summary>
    public sealed partial class HomePage : Page
    {
        /// <summary>
        /// <see cref="ILocalization"/> instance
        /// </summary>
        private readonly ILocalization m_localization;
        /// <summary>
        /// <see cref="ILocalData"/> instance
        /// </summary>
        private readonly ILocalData m_localData;

        /// <summary>
        /// Constructor
        /// </summary>
        public HomePage()
        {
            this.InitializeComponent();
            m_localization = ServiceHelper.GetService<ILocalization>();
            m_localData = ServiceHelper.GetService<ILocalData>();
            SubscribeToEvents();

            TestTextBlock.Text = m_localData.GetString(Constants.EMAIL_LOCAL_DATA);
        }

        #region OnNavigated
        /// <summary>
        /// OnNavigatedTo
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// OnNavigatingFrom
        /// </summary>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
        }
        #endregion

        /// <summary>
        /// Subscribe to events
        /// </summary>
        private void SubscribeToEvents()
        {

        }

        #region Exceptions
        /// <summary>
        /// Show alert
        /// </summary>
        private void EnsureAddedTaskToUIThread(bool isEnqueued)
        {
            if (!isEnqueued)
            {
                this.ShowAlert(
                    title: m_localization.GetString(Constants.ERROR_KEY),
                    message: m_localization.GetString(Constants.FAILED_ADD_TASK_TO_UI_THREAD_KEY),
                    InfoBarSeverity.Error);
            }
        }

        /// <summary>
        /// Show exception
        /// </summary>
        private void ShowException(Exception ex)
        {
#if DEBUG
            this.ShowAlertExceptionWithTrace(ex, m_localization);
#else
            this.ShowAlertException(ex, m_localization);
#endif
        }
        #endregion
    }
}