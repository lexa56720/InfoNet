using PostgresClient.ViewModel;
using System.Windows.Controls;

namespace PostgresClient.View
{
    /// <summary>
    /// Логика взаимодействия для ConnectView.xaml
    /// </summary>
    public partial class ConnectView : Page
    {
        public ConnectView()
        {
            InitializeComponent();
            DataContext = new ConnectViewModel(App.Api);
        }
    }


}
