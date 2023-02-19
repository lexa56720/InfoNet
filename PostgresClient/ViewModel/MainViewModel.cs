using PostgresClient.Model;
using PostgresClient.Utils;
using PostgresClient.View;
using PsqlSharp;
using System;
using System.Collections.Generic;
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
        public ICommand DisconnectClick{ get => new Command(async () => { await Disconnect(); }); }

        public Page TableViewerFrame
        {
            get => tableViewerFrame;
            set
            {
                tableViewerFrame = value;
                OnPropertyChanged(nameof(SidePanel));
            }
        }
        private Page tableViewerFrame;

        public Page WorkFrame
        {
            get => workFrame;
            set
            {
                workFrame = value;
                OnPropertyChanged(nameof(SidePanel));
            }
        }
        private Page workFrame;

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
       

        protected override MainModel Model { get => (MainModel)base.Model; }

        public MainViewModel(ISqlApi api):base(api)
        {           
            SidePanel = new ConnectView();
            WorkFrame =new  WorkArea();
            TableViewerFrame = new TableViewer();
            FuncFrame = new FuncsView();
        }
        protected override BaseModel CreateModel(ISqlApi api)
        {
            return new MainModel(api);
        }
        private async Task Disconnect()
        {
            await Model.Disconnect();
        }  
    }
}
