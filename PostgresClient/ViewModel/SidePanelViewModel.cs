using PostgresClient.Utils.MessageCentre;
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

        private Page DBExplorer { get; set; } = new DBExplorer();

        private Page ConnectionManager { get; set; } = new ConnectView();

        private Page DumpManager { get; set; } = new DumpManager();

        private void OnPropertyChanged(string? name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        public SidePanelViewModel()
        {
            Content = ConnectionManager;

            Messenger.Subscribe("DBExplorer", () => Content = DBExplorer);
            Messenger.Subscribe("ConnectionManager", () => Content = ConnectionManager);
            Messenger.Subscribe("DumpManager", () => Content = DumpManager);
        }
    }
}
