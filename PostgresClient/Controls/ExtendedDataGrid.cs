using System.Data;
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

        private DataGridRow EditedRow;

        private bool IsValueChanged;

        protected override void OnPreparingCellForEdit(DataGridPreparingCellForEditEventArgs e)
        {
            EditedRow = e.Row;
            var textBox = e.EditingElement as TextBox;
            textBox.Foreground = this.Foreground;
            textBox.Background = this.Background;
            CellValue = (textBox).Text;
            base.OnPreparingCellForEdit(e);
        }
        protected override void OnCellEditEnding(DataGridCellEditEndingEventArgs e)
        {
            base.OnCellEditEnding(e);
            if ((e.EditingElement as TextBox).Text != CellValue)
            {

                IsValueChanged = true;
            }
        }

        protected override void OnRowEditEnding(DataGridRowEditEndingEventArgs e)
        {
            base.OnRowEditEnding(e);

        }
        protected override void OnCanExecuteCommitEdit(CanExecuteRoutedEventArgs e)
        {
            base.OnCanExecuteCommitEdit(e);
        }

        protected override void OnExecutedCancelEdit(ExecutedRoutedEventArgs e)
        {
            base.OnExecutedCancelEdit(e);
        }

        protected override void OnExecutedCommitEdit(ExecutedRoutedEventArgs e)
        {
            base.OnExecutedCommitEdit(e); 
            if (Validation.GetErrors(EditedRow).Count > 0)
            {
                EditedRow.Background = new SolidColorBrush(Color.FromRgb(198, 40, 40));
            }
            else
                EditedRow.Background = new SolidColorBrush();
            if (IsValueChanged)
            {
               
                if (Validation.GetErrors(EditedRow).Count < 0)
                {
                    EditedRow.Background = new SolidColorBrush(Color.FromRgb(46, 125, 50));
                    IsValueChanged = false;
                }
                   
            }
          

        }
    }
}
