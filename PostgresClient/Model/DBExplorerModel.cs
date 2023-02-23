﻿using PsqlSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgresClient.Model
{
    class DBExplorerModel : BaseModel
    {
        public DBExplorerModel(ISqlApi api) : base(api)
        {
        }

        public async Task<DataBase[]?> GetDataBases()
        {
            if (Api.IsConnected)
            {
                var dbNames = await Api.GetAllDataBaseNames();

                var tasks = new Task<DataBase>[dbNames.Length];

                for (int i = 0; i < dbNames.Length; i++)
                    tasks[i] = Api.GetDataBaseContent(dbNames[i]);

                await Task.WhenAll(tasks);

                return tasks.Select(t => t.Result).ToArray();
            }
            return null;
        }
    }
}
