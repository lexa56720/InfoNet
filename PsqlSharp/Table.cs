using System.Data;
using System.Diagnostics;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace PsqlSharp
{
    public class Table
    {
        public int ColumnCount => DataTable.Columns.Count;
        public int RowCount => DataTable.Rows.Count;

        public string? TableName { get; internal set; }
        public string[]? ColumnNames { get; private set; }
        public Type[]? ColumnTypes { get; private set; }
        public DataTable DataTable { get; }

        public event EventHandler<string[]>? RowAdded;

        public event EventHandler<CellChangedEventArgs>? CellChanged;

        public string this[int i, int j] => DataTable.Rows[i][j].ToString() ?? string.Empty;

        public Table()
        {
            DataTable = new DataTable();
        }

        public Table(DataTable table)
        {
            DataTable = table;
            ColumnsSetup(table);

            DataTable.ColumnChanged += TableColumnChanged;
            DataTable.RowChanged += TableRowChanged;
        }

        private void ColumnsSetup(DataTable table)
        {
            ColumnNames = new string[ColumnCount];
            ColumnTypes = new Type[ColumnCount];
            for (var i = 0; i < table.Columns.Count; i++)
            {
                ColumnTypes[i] = table.Columns[i].DataType;
                ColumnNames[i] = table.Columns[i].ColumnName;
            }
        }
        public int IndexOfRow(DataRow row)
        {
            for (var i = 0; i < DataTable.Rows.Count; i++)
                if (DataTable.Rows[i] == row)
                    return i;
            return -1;
        }
        public void RemoveRow(DataRow row)
        {
            DataTable.Rows.RemoveAt(IndexOfRow(row));
        }

        public override string ToString()
        {
            const string separator = "\t";

            var longestInColumn = new int[ColumnCount];
            for (var j = 0; j < ColumnCount; j++)
            {
                longestInColumn[j] = ColumnNames[j].Length;
                for (int i = 0; i < RowCount; i++)
                {
                    if (this[i, j].Length > longestInColumn[j])
                        longestInColumn[j] = this[i, j].Length;
                }
            }
               


            var builder = new StringBuilder();
            var formater = new StringBuilder();
            for (var j = 0; j < ColumnCount; j++)
                formater.Append($"{{{j},{-longestInColumn[j]-1}}} ");

            builder.Append(string.Format(formater.ToString(), ColumnNames));
            builder.Append("\n\n");


            string[] columns=new string[ColumnCount];
            for (var i = 0; i < RowCount; i++)
            {
                for (var j = 0; j < ColumnCount; j++)
                    columns[j] = this[i, j];
                builder.Append(string.Format(formater.ToString(), columns));
                builder.Append('\n');
            }
            Debug.WriteLine(builder.ToString());
            return builder.ToString();
        }

        private void TableColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            // throw new NotImplementedException();
            var rowNumber = DataTable.Rows.IndexOf(e.Row);
            if (rowNumber >= 0)
            {
                var columnName = e.Column.ColumnName;
                var value = e.ProposedValue;
                CellChanged?.Invoke(this, new CellChangedEventArgs(columnName, rowNumber, value));
            }

        }

        private void TableRowChanged(object sender, DataRowChangeEventArgs e)
        {
            //throw new NotImplementedException();
            if (e.Action == DataRowAction.Add)
            {
                var values = new string[ColumnCount];
                for (var i = 0; i < ColumnCount; i++)
                    values[i] = e.Row.ItemArray[i].ToString();
                RowAdded?.Invoke(this, values);
            }
        }
    }

    public class CellChangedEventArgs : EventArgs
    {
        public int RowNumber { get; }

        public string ColumnName { get; }

        public object? Value { get; }
        public CellChangedEventArgs(string columnName, int rowNumber, object? value)
        {
            ColumnName = columnName;
            RowNumber = rowNumber;
            Value = value;
        }
    }
}
