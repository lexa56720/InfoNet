using PostgresClient.Model;
using PostgresClient.Utils;
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
using System.Windows.Input;

namespace PostgresClient.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        public ICommand DisconnectClick{ get => new Command(async () => { await Disconnect(); }); }
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

        public event PropertyChangedEventHandler? PropertyChanged;

        private MainModel Model { get; set; }

        public Page TableViewerFrame
        {
            get => tableViewerFrame;
            set
            {
                tableViewerFrame = value;
                OnPropertyChanged(nameof(SidePanel));
            }
        }
        private Page tableViewerFrame;

        public Page WorkFrame
        {
            get => workFrame;
            set
            {
                workFrame = value;
                OnPropertyChanged(nameof(SidePanel));
            }
        }
        private Page workFrame;

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
            WorkFrame =new  WorkArea();
            TableViewerFrame = new TableViewer();
        }

        private async Task Disconnect()
        {
            await Model.Disconnect();
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
