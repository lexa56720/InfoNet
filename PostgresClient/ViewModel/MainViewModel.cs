using PostgresClient.Model;
using PostgresClient.Utils;
using PostgresClient.Utils.MessageCentre;
using PostgresClient.View;
using PsqlSharp;
using System.Windows.Controls;
using System.Windows.Input;

namespace PostgresClient.ViewModel
{
    internal class MainViewModel : BaseViewModel
    {
        public Page SidePanel
        {
            get => sidePanel;
            set
            {
                sidePanel = value;
                OnPropertyChanged(nameof(SidePanel));
            }
        }
        private Page sidePanel;

        public Page Debugger
        {
            get => debugger;
            set
            {
                debugger = value;
                OnPropertyChanged(nameof(Debugger));
            }
        }
        private Page debugger;

        public Page MainFrame
        {
            get => mainFrame;
            set
            {
                mainFrame = value;
                OnPropertyChanged(nameof(MainFrame));
            }
        }
        private Page mainFrame;


        public Page WorkFrame { get; }
        public ICommand NavigateToWork => new Command((o) => NavigateTo(WorkFrame));

        public Page TableViewerFrame { get; }
        public ICommand NavigateToTable => new Command((o) => NavigateTo(TableViewerFrame));

        public Page FuncFrame { get; }
        public ICommand NavigateToFunc => new Command((o) => NavigateTo(FuncFrame));


        public ICommand About => new Command((o) => (new AboutWindow()).Show());
        public ICommand DumpManager => new Command(() => Messenger.Send("DumpManager"));
        public ICommand ConnectionManager => new Command(() => Messenger.Send("ConnectionManager"));
        public ICommand DBExplorer => new Command(() => Messenger.Send("DBExplorer"));

        protected override MainModel Model => (MainModel)base.Model;

        public MainViewModel(ISqlApi api) : base(api)
        {
            SidePanel = new SidePanel();
            WorkFrame = new WorkArea();
            TableViewerFrame = new TableViewer();
            FuncFrame = new FuncsView();
            Debugger = new View.Debugger();
            SubscribeToNavigation();
        }
        protected override BaseModel CreateModel(ISqlApi api)
        {
            return new MainModel(api);
        }
        private void SubscribeToNavigation()
        {
            Messenger.Subscribe("ShowTableWindow", () => NavigateTo(TableViewerFrame));
            Messenger.Subscribe("ShowFuncWindow", () => NavigateTo(FuncFrame));
        }

        private void NavigateTo(Page page)
        {
            MainFrame = page;
        }

    }
}
