using PostgresClient.ViewModel;
using System.Windows.Controls;

namespace PostgresClient.View
{
    /// <summary>
    /// Логика взаимодействия для WorkArea.xaml
    /// </summary>
    public partial class WorkArea : Page
    {
        public WorkArea()
        {

            InitializeComponent();
            DataContext = new WorkAreaViewModel(App.Api);
        }
    }
}
