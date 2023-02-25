using PostgresClient.ViewModel;
using System.Windows.Controls;

namespace PostgresClient.View
{
    /// <summary>
    /// Логика взаимодействия для DumpManager.xaml
    /// </summary>
    public partial class DumpManager : Page
    {
        public DumpManager()
        {
            InitializeComponent();
            DataContext = new DumpManagerViewModel(App.Api);
        }
    }
}
