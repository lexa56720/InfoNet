﻿using PostgresClient.MessageCentre;
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

        public Page WorkFrame { get;}
        public ICommand NavigateToWork => new Command((o)=>NavigateTo(WorkFrame));

        public Page TableViewerFrame { get; }
        public ICommand NavigateToTable => new Command((o) => NavigateTo(TableViewerFrame));

        public Page FuncFrame { get; }
        public ICommand NavigateToFunc => new Command((o) => NavigateTo(FuncFrame));


        public ICommand ConnectionManager => new Command(() => Messenger.Send(new Message("ConnectionManager"), this));
        public ICommand DBExplorer => new Command(() => Messenger.Send(new Message("DBExplorer"), this));

        protected override MainModel Model { get => (MainModel)base.Model; }

        public MainViewModel(ISqlApi api) : base(api)
        {
            SidePanel = new SidePanel();
            WorkFrame = new WorkArea();
            TableViewerFrame = new TableViewer();
            FuncFrame = new FuncsView();
            SubscribeToNavigation();
        }
        protected override BaseModel CreateModel(ISqlApi api)
        {
            return new MainModel(api);
        }
        private void SubscribeToNavigation()
        {
            Messenger.Subscribe("ShowTable", (o, e) => NavigateTo(TableViewerFrame));
            Messenger.Subscribe("ShowFunc", (o, e) => NavigateTo(FuncFrame));
            Messenger.Subscribe("ShowWork", (o, e) => NavigateTo(WorkFrame));
        }

        private void NavigateTo(Page page)
        {
            MainFrame = page;
        }

    }
}
