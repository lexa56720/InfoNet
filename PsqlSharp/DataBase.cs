using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsqlSharp
{
    public class DataBase
    {
        public string Name { get; }

        public string[]? Tables { get; }

        public Function[]? Functions { get; }

        public DataBase(string name, string[]? tables, Function[]? functions)
        {
            Name = name;
            Tables = tables;
            Functions = functions;
        }
    }
}
