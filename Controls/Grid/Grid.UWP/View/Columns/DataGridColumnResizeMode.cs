using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Defines the available modes that control the User Input and Experience related to the column resize operations within the <see cref="RadDataGrid"/> component.
    /// </summary>
    public enum DataGridColumnResizeHandleDisplayMode
    {
        ///// <summary>
        ///// The resize handle exposed to the user through the column header UI is automatically determined based on the form factor and device family dynamically.
        ///// </summary>
        //Auto,
        /// <summary>
        /// The resize exposed to the user through the column header UI is disabled.
        /// </summary>
        None,

        /// <summary>
        /// The resize exposed to the user through the column header UI is through thumb at the end of the header visible always.
        /// </summary>
        Always,

        /// <summary>
        /// The resize exposed to the user through the column header UI is through thumb at the end of the header visible when <see cref="ColumnHeaderActionMode"/> is Flyout and  column flyout is open. 
        /// </summary>
        OnColumnHeaderActionFlyoutOpen
    }
}
