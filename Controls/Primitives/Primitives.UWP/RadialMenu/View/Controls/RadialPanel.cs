using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu
{
    /// <summary>
    /// Represents a custom RadialMenu panel which hosts all the visual rings along with their children controls.
    /// </summary>
    public class RadialPanel : Panel
    {
        /// <summary>
        /// Called in the measure layout pass to determine the desired size.
        /// </summary>
        /// <param name="availableSize">The available size that was given by the layout system.</param>
        protected override Windows.Foundation.Size MeasureOverride(Windows.Foundation.Size availableSize)
        {
            foreach (var child in this.Children)
            {
                child.Measure(availableSize);
            }
            return base.MeasureOverride(availableSize);
        }

        /// <summary>
        /// Provides the behavior for the Arrange pass of layout. Classes can override this method to define their own Arrange pass behavior.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        /// <returns>
        /// The actual size that is used after the element is arranged in layout.
        /// </returns>
        protected override Windows.Foundation.Size ArrangeOverride(Windows.Foundation.Size finalSize)
        {
            foreach (var child in this.Children)
            {
                if (child.DesiredSize.IsEmpty)
                {
                    child.Arrange(new Rect(0, 0, child.DesiredSize.Width, child.DesiredSize.Height));
                }
                else
                {
                    child.Arrange(new Rect(Math.Round((finalSize.Width - child.DesiredSize.Width) / 2.0), Math.Round((finalSize.Height - child.DesiredSize.Height) / 2.0), child.DesiredSize.Width, child.DesiredSize.Height));
                }
            }

            return base.ArrangeOverride(finalSize);
        }
    }
}
