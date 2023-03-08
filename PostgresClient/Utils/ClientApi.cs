using PsqlSharp;
using System;
using System.Data;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PostgresClient.Utils
{
    public class ClientApi : ISqlApi
    {
        private ISqlApi SqlApi { get; }

        public event EventHandler<Exception>? ExceptionOccured;

        public event EventHandler<string>? SuccesExecution;

        public ClientApi(ISqlApi sqlApi)
        {
            SqlApi = sqlApi;
        }

        public bool IsConnected => SqlApi.IsConnected;

        public ConnectionData? ConnectionData => SqlApi.ConnectionData;


        public event EventHandler ConnectionStatusChanged
        {
            add => SqlApi.ConnectionStatusChanged += value;

            remove => SqlApi.ConnectionStatusChanged -= value;
        }

        public async Task<bool> AddRowAsync(Table table, object?[] values)
        {
            try
            {
                var result = await SqlApi.AddRowAsync(table, values);
                OnSuccssesExecution();
                return result;
            }
            catch (Exception e)
            {
                OnExceprionOccured(e);
                return false;
            }
        }

        public async Task<bool> ConnectAsync(ConnectionData connectionData)
        {
            try
            {
                var result = await SqlApi.ConnectAsync(connectionData);
                OnSuccssesExecution();
                return result;
            }
            catch (Exception e)
            {
                OnExceprionOccured(e);
                return false;
            }
        }

        public async Task<bool> DisconnectAsync()
        {
            try
            {
                var result = await SqlApi.DisconnectAsync();
                OnSuccssesExecution();
                return result;
            }
            catch (Exception e) { OnExceprionOccured(e); return false; }
        }

        public async Task<Table[]> ExecuteCommandAsync(string command)
        {
            try
            {
                var result = await SqlApi.ExecuteCommandAsync(command);
                OnSuccssesExecution();
                return result;
            }
            catch (Exception e)
            {
                OnExceprionOccured(e);
                return Array.Empty<Table>();
            }
        }

        public async Task<string[]> GetAllDataBaseNamesAsync()
        {
            try
            {
                var result = await SqlApi.GetAllDataBaseNamesAsync();
                OnSuccssesExecution();
                return result;
            }
            catch (Exception e)
            {
                OnExceprionOccured(e);
                return Array.Empty<string>();
            }
        }

        public async Task<Function[]> GetAllFunctionsAsync()
        {
            try
            {
                var result = await SqlApi.GetAllFunctionsAsync();
                OnSuccssesExecution();
                return result;
            }
            catch (Exception e)
            {
                OnExceprionOccured(e);
                return Array.Empty<Function>();
            }
        }

        public async Task<Function[]> GetAllFunctionsAsync(string dbName)
        {
            try
            {
                var result = await SqlApi.GetAllFunctionsAsync(dbName);
                OnSuccssesExecution();
                return result;
            }
            catch (Exception e)
            {
                OnExceprionOccured(e);
                return Array.Empty<Function>();
            }
        }

        public async Task<string[]> GetAllTableNamesAsync()
        {
            try
            {
                var result = await SqlApi.GetAllTableNamesAsync();
                OnSuccssesExecution();
                return result;
            }
            catch (Exception e) { OnExceprionOccured(e); return Array.Empty<string>(); }
        }

        public async Task<string[]> GetAllTableNamesAsync(string dbName)
        {
            try
            {
                var result = await SqlApi.GetAllTableNamesAsync(dbName);
                OnSuccssesExecution();
                return result;
            }
            catch (Exception e) { OnExceprionOccured(e); return Array.Empty<string>(); }
        }

        public async Task<DataBase?> GetDataBaseContentAsync(string dbName)
        {
            try
            {
                var result = await SqlApi.GetDataBaseContentAsync(dbName);
                OnSuccssesExecution();
                return result;
            }
            catch (Exception e) { OnExceprionOccured(e); return null; }
        }

        public async Task<Table?> GetTableContentAsync(string tableName)
        {
            try
            {
                var result = await SqlApi.GetTableContentAsync(tableName);
                OnSuccssesExecution();
                return result;
            }
            catch (Exception e) { OnExceprionOccured(e); return null; }
        }

        public async Task<Table?> GetTableContentAsync(string tableName, string dbName)
        {
            try
            {
                var result = await SqlApi.GetTableContentAsync(tableName, dbName);
                OnSuccssesExecution();
                return result;
            }
            catch (Exception e) { OnExceprionOccured(e); return null; }
        }

        public async Task<bool> RemoveFunctionAsync(Function function)
        {
            try
            {
                var result = await SqlApi.RemoveFunctionAsync(function);
                OnSuccssesExecution();
                return result;
            }
            catch (Exception e) { OnExceprionOccured(e); return false; }
        }

        public async Task<bool> SetCellValueAsync(Table table, object cellValue, int columnIndex, int rowCount)
        {
            try
            {
                var result = await SqlApi.SetCellValueAsync(table, cellValue, columnIndex, rowCount);
                OnSuccssesExecution();
                return result;
            }
            catch (Exception e) { OnExceprionOccured(e); return false; }
        }

        public async Task<bool> UpdateFunctionAsync(Function updatedFunction)
        {
            try
            {
                var result = await SqlApi.UpdateFunctionAsync(updatedFunction); ;
                OnSuccssesExecution();
                return result;
            }
            catch (Exception e) { OnExceprionOccured(e); return false; }
        }
        public async Task<bool> ExportDataBaseAsync(string outputPath)
        {
            try
            {
                var result = await SqlApi.ExportDataBaseAsync(outputPath);
                OnSuccssesExecution();
                return result;
            }
            catch (Exception e) { OnExceprionOccured(e); return false; }
        }

        public async Task<bool> ImportDataBaseAsync(string inputPath)
        {
            try
            {
                var result = await SqlApi.ImportDataBaseAsync(inputPath);
                OnSuccssesExecution();
                return result;
            }
            catch (Exception e) { OnExceprionOccured(e); return false; }
        }

        public async Task<bool> RemoveRowAsync(string tableName, int rowIndex)
        {
            try
            {
                var result = await SqlApi.RemoveRowAsync(tableName, rowIndex); ;
                OnSuccssesExecution();
                return result;
            }
            catch (Exception e) { OnExceprionOccured(e); return false; }
        }

        private void OnExceprionOccured(Exception exception)
        {
            ExceptionOccured?.Invoke(this, exception);
        }

        private void OnSuccssesExecution([CallerMemberName] string callerName = "")
        {
            SuccesExecution?.Invoke(this, callerName);
        }

        public Table[] ExecuteCommand(string command)
        {
            try
            {
                var result = SqlApi.ExecuteCommand(command);
                OnSuccssesExecution();
                return result;
            }
            catch (Exception e)
            {
                OnExceprionOccured(e);
                return Array.Empty<Table>();
            }
        }

        public bool IsCanAddRow(DataTable table, DataRow row)
        {
            try
            {
                var result = SqlApi.IsCanAddRow(table, row);
                OnSuccssesExecution();
                return result;
            }
            catch (Exception e)
            {
                OnExceprionOccured(e);
                return false;
            }
        }

        public bool IsCanChangeRow(DataTable table, DataRow row)
        {
            try
            {
                var result = SqlApi.IsCanChangeRow(table, row);
                OnSuccssesExecution();
                return result;
            }
            catch (Exception e)
            {
                OnExceprionOccured(e);
                return false;
            }
        }
    }
}
