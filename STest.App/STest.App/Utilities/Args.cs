using System;

namespace STest.App.Utilities
{
    public sealed class ShowDialogArgs
    {
        public string? Title { get; set; }
        public string? Message { get; set; }
        public string? OkButtonText { get; set; }
        public string? CancelButtonText { get; set; }

        public ShowDialogArgs(string? title = null, string? message = null, string? okButtonText = null, string? cancelButtonText = null)
        {
            Title = title;
            Message = message;
            OkButtonText = okButtonText;
            CancelButtonText = cancelButtonText;
        }
    }

    public sealed class BuilderListItem
    {
        public Guid ID { get; set; }
        public string Text { get; set; }
        public bool IsChecked { get; set; } = false;

        public BuilderListItem(string text, Guid id)
        {
            Text = text;
            ID = id;
        }
    }
}