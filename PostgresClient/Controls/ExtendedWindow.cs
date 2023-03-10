using System.Windows;
using System.Windows.Controls;

namespace PostgresClient.Controls
{
    internal class ExtendedWindow : Window
    {
        public Page A { get; set; }
        public ExtendedWindow() : base()
        {
            var f = new Frame();
            this.AddChild(f);
            f.Content = A;
        }

    }
}
