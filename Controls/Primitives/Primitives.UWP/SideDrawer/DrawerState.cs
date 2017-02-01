using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Defines the possible states of the drawer.
    /// </summary>
    public enum DrawerState
    {
        /// <summary>
        /// In this state the drawer is fully visible.
        /// </summary>
        Opened,

        /// <summary>
        /// In this state the drawer is not visible.
        /// </summary>
        Closed,

        /// <summary>
        /// In this state the drawer is partially visible.
        /// </summary>
        Moving,
    }
}
