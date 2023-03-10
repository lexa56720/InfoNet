﻿using PsqlSharp;
using System.Data;

namespace PostgresClient.Model
{
    internal class TableViewerModel : BaseModel
    {
        private Table SelectedTable
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

                selectedTable.CellChanged += SelectedTableCellChanged;
                selectedTable.RowAdded += SelectedTableRowAdded;
            }
        }
        private Table selectedTable;

        public TableViewerModel(ISqlApi api) : base(api)
        {

        }
        public async Task DeleteRow(DataRow row)
        {
            if (await Api.RemoveRowAsync(SelectedTable.TableName, SelectedTable.Rows.IndexOf(row)))
                SelectedTable.RemoveRow(row);
        }

        private void SelectedTableRowAdded(object? sender, DataRow e)
        {
            Task.Run(async () =>
            {
                await Api.AddRowAsync(SelectedTable, e.ItemArray);
            });
        }

        private void SelectedTableCellChanged(object? sender, CellChangedEventArgs e)
        {
            Api.SetCellValueAsync(SelectedTable, e.Value, e.ColumnIndex, e.RowIndex);
        }
        public async Task<string[]> GetTables()
        {
            var tables = await Api.GetAllTableNamesAsync();
            return tables ?? Array.Empty<string>();
        }

        public async Task<Table> GetSelectedTable(string tableName)
        {
            var table = await Api.GetTableContentAsync(tableName);
            table ??= new Table();
            SelectedTable = table;
            return SelectedTable;
        }

        public async Task<Table> GetTable(string tableName, string dbName)
        {
            var table = await Api.GetTableContentAsync(tableName, dbName);
            table ??= new Table();
            return table;
        }

    }
}
