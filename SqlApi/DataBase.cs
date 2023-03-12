namespace SqlApi
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
