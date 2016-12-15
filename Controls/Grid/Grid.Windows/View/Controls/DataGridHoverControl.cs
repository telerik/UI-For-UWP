using System;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents the custom <see cref="Control"/> implementation used to visualize the hover UI within a <see cref="RadDataGrid"/> component.
    /// </summary>
    public class DataGridHoverControl : RadControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridHoverControl" /> class.
        /// </summary>
        public DataGridHoverControl()
        {
            this.DefaultStyleKey = typeof(DataGridHoverControl);

            this.IsTabStop = false;
            this.IsHitTestVisible = false;
        }
    }
}
