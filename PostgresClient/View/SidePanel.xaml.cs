using PostgresClient.ViewModel;
using System.Windows.Controls;

namespace PostgresClient.View
{
    /// <summary>
    /// Логика взаимодействия для SidePanel.xaml
    /// </summary>
    public partial class SidePanel : Page
    {
        public SidePanel()
        {
            InitializeComponent();
            DataContext = new SidePanelViewModel();
        }
    }
}
