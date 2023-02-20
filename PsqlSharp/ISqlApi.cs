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

        public event EventHandler ConnectionStatusChanged;

        public Task<bool> ConnectAsync(string database, string username, string password,string server,string port = "5432");

        public Task<bool> DisconnectAsync();

        public Task<Table?> ExecuteCommand(string command);

        public Task<Table?> ExecuteFunction(string func, params string[] parameters );

        public Task<Table?> GetTableContent(string tableName);

        public Task<string[]?> GetAllTables();

        public Task<Function[]?> GetAllFunctions();

        public Task<bool> RemoveFunction(string funcName);


    }
}
