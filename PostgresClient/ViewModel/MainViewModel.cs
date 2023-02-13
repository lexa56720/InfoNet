using PostgresClient.Model;
using PsqlSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgresClient.ViewModel
{
    class MainViewModel
    {
        private MainModel Model { get; set; }
        public MainViewModel(ISqlApi api)
        {
            Model = new MainModel(api);
        }


    }
}
