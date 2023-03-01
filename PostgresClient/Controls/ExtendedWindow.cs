using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PostgresClient.Controls
{
    internal class ExtendedWindow : Window
    {
        public Page A { get; set; }
        public ExtendedWindow():base() 
        {
            var f = new Frame();
            this.AddChild(f);
            f.Content = A;
        }

    }
}
