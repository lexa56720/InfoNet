using PostgresClient.ViewModel;
using System.Windows.Controls;

namespace PostgresClient.View
{
    /// <summary>
    /// Логика взаимодействия для Debugger.xaml
    /// </summary>
    public partial class Debugger : Page
    {
        public Debugger()
        {
            InitializeComponent();
            DataContext = new DebuggerViewModel(App.Api);
        }
    }
}
