﻿using PostgresClient.Model;
using PostgresClient.Utils;
using PostgresClient.Utils.MessageCentre;
using PsqlSharp;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace PostgresClient.ViewModel
{
    internal class DBExplorerViewModel : BaseViewModel
    {

        public class Node
        {
            public string Name { get; set; }

            public SolidColorBrush Color { get; set; }
            public ObservableCollection<Node> Nodes { get; set; }

            public DataBase DataBase { get; }
            public bool IsFunc { get; }
            public int Index { get; } = -1;

            public Node(string name)
            {
                Name = name;
                Color = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
            }

            public Node(string name, bool isFunc, int index, DataBase dataBase)
            {
                Name = name;
                IsFunc = isFunc;
                Index = index;
                DataBase = dataBase;

                if (IsFunc)
                    Color = new SolidColorBrush(System.Windows.Media.Color.FromRgb(39, 174, 96));
                else
                    Color = new SolidColorBrush(System.Windows.Media.Color.FromRgb(41, 128, 185));
            }
            public Node(DataBase database)
            {
                Color = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                Name = database.Name;
                Nodes = new ObservableCollection<Node>();
                if (database.Tables != null)
                {
                    Nodes.Add(new Node("Таблицы")
                    {
                        Nodes = new ObservableCollection<Node>
                        (
                            database.Tables.Select(t =>
                                new Node(t, false, Array.IndexOf(database.Tables, t), database)
                        ))
                    });
                };
                if (database.Functions != null)
                {
                    Nodes.Add(new Node("Функции")
                    {
                        Nodes = new ObservableCollection<Node>
                        (
                            database.Functions.Select(f =>
                                new Node(f.Name + $" ({string.Join(", ", f.Arguments)})", true, Array.IndexOf(database.Functions, f), database)
                        ))
                    });
                };
            }
        }

        public ObservableCollection<Node> DataBases
        {
            get => dataBases;
            set
            {
                dataBases = value;
                OnPropertyChanged(nameof(DataBases));
            }
        }
        private ObservableCollection<Node> dataBases = new ObservableCollection<Node>();


        public ICommand NodeSelected => new Command(Selected);

        public ICommand Loaded => new Command(async () => { await Update(); });

        protected override DBExplorerModel Model => (DBExplorerModel)base.Model;
        public DBExplorerViewModel(ISqlApi api) : base(api)
        {
            Messenger.Subscribe("UpdateDB", async () => await Update());
        }
        protected override BaseModel CreateModel(ISqlApi api)
        {
            return new DBExplorerModel(api);
        }

        private void Selected(object o)
        {
            var node = (Node)o;
            if (node.Index < 0)
                return;
            if (node.IsFunc)
                FuncSelected(node.DataBase.Functions[node.Index], node.DataBase);
            else
                TableSelected(node.DataBase.Tables[node.Index], node.DataBase);

        }
        private void FuncSelected(Function function, DataBase database)
        {
            Messenger.Send("ShowFunc", Tuple.Create(function, database.Name));
            Messenger.Send("ShowFuncWindow");

        }
        private void TableSelected(string tableName, DataBase database)
        {
            Messenger.Send("ShowTable", Tuple.Create(tableName, database.Name));
            Messenger.Send("ShowTableWindow");
        }

        private async Task Update()
        {
            await App.Current.Dispatcher.InvokeAsync(async () =>
            {
                var dataBases = await Model.GetDataBases();
                var mainNode = new Node("Базы данных")
                {
                    Nodes = new ObservableCollection<Node>()
                };
                foreach (var database in dataBases)
                    mainNode.Nodes.Add(new Node(database));
                DataBases = new ObservableCollection<Node>(new Node[] { mainNode });
            });
        }
    }
}
