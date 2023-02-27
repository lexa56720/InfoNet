using PsqlSharp;
using System.Threading.Tasks;

namespace PostgresClient.Model
{
    internal class TableViewerModel : BaseModel
    {
        private Table? SelectedTable
        {
            get => selectedTable;
            set
            {
                if (selectedTable != null)
                {
                    selectedTable.CellChanged -= SelectedTableCellChanged;
                    selectedTable.RowAdded -= SelectedTableRowAdded;
                }

                selectedTable = value;

                if (selectedTable != null)
                {
                    selectedTable.CellChanged += SelectedTableCellChanged;
                    selectedTable.RowAdded += SelectedTableRowAdded;
                }
            }
        }
        private Table? selectedTable;

        public TableViewerModel(ISqlApi api) : base(api)
        {

        }

        private void SelectedTableRowAdded(object? sender, string[] e)
        {
            Api.AddRow(SelectedTable, e);
        }

        private void SelectedTableCellChanged(object? sender, CellChangedEventArgs e)
        {
            Api.SetColumnByRow(SelectedTable.TableName, e.ColumnName, e.Value.ToString(), e.RowNumber);
        }
        public async Task<string[]> GetTables()
        {
            var tables = await Api.GetAllTableNames();
            return tables ?? new[] { string.Empty };
        }

        public async Task<Table> GetSelectedTable(string tableName)
        {
            var table = await Api.GetTableContent(tableName);
            table ??= new Table();
            SelectedTable = table;
            return SelectedTable;
        }

        public async Task<Table> GetTable(string tableName, string dbName)
        {
            var table = await Api.GetTableContent(tableName, dbName);
            table ??= new Table();
            SelectedTable = null;
            return table;
        }

    }
}
