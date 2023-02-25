using System.ComponentModel.DataAnnotations;

namespace PsqlSharp
{
    public class Function
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string[] Arguments { get; set; }

        [Required]
        public string ReturnType { get; set; }

        [Required]
        public string Defenition { get; set; }


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
            for (int i = 0; i < funcTable.RowCount; i++)
            {
                funcs[i] = new Function()
                {
                    Name = funcTable[i, 1],
                    Arguments = funcTable[i, 4].Split(',', StringSplitOptions.RemoveEmptyEntries),
                    ReturnType = funcTable[i, 5],
                    Defenition = funcTable[i, 3],
                };
            }
            return funcs;
        }
    }
}
