using PsqlSharp;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PostgresClient.Model
{
    internal class DBExplorerModel : BaseModel
    {

        private int Timeout = 1000;
        public DBExplorerModel(ISqlApi api) : base(api)
        {
        }

        private DataBase[]? Result
        {
            get => result;
            set
            {
                result = value;
                ResultDate = DateTime.UtcNow;
            }
        }
        private DataBase[]? result;

        private DateTime ResultDate { get; set; }
        public async Task<DataBase[]?> GetDataBases()
        {
            await Task.Delay(Timeout);
            if (GetResultAgeInMs() > Timeout)
                return await GetDataBasesRequest();
            else return Result;
        }

        private double GetResultAgeInMs()
        {
            return (DateTime.UtcNow - ResultDate).TotalMilliseconds;
        }

        private async Task<DataBase[]?> GetDataBasesRequest()
        {
            if (!Api.IsConnected)
                return Result = null;

            var dbNames = await Api.GetAllDataBaseNames();

            if (dbNames == null)
                return Result = null;

            var tasks = new Task<DataBase?>[dbNames.Length];

            for (var i = 0; i < dbNames.Length; i++)
                tasks[i] = Api.GetDataBaseContent(dbNames[i]);

            await Task.WhenAll(tasks);
            var dbs= tasks.Select(t => t.Result).Where(r => r != null).ToArray();
            return Result = dbs.Count()==0 ? null : dbs;
        }
    }
}
