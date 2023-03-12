namespace PostgresClient.Model
{
    internal class DBExplorerModel : BaseModel
    {

        private int Timeout = 400;
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
            await Task.WhenAny(GetTasks());

            if (GetResultAgeInMs() > Timeout)
                return await GetDataBasesRequest();
            else
                return Result;
        }
        private Task[] GetTasks()
        {
            var tasks = new Task[2];
            tasks[0] = Task.Run(async () =>
            {
                var requestTime = DateTime.UtcNow;
                while (true)
                {
                    if (requestTime < ResultDate || ResultDate == default)
                        return;
                    await Task.Delay(50);
                }
            });
            tasks[1] = Task.Run(async () =>
            {
                await Task.Delay(Timeout);
            });
            return tasks;
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
