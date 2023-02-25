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
        public ConnectionData? ConnectionData { get; private set; }
        private NpgsqlConnection? Connection { get; set; }

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

        private static SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);

        ~PostgresApi()
        {
            Connection?.Close();
        }

        public async Task<bool> ConnectAsync(ConnectionData connectionData)
        {
            if (!IsConnected)
                try
                {
                    var connectionString = connectionData.ToString();
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
                    ConnectionData = connectionData;
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
            await Semaphore.WaitAsync();
            try
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
            }
            finally
            {
                Semaphore.Release();
            }
            return null;
        }

        public async Task<string[]?> GetAllDataBaseNames()
        {
            if (IsConnected)
            {
                var databases = await ExecuteCommand("SELECT datname FROM pg_database where datistemplate='f';");
                var result = new string[databases.RowCount];
                for (int i = 0; i < databases.RowCount; i++)
                    result[i] = databases[i, 0];
                return result;
            }
            return null;
        }
        public async Task<DataBase?> GetDataBaseContent(string dbName)
        {
            var tables = await GetAllTableNames(dbName);
            var funcs = await GetAllFunctions(dbName);
            return new DataBase(dbName, tables, funcs);
        }


        public async Task<string[]?> GetAllTableNames()
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
        public async Task<string[]?> GetAllTableNames(string dbName)
        {
            if (IsConnected)
            {
                var db = new PostgresApi();
                await db.ConnectAsync(ConnectionData with { Database = dbName });
                return await db.GetAllTableNames();
            }
            return null;
        }


        public async Task<Table?> GetTableContent(string tableName)
        {
            if (IsConnected)
            {
                var table = await ExecuteCommand($"select * from {tableName}");
                if (table != null)
                {
                    table.TableName = tableName;
                    return table;
                }
            }
            return null;
        }
        public async Task<Table?> GetTableContent(string tableName, string dbName)
        {
            if (IsConnected)
            {
                var db = new PostgresApi();
                await db.ConnectAsync(ConnectionData with { Database = dbName });
                return await db.GetTableContent(tableName);
            }
            return null;
        }


        public async Task<Function[]?> GetAllFunctions()
        {
            if (IsConnected)
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
            }
            return null;
        }
        public async Task<Function[]?> GetAllFunctions(string dbName)
        {
            if (IsConnected)
            {
                var db = new PostgresApi();
                await db.ConnectAsync(ConnectionData with { Database = dbName });
                return await db.GetAllFunctions();
            }
            return null;
        }


        public async Task<bool> RemoveFunction(Function function)
        {

            if (IsConnected)
            {
                await ExecuteCommand($"drop function {function.Name}({string.Join(',', function.Arguments)})");

                return true;
            }

            return false;
        }
        public async Task<bool> UpdateFunction(Function function, string newFuncCode)
        {
            if (IsConnected)
            {
                await ExecuteCommand("Begin;");

                await RemoveFunction(function);

                try
                {
                    await ExecuteCommand(newFuncCode);
                }
                catch
                {

                    await ExecuteCommand("rollback;");
                    throw;
                }

                await ExecuteCommand("commit;");
                return true;
            }
            return false;
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

        public async Task<bool> ExportDataBase(string outputPath)
        {
            if(IsConnected)
            {
                var directory = await ExecuteCommand("SELECT * FROM pg_settings WHERE name = 'data_directory'");
                var dumExe = directory[0, 1].Replace("data", "bin/pg_dump.exe");

                Process cmd = new Process();
                cmd.StartInfo.FileName = "cmd.exe";

                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                cmd.Start();

                await cmd.StandardInput.WriteLineAsync($"set pgpassword={ConnectionData.Password}");
                await cmd.StandardInput.WriteLineAsync($"\"{dumExe}\" -h {ConnectionData.Host} -U {ConnectionData.Username} -d{ConnectionData.Database} -F tar -f {outputPath}");
                cmd.StandardInput.Close();

                await cmd.WaitForExitAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> ImportDataBase(string inputPath)
        {
            if (IsConnected)
            {
                var directory = await ExecuteCommand("SELECT * FROM pg_settings WHERE name = 'data_directory'");
                var restoreExe = directory[0, 1].Replace("data", "bin/pg_restore.exe");

                Process cmd = new Process();
                cmd.StartInfo.FileName = "cmd.exe";

                cmd.StartInfo.RedirectStandardError= true;
                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                cmd.Start();

                await cmd.StandardInput.WriteLineAsync($"set pgpassword={ConnectionData.Password}");
                await cmd.StandardInput.WriteLineAsync($"\"{restoreExe}\" --verbose --clean --no-acl --no-owner -h {ConnectionData.Host} -U {ConnectionData.Username} -d {ConnectionData.Database} {inputPath}");
                cmd.StandardInput.Close();

                await cmd.WaitForExitAsync();
                return true;

            }
            return false;

        }
    }
}
