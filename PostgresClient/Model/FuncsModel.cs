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
            Functions = await Api.GetAllFunctionsAsync();

            return Array.ConvertAll(Functions, x => x.ToString());
        }

        public string GetFunctionCode(int index)
        {
            if (index < 0 || index > Functions.Length)
                return string.Empty;
            return Functions[index].UserCode;
        }

        public async Task<bool> UpdateFunction(int index, string code)
        {
            Functions[index].UserCode = code;
            return await Api.UpdateFunctionAsync(Functions[index]);
        }

        public async Task DeleteFunction(int index)
        {
            await Api.RemoveFunctionAsync(Functions[index]);
        }
    }
}
