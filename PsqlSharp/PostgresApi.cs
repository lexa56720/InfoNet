using Microsoft.Extensions.Logging;
using Npgsql;
using Npgsql.Internal;
using Npgsql.Schema;
using NpgsqlTypes;
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

                using var adapter = new NpgsqlDataAdapter(commandObj);

                var dataSet = new DataSet();
                adapter.Fill(dataSet);
                if (dataSet.Tables.Count > 0)
                    return new Table(dataSet.Tables[0]);
            }

            return null;
        }

        public async Task<string[]?> GetAllTables()
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
            var table = await ExecuteCommand($"select * from {tableName}");
            if (table != null)
            {
                table.TableName = tableName;
                return table;
            }
            return null;
        }

        public async Task<Function[]?> GetAllFunctions()
        {
            var funcTable = await ExecuteCommand(
                @"select n.nspname as function_schema,
                p.proname as function_name,
                l.lanname as function_language,
                case when l.lanname = 'internal' then p.prosrc
                    else pg_get_functiondef(p.oid)
                    end as definition,
                pg_get_function_arguments(p.oid) as function_arguments,
                t.typname as return_type
        from pg_proc p
        left
        join pg_namespace n on p.pronamespace = n.oid
        left
        join pg_language l on p.prolang = l.oid
        left
        join pg_type t on t.oid = p.prorettype
        where n.nspname not in ('pg_catalog', 'information_schema')
        order by function_schema,
                    function_name; ");
            if (funcTable != null)
                return Function.Parse(funcTable);
            return null;
        }


        public Task<Table?> ExecuteFunction(string func, params string[] parameters)
        {
            throw new NotImplementedException();
        }
        public Task<bool> AddFunction(string funcCode)
        {
            throw new NotImplementedException();
        }
        public Task<bool> RemoveFunction(string funcName)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SetColumnByRow(string tableName, string columnName, string cellValue, int rowCount)
        {
            try
            {
                var rowNumbers = await ExecuteCommand($"select ctid, * from {tableName};");
                var ctid = ((NpgsqlTid)rowNumbers.DataTable.Rows[rowCount][0]).ToString();
                await ExecuteCommand(
                    @$"UPDATE {tableName}
                    SET {columnName} = '{cellValue}'
                    WHERE ctid='{ctid}';");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> AddRow(Table table, string[] values)
        {
            try
            {
                await ExecuteCommand(
            @$"INSERT INTO {table.TableName}({string.Join(", ", table.ColumnNames)})
            VALUES (${string.Join("", "", table.ColumnNames)})");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
