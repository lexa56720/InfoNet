using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace PostgresClient.Utils
{
    class ExtendedDataGridValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            bool isValid = true;
            var rows = ((BindingGroup)value).Items;

            foreach (DataRowView row in rows)
            {
                var table = row.DataView.Table;
                if (row.IsNew)
                {
                    isValid = App.Api.IsCanAddRow(table, row.Row);
                }
                else if (row.IsEdit)
                {
                    isValid = App.Api.IsCanChangeRow(table, row.Row);
                }
                if (!isValid)
                    break;
            }
            // throw new NotImplementedException();
            return new ValidationResult(isValid, value);
        }
    }
}
