using PostgresClient.ViewModel;
using System.Windows.Controls;

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
    }
}
