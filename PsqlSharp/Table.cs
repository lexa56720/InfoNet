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

        public event EventHandler<object[]>? RowAdded;

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

        private string GetFormater()
        {
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
            var formater = new StringBuilder();
            for (var j = 0; j < ColumnCount; j++)
                formater.Append($"{{{j},{-longestInColumn[j] - 1}}} ");
            return formater.ToString();
        }
        public override string ToString()
        {
            var formater = GetFormater();

            var builder = new StringBuilder();
            builder.Append(string.Format(formater, ColumnNames)).Append("\n\n"); ;

            string[] columns=new string[ColumnCount];
            for (var i = 0; i < RowCount; i++)
            {
                for (var j = 0; j < ColumnCount; j++)
                    columns[j] = this[i, j];
                builder.Append(string.Format(formater, columns)).Append('\n');
            }
            return builder.ToString();
        }

        private void TableColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            var rowNumber = DataTable.Rows.IndexOf(e.Row);
            if (rowNumber >= 0)
            {
                var columnIndex = DataTable.Columns.IndexOf(e.Column);
                var value = e.ProposedValue;
                CellChanged?.Invoke(this, new CellChangedEventArgs(columnIndex, rowNumber, value));
            }

        }

        private void TableRowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Add)
            {
                var values = new object[ColumnCount];
                for (var i = 0; i < ColumnCount; i++)
                    values[i] = e.Row.ItemArray[i];
                RowAdded?.Invoke(this, values);
            }
        }
    }

    public class CellChangedEventArgs : EventArgs
    {
        public int RowIndex { get; }

        public int ColumnIndex { get; }

        public object? Value { get; }
        public CellChangedEventArgs(int columnIndex, int rowIndex, object? value)
        {
            ColumnIndex = columnIndex;
            RowIndex = rowIndex;
            Value = value;
        }
    }
}
