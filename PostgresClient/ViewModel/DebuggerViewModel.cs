using PostgresClient.Model;
using PostgresClient.Utils;
using PsqlSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgresClient.ViewModel
{
    internal class DebuggerViewModel : BaseViewModel
    {
        public string DebugLog
        {
            get => debugLog;
            set
            {
                debugLog = value;
                OnPropertyChanged(nameof(DebugLog));
            }
        }
        private string debugLog;

        protected override DebuggerModel Model => (DebuggerModel)base.Model;
        public DebuggerViewModel(ClientApi api) : base(api)
        {
            Model.NewLogLine += Model_NewLogLine;
        }
        protected override BaseModel CreateModel(ISqlApi api)
        {
            return new DebuggerModel(api as ClientApi);
        }

        private void Model_NewLogLine(object? sender, string e)
        {
            DebugLog+= e;
        }
    }
}
