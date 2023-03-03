using System.ComponentModel.DataAnnotations;

namespace PsqlSharp
{
    public class Function
    {
        public string Name { get; private set; }

        public string[] Arguments { get; private set; }

        public string ReturnType { get; private set; }

        public string SourceCode 
        { 
            get => sourceCode;
            private set
            {
                sourceCode = value;
                var start = SourceCode.IndexOf("Begin", StringComparison.OrdinalIgnoreCase);
                var end = SourceCode.LastIndexOf("End;", StringComparison.OrdinalIgnoreCase)+4;
                userCode = SourceCode.Substring(start,end-start);
            }
        }
        private string sourceCode;
        

        public string? UserCode 
        { 
            get => userCode;
            set  
            {
                userCode = value;
                var start = SourceCode.IndexOf("Begin", StringComparison.OrdinalIgnoreCase);
                var end = SourceCode.LastIndexOf("End;", StringComparison.OrdinalIgnoreCase)+4;
                sourceCode = SourceCode.Remove(start, end - start).Insert(start,UserCode);
            }
        }
        private string? userCode;

        public string GetHeader()
        {
            return ReturnType + " " + Name + $"({string.Join(", ", Arguments)})";
        }
        public override string ToString()
        {
            return GetHeader();
        }
        public static Function[] Parse(Table funcTable)
        {
            Function[] funcs = new Function[funcTable.RowCount];
            for (var i = 0; i < funcTable.RowCount; i++)
            {
                funcs[i] = new Function()
                {
                    Name = funcTable[i, 1],
                    Arguments = funcTable[i, 4].Split(',', StringSplitOptions.RemoveEmptyEntries),
                    ReturnType = funcTable[i, 5],
                    SourceCode = funcTable[i, 3],
                };
            }
            return funcs;
        }
    }
}
