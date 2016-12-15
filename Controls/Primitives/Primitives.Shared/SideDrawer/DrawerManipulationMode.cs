using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Defines how the drawer can be opened.
    /// </summary>
    public enum DrawerManipulationMode
    {
        /// <summary>
        /// Drawer can be opened through the drawer button.
        /// </summary>
        Button,

        /// <summary>
        /// Drawer can be opened through swipe gesture.
        /// </summary>
        Gestures,

        /// <summary>
        /// Drawer can be opened through swipe gesture or through the drawer button.
        /// </summary>
        Both,
    }
}
