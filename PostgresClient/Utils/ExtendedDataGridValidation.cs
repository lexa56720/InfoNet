﻿using System;
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
            var row = ((BindingGroup)value).Items[0] as DataRowView;

            var table = row.DataView.Table;

            if (row.IsNew)
                isValid = App.Api.IsCanAddRow(table, row.Row);
            else if (row.IsEdit)
                isValid = App.Api.IsCanChangeRow(table, row.Row);

            return new ValidationResult(isValid, value);
        }
    }
}
