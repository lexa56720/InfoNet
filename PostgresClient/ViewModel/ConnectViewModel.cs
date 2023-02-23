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
    internal class ConnectViewModel : BaseViewModel
    {

        public ICommand ConnectClick
        {
            get
            {
                return new Command(async () =>
                {
                    if (IsConnected)
                        await Disconnect();
                    else
                        await Connect();
                });
            }
        }

        public string ButtonAction
        {
            get => buttonAction;
            set
            {
                buttonAction = value;
                OnPropertyChanged(nameof(ButtonAction));
            }
        }
        private string buttonAction = "Подключиться";

        public override bool IsConnected
        {
            get => base.IsConnected;
            set
            {
                base.IsConnected = value;
                if (IsConnected)
                    ButtonAction = "Отключиться";
                else
                    ButtonAction = "Подключиться";
            }
        }


        public string Username { get; set; } = "postgres";

        public string Password { get; set; } = "1234";

        public string Server { get; set; } = "localhost";

        public string Port { get; set; } = "5432";

        public string DataBase { get; set; } = "carsdb";

        protected override ConnectModel Model => (ConnectModel)base.Model;

        public ConnectViewModel(ISqlApi api) : base(api)
        {

        }
        protected override BaseModel CreateModel(ISqlApi api)
        {
            return new ConnectModel(api);
        }

        public async Task Connect()
        {
            await Model.Connect(DataBase, Username, Password, Server, Port);
        }
        private async Task Disconnect()
        {
            await Model.Disconnect();
        }

    }
}
