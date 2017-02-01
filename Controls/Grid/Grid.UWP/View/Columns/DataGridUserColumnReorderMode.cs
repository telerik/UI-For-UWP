using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Defines the available modes that control the User Interface and Experience related to the column reorder operations within <see cref="RadDataGrid"/> component.
    /// </summary>
    public enum DataGridUserColumnReorderMode
    {
        /// <summary>
        /// The column reorder exposed to the user is executed in interactive mode through animations.
        /// </summary>
        Interactive,

        /// <summary>
        /// The column reorder exposed to the user is disabled.
        /// </summary>
        None
    }
}
