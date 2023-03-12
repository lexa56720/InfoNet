using PostgresClient.Model;
using System.ComponentModel;

namespace PostgresClient.ViewModel
{
    internal abstract class BaseViewModel : INotifyPropertyChanged
    {
        public virtual bool IsConnected
        {
            get => isConnected;
            set
            {
                isConnected = value;
                OnPropertyChanged(nameof(IsConnected));
            }
        }
        private bool isConnected;

        protected virtual BaseModel Model => model;
        private BaseModel model;

        public BaseViewModel(ISqlApi api)
        {
            model = CreateModel(api);
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
