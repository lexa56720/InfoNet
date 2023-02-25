using PostgresClient.ViewModel;
using System.Windows.Controls;

namespace PostgresClient.View
{
    /// <summary>
    /// Логика взаимодействия для FuncsView.xaml
    /// </summary>
    public partial class FuncsView : Page
    {
        private FuncsViewModel ViewModel { get; set; }

        public FuncsView()
        {
            InitializeComponent();
            ViewModel = new FuncsViewModel(App.Api);
            DataContext = ViewModel;
        }
    }
}
