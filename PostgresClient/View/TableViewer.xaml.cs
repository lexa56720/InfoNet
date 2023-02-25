﻿using PostgresClient.ViewModel;
using System.Windows.Controls;

namespace PostgresClient.View
{
    /// <summary>
    /// Логика взаимодействия для TableViewer.xaml
    /// </summary>
    public partial class TableViewer : Page
    {
        public TableViewer()
        {
            InitializeComponent();
            DataContext = new TableViewerViewModel(App.Api);
        }
    }
}
