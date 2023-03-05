using PsqlSharp;
using System;
using System.Data;
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
        public async Task DeleteRow(DataRow row)
        {
            if (SelectedTable != null)
            {
                if (await Api.RemoveRow(SelectedTable.TableName, SelectedTable.IndexOfRow(row)))
                    SelectedTable.RemoveRow(row);
            }
        }

        private void SelectedTableRowAdded(object? sender, object[] e)
        {
            Api.AddRow(SelectedTable, e);
        }

        private void SelectedTableCellChanged(object? sender, CellChangedEventArgs e)
        {
            Api.SetColumnByRow(SelectedTable, e.Value, e.ColumnIndex, e.RowIndex);
        }
        public async Task<string[]> GetTables()
        {
            var tables = await Api.GetAllTableNames();
            return tables ?? Array.Empty<string>();
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
