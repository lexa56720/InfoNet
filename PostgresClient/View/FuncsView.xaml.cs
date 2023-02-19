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
    /// Логика взаимодействия для FuncsView.xaml
    /// </summary>
    public partial class FuncsView : Page
    {
        FuncsViewModel ViewModel { get; set; }

        Action? Action = new Action(() => { });
        public FuncsView()
        {
            InitializeComponent();
            ViewModel = new FuncsViewModel(ref Action, App.Api);
            DataContext = ViewModel;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Action.Invoke();
        }
    }
}
