using PsqlSharp;
using System;
using System.Threading.Tasks;

namespace PostgresClient.Model
{
    internal class FuncsModel : BaseModel
    {
        private Function[] Functions { get; set; } = Array.Empty<Function>();
        public FuncsModel(ISqlApi api) : base(api)
        {
        }

        public async Task<string[]> GetFunctionsHeader()
        {
            Functions = await Api.GetAllFunctions();

            return Array.ConvertAll(Functions, x => x.ToString());
        }

        public async Task<string> GetFunctionCode(int index)
        {
            if (index < 0 || index > Functions.Length)
                return string.Empty;
            return Functions[index].UserCode;
        }

        public async Task<bool> UpdateFunction(int index, string code)
        {
            Functions[index].UserCode = code;
            return await Api.UpdateFunction(Functions[index]);
        }

        public async Task DeleteFunction(int index)
        {
            await Api.RemoveFunction(Functions[index]);
        }
    }
}
