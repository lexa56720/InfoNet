using PostgresClient.Model;
using PostgresClient.Utils;
using PostgresClient.Utils.MessageCentre;
using PsqlSharp;
using System;
using System.IO;
using System.Threading.Tasks;
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
                Messenger.Send(new Message("UpdateDB"), this);
                Messenger.Send(new Message("UpdateFuncs"), this);
                Messenger.Send(new Message("UpdateTables"), this);
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
