using Microsoft.Extensions.Logging;
using NodaTime;
using Npgsql;
using NpgsqlTypes;
using SqlApi;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace PsqlSharp
{
    public class PostgresApi : ISqlApi
    {
        public ConnectionData? ConnectionData { get; private set; }
        private NpgsqlConnection? Connection { get; set; }

        private Dictionary<Type, Func<object, string>> TypeConverter = new Dictionary<Type, Func<object, string>>()
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
                        return $"'{period}'";
                    }
                },
            };

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
            if (IsConnected)
                return IsConnected;
            try
            {
                var connectionString = connectionData.ToString();
                var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);

                dataSourceBuilder.UseLoggerFactory(LoggerFactory.Create(builder =>
                {
                    builder
                    .AddDebug()
                    .SetMinimumLevel(LogLevel.None);
                }));
                dataSourceBuilder.UseNodaTime();
                await using var dataSource = dataSourceBuilder.Build();
                Connection = await dataSource.OpenConnectionAsync();

                IsConnected = true;
                ConnectionData = connectionData;
                return IsConnected;
            }
            catch
            {
                IsConnected = false;
                throw;
            }
        }
        public async Task<bool> DisconnectAsync()
        {
            if (!IsConnected)
                return false;

            await Connection.CloseAsync();
            IsConnected = false;
            return true;
        }

        public async Task<Table[]> ExecuteCommandAsync(string command)
        {
            if (!IsConnected)
                return Array.Empty<Table>();

            await Semaphore.WaitAsync();
            try
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
            finally
            {
                Semaphore.Release();
            }
        }
        public Table[] ExecuteCommand(string command)
        {
            if (!IsConnected)
                return Array.Empty<Table>();

            Semaphore.Wait();
            try
            {
                using var commandObj = new NpgsqlCommand(command, Connection);
                using var adapter = new NpgsqlDataAdapter(commandObj);
                var dataSet = new DataSet();
                adapter.Fill(dataSet);

                var tables = new Table[dataSet.Tables.Count];
                for (var i = 0; i < dataSet.Tables.Count; i++)
                    tables[i] = new Table(dataSet.Tables[i]);

                return tables;
            }
            finally
            {
                Semaphore.Release();
            }
        }

        public async Task<string[]> GetAllDataBaseNamesAsync()
        {
            if (!IsConnected)
                return Array.Empty<string>();

            var databases = (await ExecuteCommandAsync("SELECT datname FROM pg_database where datistemplate='f';"))[0];
            var result = new string[databases.RowCount];
            for (var i = 0; i < databases.RowCount; i++)
                result[i] = databases[i, 0];
            return result;
        }
        public async Task<DataBase?> GetDataBaseContentAsync(string dbName)
        {
            var tables = await GetAllTableNamesAsync(dbName);
            var funcs = await GetAllFunctionsAsync(dbName);
            return new DataBase(dbName, tables, funcs);
        }

        public async Task<string[]> GetAllTableNamesAsync()
        {
            if (!IsConnected)
                return Array.Empty<string>();

            var tables = (await ExecuteCommandAsync(@"select * from pg_tables;"))[0];

            var result = new List<string>();
            for (var i = 0; i < tables.RowCount; i++)
                if (tables[i, 0] == "public")
                    result.Add(tables[i, 1]);

            if (result.Count > 0)
                return result.ToArray();

            return Array.Empty<string>();
        }
        public async Task<string[]> GetAllTableNamesAsync(string dbName)
        {
            if (!IsConnected)
                return Array.Empty<string>();

            var db = new PostgresApi();
            await db.ConnectAsync(ConnectionData with { Database = dbName });
            return await db.GetAllTableNamesAsync();
        }

        public async Task<Table?> GetTableContentAsync(string tableName)
        {
            if (!IsConnected)
                return null;

            var table = (await ExecuteCommandAsync($"select * from {tableName}"));
            if (table.Length == 0)
                return null;

            table[0].TableName = tableName;
            return table[0];
        }
        public async Task<Table?> GetTableContentAsync(string tableName, string dbName)
        {
            if (!IsConnected)
                return null;

            var db = new PostgresApi();
            await db.ConnectAsync(ConnectionData with { Database = dbName });
            return await db.GetTableContentAsync(tableName);
        }

        public async Task<Function[]> GetAllFunctionsAsync()
        {
            if (!IsConnected)
                return Array.Empty<Function>();
            var funcTable = (await ExecuteCommandAsync(
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
                    function_name; "));
            if (funcTable.Length == 0)
                return Array.Empty<Function>();

            return Function.Parse(funcTable[0]);
        }
        public async Task<Function[]> GetAllFunctionsAsync(string dbName)
        {
            if (!IsConnected)
                return Array.Empty<Function>();

            var db = new PostgresApi();
            await db.ConnectAsync(ConnectionData with { Database = dbName });
            return await db.GetAllFunctionsAsync();
        }

        public async Task<bool> RemoveFunctionAsync(Function function)
        {
            if (!IsConnected)
                return false;

            await ExecuteCommandAsync($"drop function {function.Name}({string.Join(',', function.Arguments)})");

            return true;
        }
        public async Task<bool> UpdateFunctionAsync(Function updatedFunction)
        {
            if (!IsConnected)
                return false;
            try
            {
                await ExecuteCommandAsync("Begin;");
                await RemoveFunctionAsync(updatedFunction);
                await ExecuteCommandAsync(updatedFunction.SourceCode);
            }
            catch
            {
                await ExecuteCommandAsync("rollback;");
                throw;
            }
            await ExecuteCommandAsync("commit;");
            return true;
        }

        public async Task<bool> RemoveRowAsync(string tableName, int rowIndex)
        {
            var rowNumbers = (await ExecuteCommandAsync($"select ctid, * from {tableName};"))[0];
            var ctid = ((NpgsqlTid)rowNumbers.DataTable.Rows[rowIndex][0]).ToString();
            await ExecuteCommandAsync(
                @$"Delete from {tableName}
                    WHERE ctid='{ctid}';");
            return true;
        }
        public async Task<bool> SetCellValueAsync(Table table, object cellValue, int columnIndex, int rowIndex)
        {
            rowIndex = table.GetGlobalIndexByInner(rowIndex);
            var value = ConvertValuesTypes(new object[] { cellValue })[0];
            var rowNumbers = (await ExecuteCommandAsync($"select ctid, * from {table.TableName};"))[0];
            var ctid = ((NpgsqlTid)rowNumbers.DataTable.Rows[rowIndex][0]).ToString();
            await ExecuteAndUpdateRows(@$"
                UPDATE {table.TableName}
                SET {table.DataTable.Columns[columnIndex].ColumnName} = {value}
                WHERE ctid='{ctid}';", table);
            return true;
        }

        public async Task<bool> AddRowAsync(Table table, object?[] values)
        {
            values = ConvertValuesTypes(values);
            await ExecuteAndUpdateRows(
               @$"INSERT INTO {table.TableName}({string.Join(", ", table.ColumnNames)})
                    VALUES ({string.Join(", ", values)})", table);
            return true;
        }
        public async Task<bool> ExportDataBaseAsync(string outputPath)
        {
            if (!IsConnected)
                return false;

            var directory = (await ExecuteCommandAsync("SELECT * FROM pg_settings WHERE name = 'data_directory'"))[0];
            var dumpExe = directory[0, 1].Replace("data", "bin/pg_dump.exe");
            var codePage = CultureInfo.CurrentCulture.TextInfo.ANSICodePage;

            using var cmd = new Process();

            cmd.StartInfo.FileName = "cmd.exe";

            cmd.StartInfo.RedirectStandardError = true;
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.CreateNoWindow = false;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            await cmd.StandardInput.WriteLineAsync($"chcp {codePage}");
            await cmd.StandardInput.WriteLineAsync($"set pgpassword={ConnectionData.Password}");
            await cmd.StandardInput.WriteLineAsync($"\"{dumpExe}\" -h {ConnectionData.Host} -U {ConnectionData.Username}" +
                $" -d{ConnectionData.Database} -F tar -f \"{outputPath}\"");

      
            cmd.StandardInput.Close();
            cmd.StandardError.Close();

            await cmd.WaitForExitAsync();
            return true;
        }
        public async Task<bool> ImportDataBaseAsync(string inputPath)
        {
            if (!IsConnected)
                return false;

            var directory = (await ExecuteCommandAsync("SELECT * FROM pg_settings WHERE name = 'data_directory'"))[0];
            var restoreExe = directory[0, 1].Replace("data", "bin/pg_restore.exe");
            var codePage = CultureInfo.CurrentCulture.TextInfo.ANSICodePage;
            await ClearDataBase();

            using var cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardError = true;
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            await cmd.StandardInput.WriteLineAsync($"chcp {codePage}");
            await cmd.StandardInput.WriteLineAsync($"set pgpassword={ConnectionData.Password}");
            await cmd.StandardInput.WriteLineAsync($"\"{restoreExe}\" --verbose --clean --no-acl --no-owner -h {ConnectionData.Host} " +
                $"-U {ConnectionData.Username} -d {ConnectionData.Database} \"{inputPath}\"");

            cmd.StandardInput.Close();
            cmd.StandardError.Close();

            await cmd.WaitForExitAsync();
            return true;
        }

        public bool IsCanAddRow(DataTable dataTable, DataRow row)
        {
            try
            {
                var table = dataTable.ExtendedProperties["Table"] as Table;

                var values = ConvertValuesTypes(row.ItemArray);
                ExecuteCommand(@$"begin; 
                    INSERT INTO {dataTable.TableName}({string.Join(", ", table.ColumnNames)})
                    VALUES ({string.Join(", ", values)});
                            rollback;");
                return true;
            }
            catch
            {
                ExecuteCommand("rollback;");
                throw;
            }
        }
        public bool IsCanChangeRow(DataTable dataTable, DataRow row)
        {
            try
            {
                var table = dataTable.ExtendedProperties["Table"] as Table;
                var values = ConvertValuesTypes(row.ItemArray);

                var builder = new StringBuilder();
                for (var i = 0; i < dataTable.Columns.Count; i++)
                    builder.Append($"{table.ColumnNames[i]}={values[i]},");
                var command = builder.ToString(0, builder.Length - 1);

                var tableRowNumbers = ExecuteCommand($"select ctid, * from {dataTable.TableName};")[0];
                var ctid = ((NpgsqlTid)tableRowNumbers.DataTable.Rows[table.GetIndexByRow(row)][0]).ToString();

                if (command.Length > 0)
                    ExecuteCommand(@$"begin; UPDATE {dataTable.TableName} SET {command} WHERE ctid='{ctid}'; rollback;");

                return true;
            }
            catch
            {
                ExecuteCommand("rollback;");
                throw;
            }
        }

        private async Task ExecuteAndUpdateRows(string command, Table table)
        {
            var result = (await ExecuteCommandAsync(@$"
                Select ctid,* from {table.TableName};
                {command};
                Select ctid,* from {table.TableName};"));
            int indexFrom = -1, indexTo = -1;

            var a = result[1].DataTable.AsEnumerable().Select(r => r.ItemArray.First().ToString());
            var b = result[0].DataTable.AsEnumerable().Select(r => r.ItemArray.First().ToString());

            indexFrom = b.ToList().FindIndex(b => !a.Contains(b, StringComparer.Ordinal));
            indexTo = a.ToList().FindIndex(a => !b.Contains(a, StringComparer.Ordinal));

            if (indexTo >= 0 && indexFrom != indexTo)
                table.UpdateIndex(indexFrom, indexTo);
        }


        private async Task<bool> ClearDataBase()
        {
            var dropDBCommand = new StringBuilder();

            var tables = await GetAllTableNamesAsync();
            foreach (var table in tables)
                dropDBCommand.Append($"Drop table {table} cascade;");

            var funcs = await GetAllFunctionsAsync();
            foreach (var func in funcs)
                dropDBCommand.Append($"drop function {func.Name}({string.Join(',', func.Arguments)});");

            if (dropDBCommand.Length > 0)
                await ExecuteCommandAsync(dropDBCommand.ToString());
            return true;
        }
        private string[] ConvertValuesTypes(object?[] values)
        {
            var result = new string[values.Length];
            for (var i = 0; i < values.Length; i++)
            {
                if (values[i] != null && values[i].GetType() != typeof(DBNull))
                {
                    if (TypeConverter.ContainsKey(values[i].GetType()))
                        result[i] = TypeConverter[values[i].GetType()](values[i]);
                    else
                        result[i] = values[i].ToString();
                }
                else
                {
                    result[i] = "null";
                    continue;
                }
            }
            return result;
        }
    }
}
