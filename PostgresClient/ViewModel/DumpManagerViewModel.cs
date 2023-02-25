﻿using PostgresClient.Model;
using PostgresClient.Utils;
using PostgresClient.Utils.MessageCentre;
using PsqlSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        private string dumpDirectory;

        public string DumpFile
        {
            get => dumpFile;
            set
            {
                dumpFile = value;
                OnPropertyChanged(nameof(DumpFile));
            }
        }
        private string dumpFile;

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
            await Model.Save(Path.Combine(DumpDirectory, DumpFile));
        }


        public async Task Load()
        {
            var path = Path.Combine(DumpDirectory, DumpFile);
            if (Path.Exists(path) && await Model.Load(path))
            {          
                Messenger.Send(new Message("UpdateDB"), this);
                Messenger.Send(new Message("UpdateFuncs"), this);
                Messenger.Send(new Message("UpdateTables"), this);
            }
        }
        private void OnFocus()
        {
            var folderPicker = new FolderPicker();
            folderPicker.InputPath = $"{AppContext.BaseDirectory}";
            if (folderPicker.ShowDialog() == true)
            {
                DumpDirectory = folderPicker.ResultPath;
            };
        }
    }
}
