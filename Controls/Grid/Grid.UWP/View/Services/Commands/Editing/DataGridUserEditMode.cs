using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Defines the available modes that control the User Input and Experience related to the editing operations within
    /// the <see cref="RadDataGrid"/> component.
    /// </summary>
    public enum DataGridUserEditMode
    {
        /// <summary>
        /// DataGrid cannot be edited.
        /// </summary>
        None,

        /// <summary>
        /// Editing is displayed within edit row inside the grid.
        /// </summary>
        Inline,

        /// <summary>
        /// Editing is displayed in external flyout.
        /// </summary>
        External,
    }
}
