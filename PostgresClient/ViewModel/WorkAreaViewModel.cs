using PostgresClient.Model;
using PsqlSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgresClient.ViewModel
{
    class WorkAreaViewModel:INotifyPropertyChanged
    {
        public WorkAreaModel Model { get; }

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
        public WorkAreaViewModel(ISqlApi api)
        {
            Model=new WorkAreaModel(api);
            Model.NewConnectStatus += NewConnectStatus;
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
