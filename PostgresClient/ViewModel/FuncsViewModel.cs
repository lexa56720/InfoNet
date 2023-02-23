using PostgresClient.Model;
using PsqlSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgresClient.ViewModel
{
    class FuncsViewModel : BaseViewModel
    {

        public ObservableCollection<string> FuncList { get; set; }= new ObservableCollection<string>();

        public int Selected
        {
            get => selected;
            set
            {
                if (selected != value)
                {
                    selected = value;
                    ShowFunc(Selected);
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


        protected override FuncsModel Model => (FuncsModel)base.Model;

        public FuncsViewModel(ref Action updateFuncsList, ISqlApi api) : base(api)
        {
            updateFuncsList = new Action(async () => await Update());
        }
        protected override BaseModel CreateModel(ISqlApi api)
        {
            return new FuncsModel(api);
        }
        private void ShowFunc(int index)
        {
            var code = Model.GetFunctionCode(index);
            FuncBody = code;
        }

        private async Task Update()
        {
            FuncList.Clear();
            var funcs = await Model.GetFunctions();
            if (funcs != null)
                foreach (var func in funcs)
                    FuncList.Add(func);
        }
    }
}
