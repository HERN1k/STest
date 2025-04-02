using System.Runtime.Versioning;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;

namespace STest.App.Controls
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class ConfirmationButton : Button
    {
        public ConfirmationButton()
        {
            this.DefaultStyleKey = typeof(ConfirmationButton);
        }

        private void ConfirmationButton_Click(object sender, RoutedEventArgs args)
        {
            if (GetTemplateChild("ConfirmationSuccessAnimation") is Storyboard storyBoard)
            {
                storyBoard.Begin();
                AnnounceActionForAccessibility(this, "Confirmation", "ConfirmationActivityId");
            }
        }

        protected override void OnApplyTemplate()
        {
            Click -= ConfirmationButton_Click;
            base.OnApplyTemplate();
            Click += ConfirmationButton_Click;
        }

        public static void AnnounceActionForAccessibility(UIElement ue, string annoucement, string activityID)
        {
            var peer = FrameworkElementAutomationPeer.FromElement(ue);
            peer.RaiseNotificationEvent(AutomationNotificationKind.ActionCompleted,
                                        AutomationNotificationProcessing.ImportantMostRecent, annoucement, activityID);
        }
    }
}
