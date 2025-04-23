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
        private readonly ILocalization m_localization;
        private readonly ILocalData m_localData;
        private readonly ILogger<AccountPage> m_logger;

        public AccountPage()
        {
            this.InitializeComponent();
            m_localization = ServiceHelper.GetService<ILocalization>();
            m_localData = ServiceHelper.GetService<ILocalData>();
            m_logger = ServiceHelper.GetLogger<AccountPage>();
            this.DataContext = this;
        }

        /// <inheritdoc />
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                base.OnNavigatedTo(e);

                PersonPicture.DisplayName = m_localData.GetString(Constants.USER_NAME_LOCAL_DATA);
                PersonName.Text = PersonPicture.DisplayName;
                PersonRank.Text = T(m_localData.GetString(Constants.USER_RANK_LOCAL_DATA).ToStringLocalizationKey());
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }

        /// <inheritdoc />
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            try
            {
                base.OnNavigatingFrom(e);
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        
        /// <summary>
        /// Get the localized string by key
        /// </summary>
        /// <param name="key"></param>
        private string T(string key) => m_localization.T(key);
    }
}