using PsqlSharp;

namespace InfoNet
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            var api=new PostgresApi();
            await api.ConnectAsync("carsdb", "postgres", "1234", "localhost", "5432");
            var a=await api.GetAllTables();
            var b = await api.GetTableContent(a[0]);
            var c=await api.GetAllFunctions();
            await api.ExecuteCommand("Insert into cars(id,name) Values(1,'LOL')");
            await api.ExecuteCommand("Select * from cars");
        }
    }
}