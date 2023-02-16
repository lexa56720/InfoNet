using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows;

namespace PostgresClient.Controls
{
    public sealed class ExtendedRichTextBox : RichTextBox
    {
        public static readonly DependencyProperty SourceProperty =
                DependencyProperty.Register(nameof(Source),
                typeof(Uri), typeof(ExtendedRichTextBox),
                new PropertyMetadata(OnSourceChanged));

        public Uri Source
        {
            get => GetValue(SourceProperty) as Uri;
            set => SetValue(SourceProperty, value);
        }
        private static void OnSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is ExtendedRichTextBox rtf && rtf.Source != null)
            {
                var stream = Application.GetResourceStream(rtf.Source);
                if (stream != null)
                {
                    var range = new TextRange(rtf.Document.ContentStart, rtf.Document.ContentEnd);
                    range.Load(stream.Stream, DataFormats.Rtf);
                }
            }
        }



        public static readonly DependencyProperty ContentProperty =
              DependencyProperty.Register(nameof(Content),
              typeof(string), typeof(ExtendedRichTextBox),
              new PropertyMetadata(string.Empty, ContentPropertyChanged));
        public string Content
        {
            get => (string)new TextRange(Document.ContentStart, Document.ContentEnd).Text;
            set
            {
                SetValue(ContentProperty, value);
            }
        }
        private void ContentPropertyChanged(string text)
        {
            Document.Blocks.Clear();
            Document.Blocks.Add(new Paragraph(new Run(text)));
        }

        private static void ContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ExtendedRichTextBox)d).ContentPropertyChanged((string)e.NewValue);
        }


    }
}
