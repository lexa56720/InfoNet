using PsqlSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgresClient.Model
{
    class WorkAreaModel
    {
        public ISqlApi Api { get; }
        public event EventHandler<bool>? NewConnectStatus;

        public WorkAreaModel(ISqlApi api)
        {
            Api = api; 
            api.ConnectionStatusChanged += (o, e) => NewConnectStatus?.Invoke(o, api.IsConnected);
        }

    }
}
