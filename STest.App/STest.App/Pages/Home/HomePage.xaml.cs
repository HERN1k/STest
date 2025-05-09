using System;
using STest.App.Domain.Interfaces;
using STest.App.Utilities;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Controls;

namespace STest.App.Pages.Home
{
    /// <summary>
    /// Home page
    /// </summary>
    public sealed partial class HomePage : Page
    {
        private readonly ILocalization m_localization;
        private readonly ILogger<HomePage> m_logger;

        public HomePage()
        {
            this.InitializeComponent();
            m_localization = ServiceHelper.GetService<ILocalization>();
            m_logger = ServiceHelper.GetLogger<HomePage>();
            this.DataContext = this;
        }

        /// <inheritdoc />
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                base.OnNavigatedTo(e);
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
    }
}