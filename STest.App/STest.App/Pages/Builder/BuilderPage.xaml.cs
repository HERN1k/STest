using System;
using System.Collections.ObjectModel;
using System.Runtime.Versioning;

using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using STest.App.Domain.Interfaces;
using STest.App.Pages.Home;
using STest.App.Utilities;

using STLib.Core.Testing;

namespace STest.App.Pages.Builder
{
    /// <summary>
    /// Builder page
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class BuilderPage : Page
    {
        public ObservableCollection<Test> TestsList { get; set; }

        /// <summary>
        /// <see cref="ILocalization"/> instance
        /// </summary>
        private readonly ILocalization m_localization;
        /// <summary>
        /// <see cref="ILogger"/> instance
        /// </summary>
        private readonly ILogger<HomePage> m_logger;

        public BuilderPage()
        {
            this.InitializeComponent();
            m_localization = ServiceHelper.GetService<ILocalization>();
            m_logger = ServiceHelper.GetLogger<HomePage>();

            var test = Test.Build(Guid.NewGuid())
                .AddName("Test")
                .AddDescription("Description")
                .AddInstructions("Instructions")
                .AddTestTime(TimeSpan.FromMinutes(30));

            TestsList = new ObservableCollection<Test>();

            for (int i = 0; i < 20; i++)
            {
                TestsList.Add(test);
            }

            this.DataContext = this;
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
    }
}