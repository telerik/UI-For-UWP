using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu
{
    /// <summary>
    /// Arguments used to raise Selection changed event of RadRadialMenuItem.
    /// </summary>
    public class MenuSelectionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the item that has been changed.
        /// </summary>
        /// <value>The item changed.</value>
        public RadialMenuItem Item { get; internal set; }
    }
}
