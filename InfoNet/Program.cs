using NpgsqlTypes;
using PsqlSharp;
using System.Diagnostics;

namespace InfoNet
{
    internal class Program
    {
        static ISqlApi Api;
        static async Task Main(string[] args)
        {
            Api = new PostgresApi();
            await Api.ConnectAsync(new ConnectionData()
            {
                Database = "carsdb",
                Username = "postgres",
                Password = "1234"
            });
            var dump = await CreateDump(Api.ConnectionData,"carsdb",@"D:\7.sql");
            dump = await LoadDump(Api.ConnectionData, "carsdb", @"D:\7.sql");
            Console.ReadLine();
        }
        static async Task<bool> LoadDump(ConnectionData connection, string dbName, string inputPath)
        {
            var directory = await Api.ExecuteCommand("SELECT * FROM pg_settings WHERE name = 'data_directory'");
            var restoreExe = directory[0, 1].Replace("data", "bin/pg_restore.exe");

            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";

            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.CreateNoWindow = false;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            await cmd.StandardInput.WriteLineAsync($"set pgpassword={connection.Password}");
            await cmd.StandardInput.WriteLineAsync($"\"{restoreExe}\" --verbose --clean --no-acl --no-owner -h {connection.Host} -U {connection.Username} -d {connection.Database} {inputPath}");
            cmd.StandardInput.Close();

            await cmd.WaitForExitAsync();
            return true;
        }
        static async Task<bool> CreateDump(ConnectionData connection, string dbName,string outputPath)
        {
            var directory = await Api.ExecuteCommand("SELECT * FROM pg_settings WHERE name = 'data_directory'");
            var dumExe = directory[0, 1].Replace("data", "bin/pg_dump.exe");

            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
    
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.CreateNoWindow = false;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            await cmd.StandardInput.WriteLineAsync($"set pgpassword={connection.Password}");
            await cmd.StandardInput.WriteLineAsync($"\"{dumExe}\" -h {connection.Host} -U {connection.Username} -F tar -f {outputPath}");
            cmd.StandardInput.Close();

            await cmd.WaitForExitAsync();
            return true;
        }
    }
}