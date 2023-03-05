using PsqlSharp;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PostgresClient.Utils
{
    public class ClientApi : ISqlApi
    {
        private ISqlApi SqlApi { get; }

        public event EventHandler<Exception> ExceptionOccured;

        public event EventHandler<string> SuccesExecution;

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

        public async Task<bool> AddRow(Table table, object[] values)
        {
            try
            {
                var result = await SqlApi.AddRow(table, values);
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

        public async Task<Table[]?> ExecuteCommand(string command)
        {
            try
            {
                var result = await SqlApi.ExecuteCommand(command);
                OnSuccssesExecution();
                return result;
            }
            catch (Exception e)
            {
                OnExceprionOccured(e);
                return null;
            }
        }

        public async Task<string[]?> GetAllDataBaseNames()
        {
            try
            {
                var result = await SqlApi.GetAllDataBaseNames();
                OnSuccssesExecution();
                return result;
            }
            catch (Exception e)
            {
                OnExceprionOccured(e);
                return null;
            }
        }

        public async Task<Function[]?> GetAllFunctions()
        {
            try
            {
                var result = await SqlApi.GetAllFunctions();
                OnSuccssesExecution();
                return result;
            }
            catch (Exception e)
            {
                OnExceprionOccured(e);
                return null;
            }
        }

        public async Task<Function[]?> GetAllFunctions(string dbName)
        {
            try
            {
                var result = await SqlApi.GetAllFunctions(dbName);
                OnSuccssesExecution();
                return result;
            }
            catch (Exception e)
            {
                OnExceprionOccured(e);
                return null;
            }
        }

        public async Task<string[]?> GetAllTableNames()
        {
            try
            {
                var result = await SqlApi.GetAllTableNames();
                OnSuccssesExecution();
                return result;
            }
            catch (Exception e) { OnExceprionOccured(e); return null; }
        }

        public async Task<string[]?> GetAllTableNames(string dbName)
        {
            try
            {
                var result = await SqlApi.GetAllTableNames(dbName);
                OnSuccssesExecution();
                return result;
            }
            catch (Exception e) { OnExceprionOccured(e); return null; }
        }

        public async Task<DataBase?> GetDataBaseContent(string dbName)
        {
            try
            {
                var result = await SqlApi.GetDataBaseContent(dbName);
                OnSuccssesExecution();
                return result;
            }
            catch (Exception e) { OnExceprionOccured(e); return null; }
        }

        public async Task<Table?> GetTableContent(string tableName)
        {
            try
            {
                var result = await SqlApi.GetTableContent(tableName);
                OnSuccssesExecution();
                return result;
            }
            catch (Exception e) { OnExceprionOccured(e); return null; }
        }

        public async Task<Table?> GetTableContent(string tableName, string dbName)
        {
            try
            {
                var result = await SqlApi.GetTableContent(tableName, dbName);
                OnSuccssesExecution();
                return result;
            }
            catch (Exception e) { OnExceprionOccured(e); return null; }
        }

        public async Task<bool> RemoveFunction(Function function)
        {
            try
            {
                var result = await SqlApi.RemoveFunction(function);
                OnSuccssesExecution();
                return result;
            }
            catch (Exception e) { OnExceprionOccured(e); return false; }
        }

        public async Task<bool> SetColumnByRow(Table table, object cellValue, int columnIndex, int rowCount)
        {
            try
            {
                var result = await SqlApi.SetColumnByRow(table, cellValue, columnIndex, rowCount);
                OnSuccssesExecution();
                return result;
            }
            catch (Exception e) { OnExceprionOccured(e); return false; }
        }

        public async Task<bool> UpdateFunction(Function updatedFunction)
        {
            try
            {
                var result = await SqlApi.UpdateFunction(updatedFunction); ;
                OnSuccssesExecution();
                return result;
            }
            catch (Exception e) { OnExceprionOccured(e); return false; }
        }
        public async Task<bool> ExportDataBase(string outputPath)
        {
            try
            {
                var result = await SqlApi.ExportDataBase(outputPath);
                OnSuccssesExecution();
                return result;
            }
            catch (Exception e) { OnExceprionOccured(e); return false; }
        }

        public async Task<bool> ImportDataBase(string inputPath)
        {
            try
            {
                var result = await SqlApi.ImportDataBase(inputPath);
                OnSuccssesExecution();
                return result;
            }
            catch (Exception e) { OnExceprionOccured(e); return false; }
        }

        public async Task<bool> RemoveRow(string tableName, int rowIndex)
        {
            try
            {
                var result = await SqlApi.RemoveRow(tableName, rowIndex); ;
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


    }
}
