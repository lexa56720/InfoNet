﻿using PsqlSharp;
using System.Text;
using System.Threading.Tasks;

namespace PostgresClient.Model
{
    internal class WorkAreaModel : BaseModel
    {

        public WorkAreaModel(ISqlApi api) : base(api)
        {
        }

        public async Task<string?> ExecuteCommand(string commandText)
        {
            var result = await Api.ExecuteCommand(commandText);

            var resultString = new StringBuilder();
            foreach (var table in result)
            {
                resultString.Append(table.ToString());
                resultString.Append('\n');
            }

            return resultString.ToString();
        }

    }
}
