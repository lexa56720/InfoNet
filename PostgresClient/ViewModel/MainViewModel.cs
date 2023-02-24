using PostgresClient.MessageCentre;
using PostgresClient.Model;
using PostgresClient.Utils;
using PostgresClient.View;
using PsqlSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace PostgresClient.ViewModel
{
    class MainViewModel : BaseViewModel
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

        public int CurrentTabIndex
        {
            get => currentTabIndex;
            set
            {
                currentTabIndex = value;
                OnPropertyChanged(nameof(CurrentTabIndex));
            }
        }
        private int currentTabIndex;


        public Page TableViewerFrame
        {
            get => tableViewerFrame;
            set
            {
                tableViewerFrame = value;
                OnPropertyChanged(nameof(TableViewerFrame));
            }
        }
        private Page tableViewerFrame;

        public int TableViewerTabIndex
        {
            get => tableViewerTabIndex;
            set
            {
                tableViewerTabIndex = value;
                OnPropertyChanged(nameof(TableViewerTabIndex));
            }
        }
        private int tableViewerTabIndex;


        public Page WorkFrame
        {
            get => workFrame;
            set
            {
                workFrame = value;
                OnPropertyChanged(nameof(WorkFrame));
            }
        }
        private Page workFrame;

        public int WorkTabIndex
        {
            get => workTabIndex;
            set
            {
                workTabIndex = value;
                OnPropertyChanged(nameof(WorkTabIndex));
            }
        }
        private int workTabIndex;

        public Page FuncFrame
        {
            get => funcFrame;
            set
            {
                funcFrame = value;
                OnPropertyChanged(nameof(FuncFrame));
            }
        }
        private Page funcFrame;

        public int FuncTabIndex
        {
            get => funcTabIndex;
            set
            {
                funcTabIndex = value;
                OnPropertyChanged(nameof(FuncTabIndex));
            }
        }
        private int funcTabIndex;

        public ICommand ConnectionManager => new Command(() => Messenger.Send(new Message("ConnectionManager"), this));

        public ICommand DBExplorer => new Command(() => Messenger.Send(new Message("DBExplorer"), this));
        protected override MainModel Model { get => (MainModel)base.Model; }

        public MainViewModel(ISqlApi api) : base(api)
        {
            SidePanel = new SidePanel();
            WorkFrame = new WorkArea();
            TableViewerFrame = new TableViewer();
            FuncFrame = new FuncsView();
            CurrentTabIndex = 0;
            SubscribeToNavigation();
        }
        protected override BaseModel CreateModel(ISqlApi api)
        {
            return new MainModel(api);
        }
        private void SubscribeToNavigation()
        {
            Messenger.Subscribe("ShowTable", (o, e) => CurrentTabIndex=TableViewerTabIndex);
            Messenger.Subscribe("ShowFunc", (o, e) => CurrentTabIndex = FuncTabIndex);
            Messenger.Subscribe("ShowWork", (o, e) => CurrentTabIndex = WorkTabIndex);
        }


    }
}
