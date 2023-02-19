using PsqlSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgresClient.Model
{
    class FuncsModel : BaseModel
    {
        private Function[]? Functions { get; set; }
        public FuncsModel(ISqlApi api) : base(api)
        {
        }

        public async Task<string[]?> GetFunctions()
        {
            Functions = await Api.GetAllFunctions();

            if (Functions != null)
                return Array.ConvertAll(Functions, x => x.ToString());
            return null;
        }

        public string GetFunctionCode(int index)
        {
            return string.Join("", Functions[index].Defenition);
        }
    }
}
