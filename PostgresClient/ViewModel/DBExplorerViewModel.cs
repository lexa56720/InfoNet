using PostgresClient.MessageCentre;
using PostgresClient.Model;
using PostgresClient.Utils;
using PsqlSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;

namespace PostgresClient.ViewModel
{
    class DBExplorerViewModel : BaseViewModel
    {
        private object aA;

        public class Node
        {
            public string Name { get; set; }
            public ObservableCollection<Node> Nodes { get; set; }

            public DataBase DataBase { get; }
            public bool IsFunc { get; }
            public int Index { get; } = -1;

            public Node(string name)
            {
                Name = name;
            }

            public Node(string name, bool isFunc, int index, DataBase dataBase)
            {
                Name = name;
                IsFunc = isFunc;
                Index = index;
                DataBase = dataBase;
            }
            public Node(DataBase database)
            {
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
                                new Node(f.Name, true, Array.IndexOf(database.Functions, f), database)
                        ))
                    });
                };
            }
        }

        public ObservableCollection<Node> DataBases { get; set; } = new ObservableCollection<Node>();

        public object AA
        {
            get => aA;
            set => aA = value;
        }
        public ICommand NodeSelected => new Command(Selected);

        public ICommand Loaded => new Command(async () => { await Update(); });

        protected override DBExplorerModel Model => (DBExplorerModel)base.Model;
        public DBExplorerViewModel(ISqlApi api) : base(api)
        {
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

        }
        private void TableSelected(string tableName, DataBase database)
        {
           Messenger.Send(new Message("ShowTable"),Tuple.Create(tableName,database.Name));
        }

        private async Task Update()
        {
            DataBases.Clear();
            var dataBases = await Model.GetDataBases();
            if (dataBases != null)
            {
                DataBases.Add(new Node("Базы данных") { Nodes = new ObservableCollection<Node>() });
                foreach (var database in dataBases)
                {
                    DataBases.First().Nodes.Add(new Node(database));
                }
            }
        }
    }
}
