using System;
using System.Linq;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal class CellGenerationContext : IGenerationContext
    {
        public CellGenerationContext(object rowItem, object columnItem, object cellValue, bool isFrozen = false)
        {
            this.RowItem = rowItem;
            this.ColumnItem = columnItem;
            this.CellValue = cellValue;
            this.IsFrozen = isFrozen;
        }

        public object RowItem { get; private set; }

        public object ColumnItem { get; private set; }

        public object CellValue { get; private set; }

        public bool IsFrozen { get; private set; }
    }
}