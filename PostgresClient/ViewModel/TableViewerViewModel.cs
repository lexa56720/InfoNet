using PostgresClient.Model;
using PostgresClient.Utils;
using PostgresClient.Utils.MessageCentre;
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
                OnPropertyChanged(nameof(SelectedTable));
                if (selectedTable != null)
                    ShowTableContent(SelectedTable);
            }
        }
        private string selectedTable;

        public bool IsNotEditable
        {
            get => isNotEditable || !IsConnected;
            set
            {
                isNotEditable = value;
                OnPropertyChanged(nameof(IsNotEditable));
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

        protected override TableViewerModel Model => (TableViewerModel)base.Model;

        public TableViewerViewModel(ISqlApi api) : base(api)
        {
            Messenger.Subscribe("ShowTable",
               async (o, e) => await ShowTableFromDB(o as Tuple<string, string>));
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
                SelectedTable = Tables.Where(t => t == requestInfo.Item1).First();
                IsNotEditable = false;
            }
            else
            {
                SelectedTable = null;
                TableContent = (await Model.GetTable(requestInfo.Item1, requestInfo.Item2)).DataTable;
                IsNotEditable = true;
            }
        }

        public async Task ShowTableContent(string tableName)
        {
            if (tableName != null)
                TableContent = (await Model.GetSelectedTable(tableName)).DataTable;
        }
    }
}
