using System;
using Telerik.UI.Automation.Peers;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    internal class DataGridContentLayerPanel : Panel
    {
        internal RadDataGrid Owner;

        /// <summary>
        /// Provides the behavior for the Measure pass of the layout cycle. Classes can override this method to define their own Measure pass behavior.
        /// </summary>
        /// <param name="availableSize">The available size that this object can give to child objects. Infinity can be specified as a value to indicate that the object will size to whatever content is available.</param>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of the allocated sizes for child objects or based on other considerations such as a fixed container size.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            var parent = this.Parent as DataGridCellsPanel;

            if (parent != null)
            {
                parent.OnContentLayerPanelMeasure();
            }
            else if (this.Owner != null)
            {
                this.Owner.CellsPanel.OnContentLayerPanelMeasure();
            }

            return new Size(0, 0);
        }

        /// <summary>
        /// Provides the behavior for the Arrange pass of layout. Classes can override this method to define their own Arrange pass behavior.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        /// <returns>
        /// The actual size that is used after the element is arranged in layout.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            var parent = this.Parent as DataGridCellsPanel;

            if (parent != null)
            {
                parent.OnContentLayerPanelArrange();
            }
            else if (this.Owner != null)
            {
                this.Owner.CellsPanel.OnContentLayerPanelArrange();
            }

            return finalSize;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new DataGridContentLayerPanelAutomationPeer(this);
        }
    }
}
