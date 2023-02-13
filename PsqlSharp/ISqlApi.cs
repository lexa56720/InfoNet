using System;
using Npgsql;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql.Internal;

namespace PsqlSharp
{
    public interface ISqlApi
    {
        public string UserName { get; }

        public NpgsqlDatabaseInfo DatabaseInfo { get; }

        public bool IsConnected { get; }

        public Task<bool> ConnectAsync(string database, string username, string password);

        public Task<bool> DisconnectAsync();

        public Task<NpgsqlDataReader?> ExecuteCommand(string command);

        public Task<NpgsqlDataReader?> ExecuteFunction(string func, params string[] parameters );

    }
}
