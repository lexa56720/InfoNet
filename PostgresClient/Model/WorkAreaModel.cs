using PsqlSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgresClient.Model
{
    class WorkAreaModel : BaseModel
    {

        public WorkAreaModel(ISqlApi api) : base(api)
        {
        }

        public async Task<string?> ExecuteCommand(string commandText)
        {

            try
            {
                var result = await Api.ExecuteCommand(commandText);
                if (result != null)
                    return result.ToString();
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

    }
}
