using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Data;
using System.Buffers.Text;
using System.Windows.Media.TextFormatting;

namespace PostgresClient.Controls
{
    public sealed class ExtendedRichTextBox : RichTextBox
    {

        private bool _preventDocumentUpdate;
        private bool _preventTextUpdate;


        public ExtendedRichTextBox()
        {
        }

        public ExtendedRichTextBox(FlowDocument document)
          : base(document)
        {
        }


        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
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
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        private static object CoerceTextProperty(DependencyObject d, object value)
        {
            return value ?? "";
        }


        protected override void OnTextChanged(System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateTextFromDocument();
            base.OnTextChanged(e);
        }
        private void UpdateTextFromDocument()
        {
            if (_preventTextUpdate)
                return;

            _preventDocumentUpdate = true;
            this.SetCurrentValue(ExtendedRichTextBox.TextProperty, new TextRange(Document.ContentStart, Document.ContentEnd).Text);
            _preventDocumentUpdate = false;
        }
        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ExtendedRichTextBox)d).UpdateDocumentFromText();
        }
        private void UpdateDocumentFromText()
        {
            if (_preventDocumentUpdate)
                return;

            _preventTextUpdate = true;
            Document.Blocks.Clear();
            Document.Blocks.Add(new Paragraph(new Run(Text)));
            _preventTextUpdate = false;
        }


        public void Clear()
        {
            this.Document.Blocks.Clear();
        }

    }

}
