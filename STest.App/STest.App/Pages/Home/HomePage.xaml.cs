using System;
using STest.App.Domain.Interfaces;
using STest.App.Utilities;
using System.Collections.Generic;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Controls;
using STLib.Tasks.Text;
using STLib.Tasks.TrueFalse;
using STLib.Tasks.MultipleChoice;
using STLib.Tasks.Checkboxes;
using STLib.Core.Testing;

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
        /// <see cref="ILogger"/> instance
        /// </summary>
        private readonly ILogger<HomePage> m_logger;

        /// <summary>
        /// Constructor
        /// </summary>
        public HomePage()
        {
            this.InitializeComponent();
            m_localization = ServiceHelper.GetService<ILocalization>();
            m_logger = ServiceHelper.GetLogger<HomePage>();
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