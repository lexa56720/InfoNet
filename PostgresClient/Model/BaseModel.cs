using PsqlSharp;

namespace PostgresClient.Model
{
    internal abstract class BaseModel
    {
        protected ISqlApi Api { get; }

        public event EventHandler<bool>? NewConnectStatus;

        protected BaseModel(ISqlApi api)
        {
            Api = api;
            api.ConnectionStatusChanged += ConnectionStatusChanged;
        }

        private void ConnectionStatusChanged(object? sender, EventArgs e)
        {
            NewConnectStatus?.Invoke(this, Api.IsConnected);
        }
        public bool DataBaseNameIs(string dbName)
        {
            if (Api.ConnectionData == null)
                return false;
            return Api.ConnectionData.Database.Equals(dbName);
        }
    }
}
