using PostgresClient.Model;
using PsqlSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace PostgresClient.ViewModel
{
    abstract class BaseViewModel : INotifyPropertyChanged
    {
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
   
        protected virtual BaseModel Model { get=>model;  }
        private BaseModel model;

        public BaseViewModel(ISqlApi api)
        {
            model =CreateModel(api);
            model.NewConnectStatus += NewConnectStatus;
        }
        protected abstract BaseModel CreateModel(ISqlApi api);

        private void NewConnectStatus(object? sender, bool e)
        {
            IsConnected = e;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
