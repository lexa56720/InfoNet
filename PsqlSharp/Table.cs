using Microsoft.Extensions.Primitives;
using Npgsql.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsqlSharp
{
    public class Table
    {
        public int ColumnCount => Values.GetLength(1);
        public int RowCount => Values.GetLength(0);

        public string[] ColumnNames { get; }
        public Type[] ColumnTypes { get; }
        public string[,] Values { get; }
        public string this[int i, int j]
        {
            get { return Values[i, j]; }
            set { Values[i, j] = value; }
        }

        public Table(NpgsqlDbColumn[] columns, string[,] values)
        {
            Values = values;

            ColumnNames = new string[ColumnCount];
            ColumnTypes = new Type[ColumnCount];
            for (int i = 0; i < columns.Length; i++)
            {
                ColumnTypes[i] = columns[i].DataType;
                ColumnNames[i] = columns[i].ColumnName;
            }
        }

        public override string ToString()
        {
            var separator = "\t";
            var builder = new StringBuilder();
            for (int i = 0; i < ColumnCount; i++)
            {
                builder.Append(ColumnNames[i]);
                if (i < ColumnCount-1)
                    builder.Append(separator);
            }
            builder.Append("\n\n");
            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColumnCount; j++)
                {
                    builder.Append(Values[i, j]);
                    if (j < ColumnCount-1)
                        builder.Append(separator);

                }
                builder.Append('\n');
            }
            return builder.ToString();
        }
    }
}
