﻿using PostgresClient.MessageCentre;
using PostgresClient.Model;
using PostgresClient.Utils;
using PsqlSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;

namespace PostgresClient.ViewModel
{
    class FuncsViewModel : BaseViewModel
    {
        public ObservableCollection<string> FuncList
        {
            get => funcList;
            set
            {
                funcList = value;
                OnPropertyChanged(nameof(FuncList));
            }
        }
        private ObservableCollection<string> funcList = new ObservableCollection<string>();

        public int Selected
        {
            get => selected;
            set
            {
                if (selected != value)
                {
                    selected = value;
                    ShowFunc(Selected);
                    OnPropertyChanged(nameof(Selected));
                }
            }
        }
        private int selected;

        public string FuncBody
        {
            get => funcBody;
            set
            {
                funcBody = value;
                OnPropertyChanged(nameof(FuncBody));
            }
        }
        private string funcBody;

        public bool IsEditable
        {
            get => isEditable && IsConnected;
            set
            {
                isEditable = value;
                OnPropertyChanged(nameof(IsEditable));
            }
        }
        private bool isEditable;


        public bool SuccssesSave
        {
            get => SuccssesSave;
            set
            {
                succssesSave = value;
                OnPropertyChanged(nameof(SuccssesSave));
            }
        }
        private bool succssesSave;


        public bool FailedSave
        {
            get => FailSave;
            set
            {
                failedSave = value;
                OnPropertyChanged(nameof(FailSave));
            }
        }
        private bool failedSave;
        public override bool IsConnected
        {
            get => base.IsConnected;
            set
            {
                base.IsConnected = value;
                OnPropertyChanged(nameof(IsEditable));
            }
        }

        public ICommand SaveCommand => new Command(async (o) => await Save());
        public ICommand ResetCommand => new Command(async (o) => await Reset());
        public ICommand RemoveCommand => new Command(async (o) => await Remove());

        public ICommand PageLoaded => new Command(async (o) => await Update());
        protected override FuncsModel Model => (FuncsModel)base.Model;

        public FuncsViewModel(ISqlApi api) : base(api)
        {
            Messenger.Subscribe("ShowFunc", (m, o) => LoadFunc(m as Tuple<Function, string>));
        }
        protected override BaseModel CreateModel(ISqlApi api)
        {
            return new FuncsModel(api);
        }
        private async Task ShowFunc(int index)
        {
            IsEditable = true;
            var code = Model.GetFunctionCode(index);
            FuncBody = await code;
        }
        private async void LoadFunc(Tuple<Function, string> loadRequest)
        {
            await Update();
            FuncBody = loadRequest.Item1.Defenition;
            if (Model.DataBaseNameIs(loadRequest.Item2))
            {
                Selected = FuncList.IndexOf(loadRequest.Item1.ToString());
                IsEditable = true;
            }
            else
            {
                Selected = -1;
                IsEditable = false;
            }
        }
        private async Task Update()
        {
            var funcs = await Model.GetFunctionsHeader();
            if (funcs != null)
                FuncList = new ObservableCollection<string>(funcs);

        }

        private async Task Save()
        {
            var result = await Model.UpdateFunction(Selected, FuncBody);
            await Update();
            Messenger.Send(new Message("UpdateDB"), this);
            if (result)
                SuccssesSave = true;
            else
                FailedSave=true;
        }

        private async Task Reset()
        {
            FuncBody = await Model.GetFunctionCode(Selected);
            await Update();
        }

        private async Task Remove()
        {
            await Model.DeleteFunction(Selected);
            await Update();
            Selected = Selected == 0 ? -1 : 0;
            Messenger.Send(new Message("UpdateDB"), this);
        }
    }
}
