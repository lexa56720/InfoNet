using Microsoft.Extensions.Logging;
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


        private class A : ILogger
        {
            public IDisposable? BeginScope<TState>(TState state) where TState : notnull
            {
                throw new NotImplementedException();
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                throw new NotImplementedException();
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            {
                throw new NotImplementedException();
            }
        }

        public class B : ILoggerProvider
        {
            public ILogger CreateLogger(string categoryName)
            {
                return new A();
            }

            public void Dispose()
            {
       
            }
        }

        public async Task<bool> ConnectAsync(string database, string username, string password, string server, string port = "5432")
        {
            if (!IsConnected)
                try
                {
                    var connectionString = $"Host={server};Username={username};Password={password};Database={database.ToLower()};Port={port}";
                    var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
                  
                    dataSourceBuilder.UseLoggerFactory(LoggerFactory.Create(builder =>
                    {
                        builder
                        .AddDebug()
                        .SetMinimumLevel(LogLevel.Trace);
                    }));
                    await using var dataSource = dataSourceBuilder.Build();
                    Connection = await dataSource.OpenConnectionAsync();
                    IsConnected = true;
                }
                catch (Exception e)
                {
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
                catch
                {

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
                catch
                {
                }
            return null;
        }

        public Task<NpgsqlDataReader?> ExecuteFunction(string func, params string[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}
