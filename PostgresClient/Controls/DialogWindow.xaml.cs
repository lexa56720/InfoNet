using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PostgresClient.Controls
{
    /// <summary>
    /// Логика взаимодействия для GetFunctionInfo.xaml
    /// </summary>
    public partial class DialogWindow : Window, INotifyPropertyChanged
    {
        private bool IsCanceled { get; set; }


        public event PropertyChangedEventHandler? PropertyChanged;

        private void PropertyChangedInvoke(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public DialogWindow(string question)
        {
            InitializeComponent();
            QuestinBlock.Text = question;
        }

        public async Task<string?> GetResponse()
        {
            ShowDialog();
            if (!IsCanceled)
                return Input.Text;
            return null;
        }



        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            IsCanceled = true;
            Close();
        }
    }
}
