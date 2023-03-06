using Microsoft.Extensions.Logging;
using NodaTime;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using System.Diagnostics;
using System.Text;

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
                    dataSourceBuilder.UseNodaTime();
                    await using var dataSource = dataSourceBuilder.Build();
                    Connection = await dataSource.OpenConnectionAsync();

                    IsConnected = true;
                    ConnectionData = connectionData;
                }
                catch
                {
                    IsConnected = false;
                    throw;
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

        public async Task<Table[]> ExecuteCommand(string command)
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

                    var tables = new Table[dataSet.Tables.Count];
                    for (var i = 0; i < dataSet.Tables.Count; i++)
                        tables[i] = new Table(dataSet.Tables[i]);

                    return tables;
                }
            }
            finally
            {
                Semaphore.Release();
            }
            return Array.Empty<Table>();
        }

        public async Task<string[]> GetAllDataBaseNames()
        {
            if (IsConnected)
            {
                var databases = (await ExecuteCommand("SELECT datname FROM pg_database where datistemplate='f';"))[0];
                var result = new string[databases.RowCount];
                for (var i = 0; i < databases.RowCount; i++)
                    result[i] = databases[i, 0];
                return result;
            }
            return Array.Empty<string>();
        }
        public async Task<DataBase?> GetDataBaseContent(string dbName)
        {
            var tables = await GetAllTableNames(dbName);
            var funcs = await GetAllFunctions(dbName);
            return new DataBase(dbName, tables, funcs);
        }


        public async Task<string[]> GetAllTableNames()
        {
            if (IsConnected)
            {
                var tables = (await ExecuteCommand(@"select * from pg_tables;"))[0];

                var result = new List<string>();
                for (var i = 0; i < tables.RowCount; i++)
                    if (tables[i, 0] == "public")
                        result.Add(tables[i, 1]);

                if (result.Count > 0)
                    return result.ToArray();
            }

            return Array.Empty<string>();
        }
        public async Task<string[]> GetAllTableNames(string dbName)
        {
            if (IsConnected)
            {
                var db = new PostgresApi();
                await db.ConnectAsync(ConnectionData with { Database = dbName });
                return await db.GetAllTableNames();
            }
            return Array.Empty<string>();
        }


        public async Task<Table?> GetTableContent(string tableName)
        {
            if (IsConnected)
            {
                var table = (await ExecuteCommand($"select * from {tableName}"))[0];
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


        public async Task<Function[]> GetAllFunctions()
        {
            if (IsConnected)
            {
                var funcTable = (await ExecuteCommand(
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
                    function_name; "))[0];
                if (funcTable != null)
                    return Function.Parse(funcTable);
            }
            return Array.Empty<Function>();
        }
        public async Task<Function[]> GetAllFunctions(string dbName)
        {
            if (IsConnected)
            {
                var db = new PostgresApi();
                await db.ConnectAsync(ConnectionData with { Database = dbName });
                return await db.GetAllFunctions();
            }
            return Array.Empty<Function>();
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
        public async Task<bool> UpdateFunction(Function updatedFunction)
        {
            if (IsConnected)
            {

                try
                {
                    await ExecuteCommand("Begin;");

                    await RemoveFunction(updatedFunction);
                    await ExecuteCommand(updatedFunction.SourceCode);
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

        public async Task<bool> RemoveRow(string tableName, int rowIndex)
        {
            try
            {
                var rowNumbers = (await ExecuteCommand($"select ctid, * from {tableName};"))[0];
                var ctid = ((NpgsqlTid)rowNumbers.DataTable.Rows[rowIndex][0]).ToString();
                await ExecuteCommand(
                    @$"Delete from {tableName}
                    WHERE ctid='{ctid}';");
                return true;
            }
            catch
            {
                throw;
            }

        }
        public async Task<bool> SetColumnByRow(Table table, object cellValue, int columnIndex, int rowIndex)
        {
            var value = ConvertValuesTypes(new object[] { cellValue }, table)[0];
            var rowNumbers = (await ExecuteCommand($"select ctid, * from {table.TableName};"))[0];
            var ctid = ((NpgsqlTid)rowNumbers.DataTable.Rows[rowIndex][0]).ToString();
            await ExecuteCommand(
                @$"UPDATE {table.TableName}
                    SET {table.DataTable.Columns[columnIndex].ColumnName} = {value}
                    WHERE ctid='{ctid}';");
            return true;
        }
        public async Task<bool> AddRow(Table table, object[] values)
        {
            values = ConvertValuesTypes(values, table);
            await ExecuteCommand(
               @$"INSERT INTO {table.TableName}({string.Join(", ", table.ColumnNames)})
                    VALUES ({string.Join(", ", values)})");
            return true;

        }
        private string[] ConvertValuesTypes(object[] values, Table table)
        {
            var result = new string[values.Length];
            var typeConverter = new Dictionary<Type, Func<object, string>>()
            {
                { typeof(string),o=>
                    {
                        return $"'{o}'";
                    }
                },
                { typeof(LocalDate),o=>
                    {
                        var date=(LocalDate)o;
                        return $"'{date.Year}-{date.Month}-{date.Day}'";
                    }
                },
                { typeof(Period),o=>
                    {
                        var period=(Period)o;
                        return $"'{period.ToString()}'";
                    }
                },
            };
            for (var i = 0; i < values.Length; i++)
            {
                if (values[i].GetType() == typeof(DBNull))
                {
                    result[i] = "null";
                    continue;
                }
                if (typeConverter.ContainsKey(table.ColumnTypes[i]))
                    result[i] = typeConverter[table.ColumnTypes[i]](values[i]);
                else
                    result[i] = values[i].ToString();
            }

            return result;
        }

        public async Task<bool> ExportDataBase(string outputPath)
        {
            if (IsConnected)
            {
                var directory = (await ExecuteCommand("SELECT * FROM pg_settings WHERE name = 'data_directory'"))[0];
                var dumExe = directory[0, 1].Replace("data", "bin/pg_dump.exe");

                using var cmd = new Process();
                cmd.StartInfo.FileName = "cmd.exe";

                cmd.StartInfo.RedirectStandardError = true;
                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                cmd.Start();

                await cmd.StandardInput.WriteLineAsync($"set pgpassword={ConnectionData.Password}");
                await cmd.StandardInput.WriteLineAsync($"\"{dumExe}\" -h {ConnectionData.Host} -U {ConnectionData.Username} -d{ConnectionData.Database} -F tar -f {outputPath}");

                cmd.StandardInput.Close();
                cmd.StandardError.Close();

                await cmd.WaitForExitAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> ImportDataBase(string inputPath)
        {
            if (IsConnected)
            {
                var directory = (await ExecuteCommand("SELECT * FROM pg_settings WHERE name = 'data_directory'"))[0];
                var restoreExe = directory[0, 1].Replace("data", "bin/pg_restore.exe");
                await ClearDataBase();

                using var cmd = new Process();
                cmd.StartInfo.FileName = "cmd.exe";

                cmd.StartInfo.RedirectStandardError = true;
                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                cmd.Start();

                await cmd.StandardInput.WriteLineAsync($"set pgpassword={ConnectionData.Password}");
                await cmd.StandardInput.WriteLineAsync($"\"{restoreExe}\" --verbose --clean --no-acl --no-owner -h {ConnectionData.Host} -U {ConnectionData.Username} -d {ConnectionData.Database} {inputPath}");

                cmd.StandardInput.Close();
                cmd.StandardError.Close();

                await cmd.WaitForExitAsync();
                return true;

            }
            return false;

        }

        private async Task<bool> ClearDataBase()
        {
            var dropDBCommand = new StringBuilder();

            var tables = await GetAllTableNames();
            foreach (var table in tables)
                dropDBCommand.Append($"Drop table {table} cascade;");

            var funcs = await GetAllFunctions();
            foreach (var func in funcs)
                dropDBCommand.Append($"drop function {func.Name}({string.Join(',', func.Arguments)});");

            if (dropDBCommand.Length >0)
                await ExecuteCommand(dropDBCommand.ToString());
            return true;
        }

    }
}
