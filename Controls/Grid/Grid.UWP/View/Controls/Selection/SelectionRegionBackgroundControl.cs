using System;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents the custom Control implementation used to visualize a selection region within a <see cref="RadDataGrid"/> component.
    /// </summary>
    public class SelectionRegionBackgroundControl : RadControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionRegionBackgroundControl" /> class.
        /// </summary>
        public SelectionRegionBackgroundControl()
        {
            this.DefaultStyleKey = typeof(SelectionRegionBackgroundControl);

            this.IsTabStop = false;
            this.IsHitTestVisible = false;
        }
    }
}
