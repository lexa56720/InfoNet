using PostgresClient.Model;
using PostgresClient.Utils;
using PostgresClient.Utils.MessageCentre;
using System.IO;
using System.Windows.Input;

namespace PostgresClient.ViewModel
{
    internal class DumpManagerViewModel : BaseViewModel
    {
        public string DumpDirectory
        {
            get => dumpDirectory;
            set
            {
                dumpDirectory = value;
                OnPropertyChanged(nameof(DumpDirectory));
            }
        }
        private string dumpDirectory = string.Empty;

        public ICommand SaveCommand => new Command(async o => await Save());
        public ICommand LoadCommand => new Command(async o => await Load());

        public ICommand GotFocus => new Command(o => OnFocus());
        protected override DumpManagerModel Model => (DumpManagerModel)base.Model;

        public DumpManagerViewModel(ISqlApi api) : base(api)
        {
        }

        protected override BaseModel CreateModel(ISqlApi api)
        {
            return new DumpManagerModel(api);
        }

        public async Task Save()
        {
            if (!string.IsNullOrEmpty(DumpDirectory))
                await Model.Save(DumpDirectory);
        }


        public async Task Load()
        {
            if (!string.IsNullOrEmpty(DumpDirectory) && Path.Exists(DumpDirectory) && await Model.Load(DumpDirectory))
            {
                await Messenger.Send("UpdateDB");
                await Messenger.Send("UpdateFuncs");
                await Messenger.Send("UpdateTables");
            }
        }
        private void OnFocus()
        {
            var folderPicker = new FolderPicker
            {
                InputPath = $"{AppContext.BaseDirectory}"
            };
            if (folderPicker.ShowDialog() == true)
                DumpDirectory = folderPicker.ResultPath;
        }
    }
}
