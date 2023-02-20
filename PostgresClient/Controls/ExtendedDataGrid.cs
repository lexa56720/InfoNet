using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PostgresClient.Controls
{
    internal class ExtendedDataGrid:DataGrid
    {

        public static readonly DependencyProperty ArrayContentProperty =
    DependencyProperty.Register(
        nameof(ArrayContent),
        typeof(string[,]),
        typeof(DataGrid),
        new FrameworkPropertyMetadata(
            null,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            OnArrayContentPropertyChanged,
            CoerceArrayContentProperty,
            true,
            UpdateSourceTrigger.LostFocus));

        private static object CoerceArrayContentProperty(DependencyObject d, object value)
        {
            return value ?? null;
        }

        private static void OnArrayContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ExtendedDataGrid).ArrayContentChanged((string[,])e.NewValue);
        }

        private void ArrayContentChanged(string[,] values)
        {
            Columns.Clear();
            if(values != null)
            {
                for (int i = 0; i < values.GetLength(1); i++)
                {
                    var column = new DataGridTextColumn();
                    column.Header = values[0, i];
                    column.Binding = new Binding(string.Format("[{0}]", i));
                    Columns.Add(column);
                }
                this.ItemsSource = ToJaggedArray(values);
                for (int i = 0; i < typeof(string[][]).GetProperties().Count(); i++)
                    Columns.Remove(Columns.Last());
            }

        }

        public string[,] ArrayContent
        {
            get
            {
                return (string[,])GetValue(ArrayContentProperty);
            }
            set
            {
                SetValue(ArrayContentProperty, value);
            }
        }
        internal static T[][] ToJaggedArray<T>(T[,] twoDimensionalArray)
        {
            int rowsFirstIndex = twoDimensionalArray.GetLowerBound(0)+1;
            int rowsLastIndex = twoDimensionalArray.GetUpperBound(0);
            int numberOfRows = rowsLastIndex - rowsFirstIndex + 1;

            int columnsFirstIndex = twoDimensionalArray.GetLowerBound(1);
            int columnsLastIndex = twoDimensionalArray.GetUpperBound(1);
            int numberOfColumns = columnsLastIndex - columnsFirstIndex + 1;

            T[][] jaggedArray = new T[numberOfRows][];
            for (int i = 0; i < numberOfRows; i++)
            {
                jaggedArray[i] = new T[numberOfColumns];

                for (int j = 0; j < numberOfColumns; j++)
                {
                    jaggedArray[i][j] = twoDimensionalArray[i + rowsFirstIndex, j + columnsFirstIndex];
                }
            }
            return jaggedArray;
        }
    }
}
