using PsqlSharp;
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
            if (result != null)
                return result.ToString();
            return null;
        }

    }
}
