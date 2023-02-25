using PostgresClient.Utils;
using PsqlSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgresClient.Model
{
    internal class DebuggerModel : BaseModel
    {
        public event EventHandler<string> NewLogLine;
        public DebuggerModel(ClientApi api) : base(api)
        {
            api.ExceptionOccured += Api_ExceptionOccured;
            api.SuccesExecution += Api_SuccesExecution;
        }

        private void OnNewLogLine(string message)
        {
            NewLogLine?.Invoke(this, $"\n{DateTime.Now:HH:mm:ss} :\t {message}");
        }

        private void Api_SuccesExecution(object? sender, string e)
        {
            OnNewLogLine($"Комманда {e} выполнена успешно");
        }

        private void Api_ExceptionOccured(object? sender, Exception e)
        {
            OnNewLogLine(e.Message);
        }
    }
}
