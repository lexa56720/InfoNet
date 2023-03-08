using PostgresClient.Utils;
using PsqlSharp;
using System.Windows;

namespace PostgresClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>          

    public partial class App : Application
    {
        public static ClientApi Api { get; private set; }
        public App()
        {
            Api = new ClientApi(new PostgresApi());
        }
    }
}
