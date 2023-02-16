using Microsoft.Extensions.Logging;
using Npgsql;
using Npgsql.Internal;
using Npgsql.Schema;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PsqlSharp
{
    public class PostgresApi : ISqlApi
    {


        public string UserName => throw new NotImplementedException();

        public NpgsqlDatabaseInfo DatabaseInfo => throw new NotImplementedException();

        public event EventHandler? ConnectionStatusChanged;
        public bool IsConnected
        {
            get => isConnected;
            set
            {
                isConnected = value;
                ConnectionStatusChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        private bool isConnected;
        private NpgsqlConnection? Connection { get; set; }

        public async Task<bool> ConnectAsync(string database, string username, string password, string server, string port = "5432")
        {
            if (!IsConnected)
                try
                {
                    var connectionString = $"Host={server};Username={username};Password={password};Database={database.ToLower()};Port={port}";
                    var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);

                    dataSourceBuilder.UseLoggerFactory(LoggerFactory.Create(builder =>
                    {
                        builder
                        .AddDebug()
                        .SetMinimumLevel(LogLevel.Trace);
                    }));
                    await using var dataSource = dataSourceBuilder.Build();
                    Connection = await dataSource.OpenConnectionAsync();
                    IsConnected = true;
                }
                catch (Exception e)
                {
                    IsConnected = false;
                }

            return IsConnected;
        }

        public async Task<bool> DisconnectAsync()
        {
            if (IsConnected)
            {
                await Connection.CloseAsync();
                IsConnected = false;
                return true;
            }


            return false;
        }

        public async Task<Table?> ExecuteCommand(string command)
        {
            if (IsConnected)
            {
                await using var commandObj = new NpgsqlCommand(command, Connection);
                await using var reader = await commandObj.ExecuteReaderAsync();

                var columnsInfo = await reader.GetColumnSchemaAsync();

                if (columnsInfo.Count != 0)
                    return await CopyCommandResult(reader, columnsInfo.ToArray());
            }
            return null;
        }

        private async Task<Table> CopyCommandResult(NpgsqlDataReader reader, NpgsqlDbColumn[] columns)
        {
            var resultList = new List<string[]>();
            var column = new string[columns.Length];
            while (await reader.ReadAsync())
            {
                for (int i = 0; i < columns.Length; i++)
                {
                    var a = reader.GetValue(i);
                    if (a == null)
                        column[i] = string.Empty;
                    else
                        column[i] = a.ToString();
                }
                resultList.Add(column.ToArray());
            }

            var arr = new string[resultList.Count, resultList[0].Length];
            for (int i = 0; i < resultList.Count; i++)
                for (int j = 0; j < resultList[i].Length; j++)
                    arr[i, j] = resultList[i][j];


            return new Table(columns,arr);
        }

        public Task<NpgsqlDataReader?> ExecuteFunction(string func, params string[] parameters)
        {
            throw new NotImplementedException();
        }

        public async Task<string[]?> GetTables()
        {
            if (IsConnected)
            {

                var tables = await ExecuteCommand(@"select * from pg_tables;");

                var result = new List<string>();
                for (int i = 0; i < tables.ColumnCount; i++)
                    if (tables[i, 0] == "public")
                        result.Add(tables[i, 1]);

                return result.ToArray();
            }

            return null;
        }

        public async Task<Table?> GetTableContent(string tableName)
        {
            return await ExecuteCommand($"select * from {tableName}"); ;
        }

        public async Task<TableInfo?> GetTableInfo(string tableName)
        {
            if (IsConnected)
            {

                var table = await ExecuteCommand(
                "select table_name, count(*) as column_count" +
                "from information_schema.\"columns\"" +
                $"where table_schema = 'public' && tablename={tableName}" +
                "GROUP by table_name order by column_count desc;");

                // await table.ReadAsync();
                // int collumns = int.Parse(table.GetString(1));

                table = await ExecuteCommand($"select  count(*) from{tableName}");
                // await table.ReadAsync();
                //int rows = int.Parse(table.GetString(0));

                return new TableInfo(tableName, 0, 1);
            }

            return null;
        }
    }

    public class TableInfo
    {
        public string Name { get; }

        public int Rows { get; }

        public int Collumns { get; }

        public TableInfo(string name, int rows, int collumns)
        {
            Name = name;
            Rows = rows;
            Collumns = collumns;
        }
    }
}
