using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            if(IsValueChanged)
            {
                e.Row.Background = new SolidColorBrush(Color.FromRgb(46, 125, 50));
                IsValueChanged= false;
            }
           
        }
        protected override void OnCanExecuteCommitEdit(CanExecuteRoutedEventArgs e)
        {
            base.OnCanExecuteCommitEdit(e);
        }

        public static class Validator
        {

            public static bool IsValid(DependencyObject parent)
            {
                // Validate all the bindings on the parent
                bool valid = true;
                LocalValueEnumerator localValues = parent.GetLocalValueEnumerator();
                while (localValues.MoveNext())
                {
                    LocalValueEntry entry = localValues.Current;
                    if (BindingOperations.IsDataBound(parent, entry.Property))
                    {
                        Binding binding = BindingOperations.GetBinding(parent, entry.Property);
                        foreach (ValidationRule rule in binding.ValidationRules)
                        {
                            ValidationResult result = rule.Validate(parent.GetValue(entry.Property), null);
                            if (!result.IsValid)
                            {
                                BindingExpression expression = BindingOperations.GetBindingExpression(parent, entry.Property);
                                System.Windows.Controls.Validation.MarkInvalid(expression, new ValidationError(rule, expression, result.ErrorContent, null));
                                valid = false;
                            }
                        }
                    }
                }

                // Validate all the bindings on the children
                for (int i = 0; i != VisualTreeHelper.GetChildrenCount(parent); ++i)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                    if (!IsValid(child)) { valid = false; }
                }

                return valid;
            }

        }
        protected override void OnExecutedCommitEdit(ExecutedRoutedEventArgs e)
        {
            base.OnExecutedCommitEdit(e);

        }

    }
}
