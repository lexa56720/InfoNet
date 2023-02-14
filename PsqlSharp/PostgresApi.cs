using Npgsql;
using Npgsql.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsqlSharp
{
    public class PostgresApi : ISqlApi
    {


        public string UserName => throw new NotImplementedException();

        public NpgsqlDatabaseInfo DatabaseInfo => throw new NotImplementedException();

        public event EventHandler? ConnectionStatusChanged;
        public bool IsConnected
        {
            get => isConnected;
            set
            {
                isConnected = value;
                ConnectionStatusChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        private bool isConnected;
        private NpgsqlConnection? Connection { get; set; }



        public async Task<bool> ConnectAsync(string database, string username, string password, string server, string port = "5432")
        {
            if (!IsConnected)
                try
                {
                    var connectionString = $"Host={server};Username={username};Password={password};Database={database.ToLower()};Port={port}";
                    await using var dataSource = NpgsqlDataSource.Create(connectionString);
                    Connection = await dataSource.OpenConnectionAsync();
                    IsConnected = true;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    IsConnected = false;
                }

            return IsConnected;
        }

        public async Task<bool> DisconnectAsync()
        {
            if (Connection != null)
                try
                {
                    await Connection.CloseAsync();
                    IsConnected = false;
                    return true;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);

                }
            return false;
        }

        public async Task<NpgsqlDataReader?> ExecuteCommand(string command)
        {
            if (Connection != null)
                try
                {
                    await using var commandObj = new NpgsqlCommand(command, Connection);
                    await using var reader = await commandObj.ExecuteReaderAsync();
                    return reader;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            return null;
        }

        public Task<NpgsqlDataReader?> ExecuteFunction(string func, params string[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}
