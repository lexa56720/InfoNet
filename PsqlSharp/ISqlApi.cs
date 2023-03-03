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


        public Task<string[]?> GetAllDataBaseNames();
        public Task<DataBase?> GetDataBaseContent(string dbName);

        public Task<Table?> GetTableContent(string tableName);
        public Task<Table?> GetTableContent(string tableName, string dbName);


        public Task<string[]?> GetAllTableNames();
        public Task<string[]?> GetAllTableNames(string dbName);

        public Task<Function[]?> GetAllFunctions();
        public Task<Function[]?> GetAllFunctions(string dbName);

        public Task<bool> RemoveFunction(Function function);
        public Task<bool> UpdateFunction(Function updatedFunction);

        public Task<bool> RemoveRow(string tableName, int rowIndex);
        public Task<bool> SetColumnByRow(string tableName, string columnName, string cellValue, int rowIndex);
        public Task<bool> AddRow(Table table, string[] values);

        public Task<bool> ExportDataBase(string outputPath);

        public Task<bool> ImportDataBase(string inputPath);
    }
}
