using Npgsql;
using Npgsql.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsqlSharp
{
    public class PostgresApi : ISqlApi
    {
        public string UserName => throw new NotImplementedException();

        public NpgsqlDatabaseInfo DatabaseInfo => throw new NotImplementedException();

        public bool IsConnected => throw new NotImplementedException();



        public Task<bool> ConnectAsync(string database, string username, string password)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DisconnectAsync()
        {
            throw new NotImplementedException();
        }

        public Task<NpgsqlDataReader?> ExecuteCommand(string command)
        {
            throw new NotImplementedException();
        }
    }
}
