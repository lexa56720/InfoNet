using PostgresClient.Model;
using PostgresClient.Utils;
using PostgresClient.View;
using PsqlSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PostgresClient.ViewModel
{
    class TableViewerViewModel : BaseViewModel
    {
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

        public string SelectedTable
        {
            get => selectedTable;
            set
            {
                selectedTable = value;
            }
        }
        private string selectedTable;

        public string TableContent
        {
            get => tableContent;
            set
            {
                tableContent = value;
                OnPropertyChanged(nameof(TableContent));
            }
        }
        private string tableContent;

        public ICommand ShowTable { get => new Command(async () => await ShowTableContent(SelectedTable)); }
        public ObservableCollection<string> Tables { get; set; }

        protected override TableViewerModel Model { get => (TableViewerModel)base.Model; }
        public TableViewerViewModel(ISqlApi api):base(api)
        {
            Tables = new ObservableCollection<string>(new string[] { "123", "432432", "4234234" });
        }
        protected override BaseModel CreateModel(ISqlApi api)
        {
            return new TableViewerModel(api);
        }
        public async Task UpdateTables()
        {
            Tables.Clear();
            var tables = await Model.GetTables();
            foreach (var table in tables)
                Tables.Add(table);

        }

        public async Task ShowTableContent(string tableName)
        {
            TableContent = await Model.GetTableContent(tableName);
        }


    }
}
