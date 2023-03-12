namespace PostgresClient.Model
{
    internal class ConnectModel : BaseModel
    {
        public ConnectModel(ISqlApi api) : base(api)
        {
        }
        public async Task Disconnect()
        {
            await Api.DisconnectAsync();
        }
        public async Task<bool> Connect(string database, string username, string password, string server, string port)
        {
            return await Api.ConnectAsync(new ConnectionData()
            {
                Database = database,
                Username = username,
                Password = password,
                Host = server,
                Port = port,
            });
        }
    }
}
