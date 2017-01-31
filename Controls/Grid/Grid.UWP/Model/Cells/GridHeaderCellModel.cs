using System;
using Telerik.Core;
using Telerik.Data.Core.Layouts;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal class GridHeaderCellModel : GridCellModel, IItemInfoNode
    {
        public ItemInfo ItemInfo
        {
            get;
            set;
        }

        public bool IsFrozen
        {
            get;
            set;
        }
    }
}
