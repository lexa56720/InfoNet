using PsqlSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgresClient.Model
{
    internal class ConnectModel
    {
        private ISqlApi Api { get; }
        public ConnectModel(ISqlApi api)
        {
            Api = api;
        }

        public async Task Connect(string database,string username,string password,string server,string port)
        {
            await Api.ConnectAsync(database, username, password, server, port);
        }
    }
}
