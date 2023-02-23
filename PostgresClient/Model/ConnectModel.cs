using PsqlSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgresClient.Model
{
    internal class ConnectModel:BaseModel
    {
        private ISqlApi Api { get; }

        public event EventHandler<bool>? NewConnectStatus;

        public ConnectModel(ISqlApi api):base(api)
        {
            Api = api; 
            api.ConnectionStatusChanged += (o, e) => NewConnectStatus?.Invoke(o, api.IsConnected);
        }
        public async Task Disconnect()
        {
            await Api.DisconnectAsync();
        }
        public async Task<bool> Connect(string database,string username,string password,string server,string port)
        {
           return await Api.ConnectAsync(database, username, password, server, port);
        }
    }
}
