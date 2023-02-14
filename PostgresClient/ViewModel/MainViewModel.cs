using PostgresClient.Model;
using PostgresClient.View;
using PsqlSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PostgresClient.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler? PropertyChanged;

        private MainModel Model { get; set; }

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


        public Page SidePanel
        {
            get => sidePanel;
            set
            {
                sidePanel = value;
                OnPropertyChanged(nameof(SidePanel));
            }
        }
        private Page sidePanel;


        public MainViewModel(ISqlApi api)
        {
            Model = new MainModel(api);
            Model.NewConnectStatus += NewConnectStatus;
            SidePanel = new ConnectView();
        }

        private void NewConnectStatus(object? sender, bool e)
        {
            IsConnected = e;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
