using PostgresClient.Model;
using PostgresClient.Utils;
using PostgresClient.Utils.MessageCentre;
using PsqlSharp;
using System.Windows.Input;

namespace PostgresClient.ViewModel
{
    internal class WorkAreaViewModel : BaseViewModel
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
        public ICommand Execute => new Command(async () => await ExecuteCommand());

        protected override WorkAreaModel Model => (WorkAreaModel)base.Model;

        public WorkAreaViewModel(ISqlApi api) : base(api)
        {
        }
        protected override BaseModel CreateModel(ISqlApi api)
        {
            return new WorkAreaModel(api);
        }

        private async Task ExecuteCommand()
        {
            if (!string.IsNullOrEmpty(CommandText) && !string.IsNullOrWhiteSpace(CommandText))
            {
                var result = await Model.ExecuteCommand(CommandText);
                if (result != null)
                    CommandResult += new string('-', 20) + '\n' + result;
                await Messenger.Send("UpdateDB");
            }
        }

    }
}
