using PsqlSharp;
using System.Threading.Tasks;

namespace PostgresClient.Model
{
    internal class DumpManagerModel : BaseModel
    {
        public DumpManagerModel(ISqlApi api) : base(api)
        {
        }

        public async Task Save(string path)
        {
            await Api.ExportDataBase(path);
        }

        public async Task<bool> Load(string path)
        {
            return await Api.ImportDataBase(path);
        }
    }
}
