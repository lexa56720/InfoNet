using PsqlSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgresClient.Model
{
    class TableViewerModel
    {
        public ISqlApi Api { get; }

        public TableViewerModel(ISqlApi api)
        {
            Api = api;
        }

        public async Task<string[]> GetTables()
        {
            var tables = await Api.GetTables();
            if (tables != null)
                return tables;
            else
                return new string[] { string.Empty };
        }

        public async Task<string[]> GetTableContent(string table)
        {
            var tables = await Api.GetTableContent(table);
            var result = new string[tables.GetLength(0)];
            for (int j = 0; j < tables.GetLength(0); j++)
            {
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < tables.GetLength(1); i++)
                {
                    builder.Append(tables[j, i]);
                    if (i < tables.GetLength(1))
                        builder.Append(",\t");
                }
                result[j] = builder.ToString();
            }
            return result;
        }

    }
}
