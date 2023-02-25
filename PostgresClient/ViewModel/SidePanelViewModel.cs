﻿using PostgresClient.Utils.MessageCentre;
using PostgresClient.View;
using System.ComponentModel;
using System.Windows.Controls;

namespace PostgresClient.ViewModel
{
    internal class SidePanelViewModel : INotifyPropertyChanged
    {
        public Page Content
        {
            get => content;
            set
            {
                content = value;
                OnPropertyChanged(nameof(Content));
            }
        }
        private Page content;


        private Page DBExplorer { get; set; }

        private Page ConnectionManager { get; set; }

        private Page DumpManager { get; set; }

        private void OnPropertyChanged(string? name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        public SidePanelViewModel()
        {
            DBExplorer = new DBExplorer();
            DumpManager = new DumpManager();
            Content = ConnectionManager = new ConnectView();


            Messenger.Subscribe("DBExplorer", (o, e) => Content = DBExplorer);
            Messenger.Subscribe("ConnectionManager", (o, e) => Content = ConnectionManager);
            Messenger.Subscribe("DumpManager", (o, e) => Content = DumpManager);
        }
    }
}
