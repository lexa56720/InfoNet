using PostgresClient.Model;
using PostgresClient.Utils;
using PsqlSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PostgresClient.ViewModel
{
    internal class ConnectViewModel : INotifyPropertyChanged
    {
      
        public ICommand ConnectClick { get => new Command(async () => { await Connect(); }); }

        public bool IsConnected
        { 
            get => isConnected; 
            set 
            {
                isConnected = value;
                OnPropertyChanged(nameof(IsConnected));
            }
        }
        private bool isConnected;

        public string Username { get; set; } = "postgres";

        public string Password { get; set; } = "1234";

        public string Server { get; set; } = "localhost";

        public string Port { get; set; } = "5432";

        public string DataBase { get; set; } = "carsdb";

        public event PropertyChangedEventHandler? PropertyChanged;
        private ConnectModel Model { get; }
        public ConnectViewModel(ISqlApi api)
        {
            Model = new ConnectModel(api);
            Model.NewConnectStatus += NewConnectStatus;
        }

        private void NewConnectStatus(object? sender, bool e)
        {
            IsConnected=e;
        }

        public async Task Connect()
        {
           await Model.Connect(DataBase, Username, Password, Server, Port);       
        }
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));          
        }

    }
}
