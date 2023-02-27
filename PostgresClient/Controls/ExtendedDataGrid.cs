using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace PostgresClient.Controls
{
    internal class ExtendedDataGrid : DataGrid
    {
        private string CellValue;

        private bool IsValueChanged;
        protected override void OnPreparingCellForEdit(DataGridPreparingCellForEditEventArgs e)
        {
            CellValue = (e.EditingElement as TextBox).Text;
            base.OnPreparingCellForEdit(e);
        }
        protected override void OnCellEditEnding(DataGridCellEditEndingEventArgs e)
        {
            base.OnCellEditEnding(e);
            if ((e.EditingElement as TextBox).Text != CellValue)
            {
                IsValueChanged = true;
                e.Row.Background = new SolidColorBrush(Color.FromRgb(198, 40, 40));
            }
        }

        protected override void OnRowEditEnding(DataGridRowEditEndingEventArgs e)
        {
            base.OnRowEditEnding(e);
            if (IsValueChanged)
            {
                e.Row.Background = new SolidColorBrush(Color.FromRgb(46, 125, 50));
                IsValueChanged = false;
            }

        }
        protected override void OnCanExecuteCommitEdit(CanExecuteRoutedEventArgs e)
        {
            base.OnCanExecuteCommitEdit(e);
        }

        protected override void OnExecutedCommitEdit(ExecutedRoutedEventArgs e)
        {
            base.OnExecutedCommitEdit(e);

        }

    }
}
