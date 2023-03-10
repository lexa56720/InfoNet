using PostgresClient.Model;
using PostgresClient.Utils;
using PostgresClient.Utils.MessageCentre;
using PsqlSharp;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Input;

namespace PostgresClient.ViewModel
{
    internal class TableViewerViewModel : BaseViewModel
    {
        public object? SelectedRow { get; set; }

        public bool IsDropDownOpened
        {
            get => isDropDownOpened;
            set
            {
                isDropDownOpened = value;
                if (isDropDownOpened)
                    UpdateTables();
            }
        }
        private bool isDropDownOpened;

        public string? SelectedTable
        {
            get => selectedTable;
            set
            {
                selectedTable = value;
                OnPropertyChanged(nameof(SelectedTable));
                if (selectedTable != null)
                    ShowTableContent(SelectedTable);
            }
        }
        private string? selectedTable;

        public bool IsEditable => !IsNotEditable;

        public bool IsNotEditable
        {
            get => isNotEditable || !IsConnected;
            set
            {
                isNotEditable = value;
                OnPropertyChanged(nameof(IsNotEditable));
                OnPropertyChanged(nameof(IsEditable));
            }
        }
        private bool isNotEditable;

        public override bool IsConnected
        {
            get => base.IsConnected;
            set
            {
                base.IsConnected = value;
                OnPropertyChanged(nameof(IsNotEditable));
                OnPropertyChanged(nameof(IsEditable));
            }
        }

        public DataTable TableContent
        {
            get => tableContent;
            set
            {
                tableContent = value;
                OnPropertyChanged(nameof(TableContent));
            }
        }
        private DataTable tableContent;

        public ObservableCollection<string> Tables
        {
            get => tables;
            set
            {
                tables = value;
                OnPropertyChanged(nameof(Tables));
            }
        }
        private ObservableCollection<string> tables = new ObservableCollection<string>();

        public ICommand DeleteRowCommand => new Command(async o => await DeleteRow());
        protected override TableViewerModel Model => (TableViewerModel)base.Model;

        public TableViewerViewModel(ISqlApi api) : base(api)
        {
            Messenger.Subscribe("UpdateTables", async () => await UpdateTables());
            Messenger.Subscribe<Tuple<string, string>>("ShowTable", async (s, m) => await ShowTableFromDB(m));
        }
        protected override BaseModel CreateModel(ISqlApi api)
        {
            return new TableViewerModel(api);
        }

        public async Task UpdateTables()
        {
            var tables = await Model.GetTables();
            Tables = new ObservableCollection<string>(tables);
        }

        private async Task ShowTableFromDB(Tuple<string, string> requestInfo)
        {
            await UpdateTables();
            if (Model.DataBaseNameIs(requestInfo.Item2))
            {
                SelectedTable = Tables.First(t => t == requestInfo.Item1);
                IsNotEditable = false;
            }
            else
            {
                SelectedTable = null;
                TableContent = (await Model.GetTable(requestInfo.Item1, requestInfo.Item2)).DataTable;
                IsNotEditable = true;
            }
        }

        private async Task DeleteRow()
        {
            if (SelectedRow is DataRowView row)
                await Model.DeleteRow(row.Row);
        }

        public async Task ShowTableContent(string? tableName)
        {
            if (tableName != null)
                TableContent = (await Model.GetSelectedTable(tableName)).DataTable;
        }
    }
}
