using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Primitives.SideDrawer.Commands
{
    /// <summary>
    /// Defines the known commands that are available within a <see cref="RadSideDrawer"/> control.
    /// </summary>
    public enum CommandId
    {
        /// <summary>
        /// A command associated with the action of generating new set of animations. For example when a new DrawerTransition is applied.
        /// </summary>
        GenerateAnimations,

        /// <summary>
        /// A command associated with the action of moving the drawer(DrawerState property).
        /// </summary>
        DrawerStateChanged,

        /// <summary>
        /// A command associated with the KeyDown event processing in a <see cref="RadSideDrawer"/> instance.
        /// </summary>
        KeyDown,
    }
}
