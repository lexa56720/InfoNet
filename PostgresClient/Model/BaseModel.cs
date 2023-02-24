using PsqlSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgresClient.Model
{
    abstract class BaseModel
    {
        protected ISqlApi Api { get; }

        public event EventHandler<bool>? NewConnectStatus;
        public BaseModel(ISqlApi api)
        {
            Api = api;
            api.ConnectionStatusChanged += ConnectionStatusChanged;
        }

        private void ConnectionStatusChanged(object? sender, EventArgs e)
        {
            NewConnectStatus?.Invoke(this, Api.IsConnected);
        }
        public bool DataBaseNameIs(string dbName)
        {
            if(Api.ConnectionData==null)
                return false;
            return Api.ConnectionData.Database == dbName;
        }
    }
}
