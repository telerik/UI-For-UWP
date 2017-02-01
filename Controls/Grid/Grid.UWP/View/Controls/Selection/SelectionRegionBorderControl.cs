using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents the custom Control implementation used to visualize a selection region within a <see cref="RadDataGrid"/> component.
    /// </summary>
    public class SelectionRegionBorderControl : RadControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionRegionBorderControl" /> class.
        /// </summary>
        public SelectionRegionBorderControl()
        {
            this.DefaultStyleKey = typeof(SelectionRegionBorderControl);

            this.IsTabStop = false;
            this.IsHitTestVisible = false;
        }
    }
}
