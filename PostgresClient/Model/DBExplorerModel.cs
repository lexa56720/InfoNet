namespace PostgresClient.Model
{
    internal class DBExplorerModel : BaseModel
    {

        private int Timeout = 1000;
        public DBExplorerModel(ISqlApi api) : base(api)
        {
        }

        private DataBase[] Result
        {
            get => result;
            set
            {
                result = value;
                ResultDate = DateTime.UtcNow;
            }
        }
        private DataBase[] result;

        private DateTime ResultDate { get; set; }
        public async Task<DataBase[]> GetDataBases()
        {
            await Task.Delay(Timeout);
            if (GetResultAgeInMs() > Timeout)
                return await GetDataBasesRequest();
            else
                return Result;
        }

        private double GetResultAgeInMs()
        {
            return (DateTime.UtcNow - ResultDate).TotalMilliseconds;
        }

        private async Task<DataBase[]> GetDataBasesRequest()
        {
            var dbNames = await Api.GetAllDataBaseNamesAsync();

            var tasks = new Task<DataBase?>[dbNames.Length];

            for (var i = 0; i < dbNames.Length; i++)
                tasks[i] = Api.GetDataBaseContentAsync(dbNames[i]);

            await Task.WhenAll(tasks);
            return Result = tasks.Select(t => t.Result).Where(r => r != null).ToArray();
        }
    }
}
