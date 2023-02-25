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

        public async Task<string[]?> GetFunctionsHeader()
        {
            Functions = await Api.GetAllFunctions();

            if (Functions != null)
                return Array.ConvertAll(Functions, x => x.ToString());
            return null;
        }

        public async Task<string> GetFunctionCode(int index)
        {
            if (Functions == null)
                await GetFunctionsHeader();

            return Functions[index].Defenition;
        }

        public async Task<bool> UpdateFunction(int index, string code)
        {
           return await Api.UpdateFunction(Functions[index], code);
        }

        public async Task DeleteFunction(int index)
        {
            await Api.RemoveFunction(Functions[index]);
        }
    }
}
