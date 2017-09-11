using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Grid
{
    public enum DataGridRowDetailsMode
    {
        /// <summary>
        /// DataGrid will not display row details.
        /// </summary>
        None,

        /// <summary>
        /// Display only one row details for the grid. If secod item expand is requested the old one is collapsed.
        /// </summary>
        Single,
    }
}
