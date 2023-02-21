using PostgresClient.Model;
using PostgresClient.Utils;
using PostgresClient.View;
using PsqlSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;

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
               OnPropertyChanged(nameof(IsCanShow));
            }
        }
        private string selectedTable;

        public bool IsCanShow
        {
            get => SelectedTable!=null&& IsConnected;
        }

        public override bool IsConnected
        {
            get => base.IsConnected;
            set
            {
                base.IsConnected = value;
                OnPropertyChanged(nameof(IsCanShow));
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


        public ICommand ShowTable { get => new Command(async () => await ShowTableContent(SelectedTable)); }
        public ObservableCollection<string> Tables { get; set; } = new ObservableCollection<string>();

        protected override TableViewerModel Model => (TableViewerModel)base.Model;
        public TableViewerViewModel(ISqlApi api) : base(api)
        {
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
            var table = await Model.GetTableContent(tableName);

            TableContent = table.DataTable;
        }
    }
}
