using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace PostgresClient.Controls
{
    public sealed class ExtendedRichTextBox : RichTextBox
    {

        private bool preventDocumentUpdate;
        private bool preventTextUpdate;


        public bool AlwaysScrollToEnd { get; set; }

        public int PageWidth
        {
            get => pageWidth;
            set
            {
                pageWidth = value;
                Document.PageWidth = value;
            }
        }
        private int pageWidth;
        public ExtendedRichTextBox()
        {
  
        }

        public ExtendedRichTextBox(FlowDocument document)
          : base(document)
        {
        }


        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                nameof(Text),
                typeof(string),
                typeof(RichTextBox),
                new FrameworkPropertyMetadata(
                    String.Empty,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnTextPropertyChanged,
                    CoerceTextProperty,
                    true,
                    UpdateSourceTrigger.LostFocus));
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        private static object CoerceTextProperty(DependencyObject d, object value)
        {
            return value ?? string.Empty;
        }


        protected override void OnTextChanged(System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateTextFromDocument();
            base.OnTextChanged(e);
            if (AlwaysScrollToEnd)
                ScrollToEnd();
        }
        private void UpdateTextFromDocument()
        {
            if (preventTextUpdate)
                return;

            preventDocumentUpdate = true;
            this.SetCurrentValue(ExtendedRichTextBox.TextProperty, new TextRange(Document.ContentStart, Document.ContentEnd).Text);
            preventDocumentUpdate = false;
        }
        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ExtendedRichTextBox)d).UpdateDocumentFromText();
        }
        private void UpdateDocumentFromText()
        {
            if (preventDocumentUpdate)
                return;

            preventTextUpdate = true;
            Document.Blocks.Clear();
            Document.Blocks.Add(new Paragraph(new Run(Text)));
            preventTextUpdate = false;
        }


        public void Clear()
        {
            Document.Blocks.Clear();
        }

    }

}
