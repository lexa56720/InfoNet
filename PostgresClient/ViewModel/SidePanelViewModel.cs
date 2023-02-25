using PostgresClient.Utils.MessageCentre;
using PostgresClient.View;
using PsqlSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PostgresClient.ViewModel
{
    class SidePanelViewModel : INotifyPropertyChanged
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


        private void OnPropertyChanged(string? name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        public SidePanelViewModel(ISqlApi api)
        {
            DBExplorer = new DBExplorer();
            Content = ConnectionManager = new ConnectView();

            Messenger.Subscribe("DBExplorer", (o, e) => Content = DBExplorer);
            Messenger.Subscribe("ConnectionManager", (o, e) => Content = ConnectionManager);
        }
    }
}
