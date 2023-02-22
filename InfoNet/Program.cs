using NpgsqlTypes;
using PsqlSharp;

namespace InfoNet
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            var api = new PostgresApi();
            await api.ConnectAsync("carsdb", "postgres", "1234", "localhost", "5432");
            var a = await api.GetAllTables();
            var b = await api.GetTableContent(a[0]);
            var c = await api.GetAllFunctions();
            await api.ExecuteCommand("Insert into cars(id,name) Values(1,'LOL')");
            var ac = await api.ExecuteCommand("select ctid, * from cars;");

            var add = ac.DataTable.Rows[4][0];
            var r = ((NpgsqlTid)add).ToString();
            await api.ExecuteCommand($"update cars set id=99 where ctid='{r}';");


        }
    }
}