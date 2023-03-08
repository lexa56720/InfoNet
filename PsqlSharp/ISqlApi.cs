using System.Data;

namespace PsqlSharp
{
    public interface ISqlApi
    {
        public ConnectionData? ConnectionData { get; }
        public bool IsConnected { get; }

        public event EventHandler ConnectionStatusChanged;


        public Task<bool> ConnectAsync(ConnectionData connectionData);
        public Task<bool> DisconnectAsync();

        public Task<Table[]> ExecuteCommandAsync(string command);
        public Table[] ExecuteCommand(string command);

        public Task<string[]> GetAllDataBaseNamesAsync();
        public Task<DataBase?> GetDataBaseContentAsync(string dbName);

        public Task<Table?> GetTableContentAsync(string tableName);
        public Task<Table?> GetTableContentAsync(string tableName, string dbName);

        public Task<string[]> GetAllTableNamesAsync();
        public Task<string[]> GetAllTableNamesAsync(string dbName);

        public Task<Function[]> GetAllFunctionsAsync();
        public Task<Function[]> GetAllFunctionsAsync(string dbName);

        public Task<bool> RemoveFunctionAsync(Function function);
        public Task<bool> UpdateFunctionAsync(Function updatedFunction);

        public Task<bool> RemoveRowAsync(string tableName, int rowIndex);
        public Task<bool> SetCellValueAsync(Table table, object cellValue, int columnIndex, int rowIndex);
        public Task<bool> AddRowAsync(Table table, object?[] values);

        public bool IsCanAddRow(DataTable table, DataRow row);
        public bool IsCanChangeRow(DataTable table, DataRow row);

        public Task<bool> ExportDataBaseAsync(string outputPath);
        public Task<bool> ImportDataBaseAsync(string inputPath);
    }
}
