using PsqlSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgresClient.Model
{
    class TableViewerModel : BaseModel
    {
      
        private Table SelectedTable 
        { 
            get => selectedTable;
            set
            {
                if(selectedTable!=null)
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
            var tables = await Api.GetAllTables();
            if (tables != null)
                return tables;
            else
                return new string[] { string.Empty };
        }

        public async Task<Table> GetSelectedTable(string tableName)
        {
            var table = await Api.GetTableContent(tableName);
            table ??= new Table();
            SelectedTable = table;
            return SelectedTable;
        }
    }
}
