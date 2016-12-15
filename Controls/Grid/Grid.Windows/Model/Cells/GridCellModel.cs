using System;
using Telerik.Core;
using Telerik.Data.Core.Layouts;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal class GridCellModel : GridNode, IGridNode
    {
        internal static readonly int ValuePropertyKey = PropertyKeys.Register(typeof(GridCellModel), "Value");

        /// <summary>
        /// Gets or sets the content associated with the node.
        /// </summary>
        public object Value
        {
            get
            {
                return this.GetValue(ValuePropertyKey);
            }
            set
            {
                this.DesiredSize = RadSize.Invalid;
                this.SetValue(ValuePropertyKey, value);
            }
        }

        public GridRowModel ParentRow
        {
            get
            {
                return this.parent as GridRowModel;
            }
        }

        public DataGridColumn Column
        {
            get;
            set;
        }

        public object DecorationContainer
        {
            get;
            set;
        }
    }
}
