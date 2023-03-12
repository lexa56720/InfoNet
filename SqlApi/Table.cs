﻿using System.Data;
using System.Text;

namespace SqlApi
{
    public class Table
    {
        public int ColumnCount => DataTable.Columns.Count;
        public int RowCount => DataTable.Rows.Count;

        public DataRowCollection Rows => DataTable.Rows;

        public string[] ColumnNames { get; private set; } = Array.Empty<string>();
        public Type[]? ColumnTypes { get; private set; }
        public DataTable DataTable { get; }

        public string? TableName
        {
            get => tableName;
            set
            {
                tableName = value;
                DataTable.TableName = value;
            }
        }
        private string? tableName;

        public string this[int i, int j] => DataTable.Rows[i][j].ToString() ?? string.Empty;

        public event EventHandler<DataRow>? RowAdded;

        public event EventHandler<CellChangedEventArgs>? CellChanged;


        private List<int> RowIndexes = new List<int>();

        public Table()
        {
            DataTable = new DataTable();
        }

        public Table(DataTable table)
        {
            DataTable = table;
            DataTable.ExtendedProperties.Add("Table", this);
            ColumnsSetup(table);
            for (var i = 0; i < DataTable.Rows.Count; i++)
                RowIndexes.Insert(i, i);

            DataTable.ColumnChanged += TableCellChanged;
            DataTable.RowChanged += TableRowChanged;
        }

        private void ColumnsSetup(DataTable table)
        {
            ColumnNames = new string[ColumnCount];
            ColumnTypes = new Type[ColumnCount];
            for (var j = 0; j < table.Columns.Count; j++)
            {
                ColumnTypes[j] = table.Columns[j].DataType;
                ColumnNames[j] = table.Columns[j].ColumnName;
            }
        }

        public void RemoveRow(DataRow row)
        {
            var index = DataTable.Rows.IndexOf(row);
            DataTable.Rows.RemoveAt(index);
            RowIndexes.Remove(index);
            for (int i = 0; i < RowIndexes.Count; i++)
                if (RowIndexes[i] > index)
                    RowIndexes[i]--;
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

            string[] columns = new string[ColumnCount];
            for (var i = 0; i < RowCount; i++)
            {
                for (var j = 0; j < ColumnCount; j++)
                    columns[j] = GetRowByIndex(i).ItemArray[j].ToString();
                builder.Append(string.Format(formater, columns)).Append('\n');
            }
            return builder.ToString();
        }

        public DataRow GetRowByIndex(int index)
        {
            var innerIndex = RowIndexes[index];
            return DataTable.Rows[innerIndex];
        }
        public int GetIndexByRow(DataRow row)
        {
            var index = DataTable.Rows.IndexOf(row);
            return RowIndexes.IndexOf(index);
        }
        public void UpdateIndex(int oldIndex, int newIndex)
        {
            if (oldIndex == -1)
            {
                RowIndexes.Insert(newIndex, DataTable.Rows.Count - 1);
            }
            else
            {
                var value = RowIndexes[oldIndex];
                RowIndexes.RemoveAt(oldIndex);
                RowIndexes.Insert(newIndex, value);
            }
        }
        public int GetGlobalIndexByInner(int innerIndex)
        {
            return RowIndexes.IndexOf(innerIndex);
        }

        private void TableCellChanged(object sender, DataColumnChangeEventArgs e)
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
                RowAdded?.Invoke(this, e.Row);
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
