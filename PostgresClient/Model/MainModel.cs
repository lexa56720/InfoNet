using PsqlSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgresClient.Model
{
    class MainModel
    {

        public MainModel(ISqlApi api) 
        {
            Api = api;
        }

        public ISqlApi Api { get; }
    }
}
