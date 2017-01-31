using System;
using System.Collections.Generic;
using Telerik.Data.Core.Layouts;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal class EditOperation
    {
        public EditOperation(ItemInfo editRowInfo, DataGridUserEditMode editMode)
        {
            this.EditItemInfo = editRowInfo;
            this.OriginalValues = new Dictionary<DataGridColumn, object>();
            this.EditMode = editMode;
        }

        public Dictionary<DataGridColumn,  object> OriginalValues { get; set; }

        public ItemInfo EditItemInfo { get; private set; }

        public DataGridUserEditMode EditMode { get; set; }
    }
}
