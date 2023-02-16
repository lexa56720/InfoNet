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
    class WorkAreaViewModel: BaseViewModel
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


        public WorkAreaViewModel(ISqlApi api)
        {
            Model=new WorkAreaModel(api);
            Model.NewConnectStatus += NewConnectStatus;
        }
        private void NewConnectStatus(object? sender, bool e)
        {
            IsConnected = e;
        }

    }
}
