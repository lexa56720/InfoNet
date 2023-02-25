using PostgresClient.Utils;
using PsqlSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
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
