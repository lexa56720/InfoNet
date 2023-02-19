using PsqlSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgresClient.Model
{
    class TableViewerModel:BaseModel
    {

        public TableViewerModel(ISqlApi api):base(api) 
        {
            
        }

        public async Task<string[]> GetTables()
        {
            var tables = await Api.GetAllTables();
            if (tables != null)
                return tables;
            else
                return new string[] { string.Empty };
        }

        public async Task<string> GetTableContent(string table)
        {
            return (await Api.GetTableContent(table)).ToString();
        }
    }
}
