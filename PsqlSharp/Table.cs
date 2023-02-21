using Microsoft.Extensions.Primitives;
using Npgsql.Schema;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsqlSharp
{
    public class Table
    {
        public int ColumnCount => Values.GetLength(1);
        public int RowCount => Values.GetLength(0);

        public string[] ColumnNames { get; private set; }
        public Type[] ColumnTypes { get; private set; }
        public string[,] Values { get; private set; }
        public DataTable DataTable { get; }
        public string this[int i, int j]
        {
            get { return Values[i, j]; }
            set { Values[i, j] = value; }
        }
        public Table(DataTable table)
        {
            FillValues(table);

            ColumnsSetup(table);

            DataTable = table;
        }

        private void FillValues(DataTable table)
        {
            Values = new string[table.Rows.Count, table.Columns.Count];
            for (int i = 0; i < table.Rows.Count; i++)
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    Values[i, j] = table.Rows[i][j].ToString();
                    if (Values[i, j] == null)
                        Values[i, j] = string.Empty;
                }
        }

        private void ColumnsSetup(DataTable table)
        {
            ColumnNames = new string[ColumnCount];
            ColumnTypes = new Type[ColumnCount];
            for (int i = 0; i < table.Columns.Count; i++)
            {
                ColumnTypes[i] = table.Columns[i].DataType;
                ColumnNames[i] = table.Columns[i].ColumnName;
            }
        }
        public override string ToString()
        {
            var separator = "\t";
            var builder = new StringBuilder();
            for (int i = 0; i < ColumnCount; i++)
            {
                builder.Append(ColumnNames[i]);
                if (i < ColumnCount - 1)
                    builder.Append(separator);
            }
            builder.Append("\n\n");
            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColumnCount; j++)
                {
                    builder.Append(Values[i, j]);
                    if (j < ColumnCount - 1)
                        builder.Append(separator);
                }
                builder.Append('\n');
            }
            return builder.ToString();
        }
    }
}
