using System;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

using STest.App.Domain.Enums;
using STest.App.Domain.Interfaces;
using STest.App.Utilities;

namespace STest.App.Pages.Account
{
    /// <summary>
    /// Account page
    /// </summary>
    public sealed partial class AccountPage : Page
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
        /// <see cref="ILogger"/> instance
        /// </summary>
        private readonly ILogger<AccountPage> m_logger;

        /// <summary>
        /// Constructor
        /// </summary>
        public AccountPage()
        {
            this.InitializeComponent();
            m_localization = ServiceHelper.GetService<ILocalization>();
            m_localData = ServiceHelper.GetService<ILocalData>();
            m_logger = ServiceHelper.GetLogger<AccountPage>();
            DataContext = this;
        }

        #region OnNavigated
        /// <summary>
        /// OnNavigatedTo
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                base.OnNavigatedTo(e);

                SubscribeToEvents();
                TitleText.Text = T(Constants.PROFILE_KEY);
                PersonPicture.DisplayName = m_localData.GetString(Constants.USER_NAME_LOCAL_DATA);
                PersonName.Text = PersonPicture.DisplayName;
                PersonRank.Text = T(m_localData.GetString(Constants.USER_RANK_LOCAL_DATA).ToStringLocalizationKey());
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }

        /// <summary>
        /// OnNavigatingFrom
        /// </summary>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            try
            {
                base.OnNavigatingFrom(e);

                UnSubscribeToEvents();
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        #endregion

        /// <summary>
        /// Subscribe to events
        /// </summary>
        private void SubscribeToEvents()
        {

        }

        /// <summary>
        /// Un subscribe to events
        /// </summary>
        private void UnSubscribeToEvents()
        {

        }

        /// <summary>
        /// Get the localized string by key
        /// </summary>
        /// <param name="key"></param>
        private string T(string key) => m_localization.T(key);
    }
}