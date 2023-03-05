using PostgresClient.Controls;
using PostgresClient.Model;
using PostgresClient.Utils;
using PostgresClient.Utils.MessageCentre;
using PsqlSharp;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace PostgresClient.ViewModel
{
    internal class FuncsViewModel : BaseViewModel
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
        private int selected = -1;

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
            get => succssesSave;
            set
            {
                succssesSave = value;
                OnPropertyChanged(nameof(SuccssesSave));
            }
        }
        private bool succssesSave;

        public bool FailedSave
        {
            get => failedSave;
            set
            {
                failedSave = value;
                OnPropertyChanged(nameof(FailedSave));
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
        public ICommand RemoveCommand => new Command((o) => Remove());

        public ICommand PageLoaded => new Command(async (o) => await Update());
        protected override FuncsModel Model => (FuncsModel)base.Model;

        public FuncsViewModel(ISqlApi api) : base(api)
        {
            Messenger.Subscribe("ShowFunc", (m, o) => LoadFunc(m as Tuple<Function, string>));
            Messenger.Subscribe("UpdateFuncs", async (m, o) => await Update());
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
            FuncBody = loadRequest.Item1.SourceCode;
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
            FuncList = new ObservableCollection<string>(funcs);
        }

        private async Task Save()
        {
            var result = await Model.UpdateFunction(Selected, FuncBody);

            if (result)
            {
                Messenger.Send(new Message("UpdateDB"), this);
                await Update();
                SuccssesSave = true;
            }

            else
                FailedSave = true;
        }

        private async Task Reset()
        {
            if (Selected != -1)
            {
                await Update();
                FuncBody = await Model.GetFunctionCode(Selected);
            }
                
        }

        private async Task Remove()
        {
            if(Selected!=-1)
                await Task.Run(async () =>
                {
                    await Model.DeleteFunction(Selected);
                    await Update();
                    Selected = -1;
                    Messenger.Send(new Message("UpdateDB"), this);
                });
        }

    }
}
