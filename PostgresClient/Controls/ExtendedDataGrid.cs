using System.Collections;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PostgresClient.Controls
{
    internal class ExtendedDataGrid : DataGrid
    {
        private string CellValue;

        private DataGridRow EditedRow;

        private bool IsValueChanged;

        private Color ErrorColor = Color.FromRgb(198, 40, 40);
        private Color ChangedColor = Color.FromRgb(46, 125, 50);

        private IDictionary CellValueHolder;
        public ExtendedDataGrid()
        {
            CellValueHolder = GetType().BaseType.GetField("_editingCellAutomationValueHolders",
             BindingFlags.NonPublic |
             BindingFlags.Instance).GetValue(this) as IDictionary;
        }
        protected override void OnPreparingCellForEdit(DataGridPreparingCellForEditEventArgs e)
        {
            EditedRow = e.Row;

            var textBox = e.EditingElement as TextBox;
            textBox.Foreground = Foreground;
            textBox.Background = Background;

            CellValue = textBox.Text;

            base.OnPreparingCellForEdit(e);
        }
        protected override void OnCellEditEnding(DataGridCellEditEndingEventArgs e)
        {
            base.OnCellEditEnding(e);
            IsValueChanged = (e.EditingElement as TextBox).Text != CellValue;
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
            CellValueHolder.Clear();
        }

        protected override void OnExecutedCommitEdit(ExecutedRoutedEventArgs e)
        {
            base.OnExecutedCommitEdit(e);
            if (Validation.GetErrors(EditedRow).Count > 0)
                EditedRow.Background = new SolidColorBrush(ErrorColor);
            else if (IsValueChanged)
            {
                EditedRow.Background = new SolidColorBrush(ChangedColor);
                IsValueChanged = false;
            }
            else if (!(EditedRow.Background as SolidColorBrush).Color.Equals(ChangedColor))
                EditedRow.Background = new SolidColorBrush();
        }
    }
}
