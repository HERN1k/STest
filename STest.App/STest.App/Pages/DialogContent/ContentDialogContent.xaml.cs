using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace STest.App.Pages.DialogContent
{
    public sealed partial class ContentDialogContent : Page
    {
        public string Text { get; set; } = string.Empty;
        public Visibility PageVisibility { get; set; } = Visibility.Collapsed;

        public ContentDialogContent(string? text)
        {
            this.InitializeComponent();

            if (!string.IsNullOrEmpty(text))
            {
                Text = text;
                PageVisibility = Visibility.Visible;
            }
        }
    }
}
