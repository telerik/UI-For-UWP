using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu
{
    /// <summary>
    /// Represents a context, passed to the command associated with menu opening.
    /// </summary>
    public class NavigateContext
    {
         /// <summary>
        /// Initializes a new instance of the <see cref="NavigateContext"/> class.
        /// </summary>
        /// <param name="menuItem">The desired menu item.</param>
        public NavigateContext(RadialMenuItem menuItem)
        {
            this.MenuItemTarget = menuItem;
        }

        internal NavigateContext()
        {
        }

        /// <summary>
        /// Gets the current <see cref="RadialMenuItem"/> that has triggered <see cref="Commands.NavigateCommand"/> execution.
        /// </summary>
        public RadialMenuItem MenuItemTarget
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the previous (if any) <see cref="RadialMenuItem"/> that has triggered <see cref="Commands.NavigateCommand"/> execution.
        /// </summary>
        public RadialMenuItem MenuItemSource
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the start angle that menu items will start to reorder from.
        /// </summary>
        public double StartAngle
        {
            get;
            set;
        }

        internal bool IsBackButtonPressed
        {
            get;
            set;
        }
    }
}
