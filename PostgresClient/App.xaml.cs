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
        public static ISqlApi Api { get; private set; }

        public App()
        {
            Api = new PostgresApi();
        }

    }
}
