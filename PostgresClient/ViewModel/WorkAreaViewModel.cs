using PostgresClient.Model;
using PostgresClient.Utils;
using PsqlSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PostgresClient.ViewModel
{
    class WorkAreaViewModel : BaseViewModel
    {
    

        public string CommandText { get; set; }

        public string CommandResult 
        { 
            get => commandResult;
            set
            {
                commandResult = value;
                OnPropertyChanged(nameof(CommandResult));
            }
        }
        private string commandResult;
        public ICommand Execute { get => new Command(async () => await ExecuteCommand()); }
        protected override WorkAreaModel Model { get => (WorkAreaModel)base.Model; }
        public WorkAreaViewModel(ISqlApi api) : base(api)
        {
        }
        protected override BaseModel CreateModel(ISqlApi api)
        {
            return new WorkAreaModel(api);
        }

        private async Task ExecuteCommand()
        {
            if(string.IsNullOrEmpty(CommandText) || string.IsNullOrWhiteSpace(CommandText))
            {
                var result = await Model.ExecuteCommand(CommandText);
                if (result == null)
                    CommandResult = "Команда выполнена успешно";
                else
                    CommandResult = result;
            }
        }

    }
}
