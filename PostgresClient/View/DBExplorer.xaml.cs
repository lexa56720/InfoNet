using PostgresClient.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PostgresClient.View
{
    /// <summary>
    /// Логика взаимодействия для DBExplorer.xaml
    /// </summary>
    public partial class DBExplorer : Page
    {
        public DBExplorer()
        {
            InitializeComponent();
            DataContext = new DBExplorerViewModel(App.Api);
        }

        private void TreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
