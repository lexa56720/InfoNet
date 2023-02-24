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
        public ConnectionData? ConnectionData { get; }
        public bool IsConnected { get; }

        public event EventHandler ConnectionStatusChanged;


        public Task<bool> ConnectAsync(ConnectionData connectionData);
        public Task<bool> DisconnectAsync();

        public Task<Table?> ExecuteCommand(string command);
        public Task<Table?> ExecuteFunction(string func, params string[] parameters );


        public Task<string[]?> GetAllDataBaseNames();
        public Task<DataBase?> GetDataBaseContent(string dbName);

        public Task<Table?> GetTableContent(string tableName);
        public Task<Table?> GetTableContent(string tableName,string dbName);


        public Task<string[]?> GetAllTableNames();
        public Task<string[]?> GetAllTableNames(string dbName);

        public Task<Function[]?> GetAllFunctions();
        public Task<Function[]?> GetAllFunctions(string dbName);

        public Task<bool> AddFunction(string funcCode);
        public Task<bool> RemoveFunction(string funcHeader);

        public Task<bool> SetColumnByRow(string tableName, string columnName,string cellValue,int rowCount);
        public Task<bool> AddRow(Table table, string[] values);

    }
}
