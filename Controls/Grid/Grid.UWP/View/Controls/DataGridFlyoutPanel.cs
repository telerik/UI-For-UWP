using System;
using System.Collections.Generic;
using Telerik.UI.Xaml.Controls;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Container panel used in <see cref="DataGridFlyout"/> to display <see cref="DataGridFlyoutHeader"/> items.
    /// </summary>
    public class DataGridFlyoutPanel : Canvas
    {
        private List<DataGridFlyoutHeader> elementsCache = new List<DataGridFlyoutHeader>();
        private RadDataGrid owner;

        private double totalRealizedHeight = 0;
        private double realizedWidth = 0;

        internal DataGridFlyout Flyout { get; set; }
        
        internal List<DataGridFlyoutHeader> Elements
        {
            get
            {
                if (this.elementsCache == null)
                {
                    this.elementsCache = new List<DataGridFlyoutHeader>();
                }

                return this.elementsCache;
            }
        }

        internal RadDataGrid Owner
        {
            get
            {
                return this.owner;
            }
            set
            {
                this.owner = value;
            }
        }

        internal void ClearItems()
        {
            this.Children.Clear();
            this.Elements.Clear();
            this.realizedWidth = 0;
            this.totalRealizedHeight = 0;
        }

        /// <summary>
        /// Provides the behavior for the Arrange pass of layout. Classes can override
        /// this method to define their own Arrange pass behavior.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this object should
        /// use to arrange itself and its children.</param>
        /// <returns>
        /// The actual size that is used after the element is arranged in layout.
        /// </returns>
        protected override Windows.Foundation.Size ArrangeOverride(Windows.Foundation.Size finalSize)
        {
            foreach (FrameworkElement child in this.Children)
            {
                var x = Canvas.GetLeft(child);
                var y = Canvas.GetTop(child);

                var rect = new Rect(new Point(x, y), new Size(finalSize.Width, child.DesiredSize.Height));

                child.Width = rect.Width;

                child.Arrange(rect);
            }

            return finalSize;
        }

        /// <summary>
        /// Provides the behavior for the Measure pass of the layout cycle. Classes
        /// can override this method to define their own Measure pass behavior.
        /// </summary>
        /// <param name="availableSize">The available size that this object can give to child
        /// objects. Infinity can be specified as a value to indicate that the object will
        /// size to whatever content is available.</param>
        /// <returns>
        /// The size that this object determines it needs during layout, based on
        /// its calculations of the allocated sizes for child objects or based on other considerations
        /// such as a fixed container size.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            double servicePanelWidthToDeduct = 0d;
            double flyoutPadding = 0d;

            if (double.IsInfinity(availableSize.Width))
            {
                if (this.Owner.GroupPanelPosition == GroupPanelPosition.Left)
                {
                    servicePanelWidthToDeduct = this.owner.ServicePanel.ActualWidth;
                }
            }
            else
            {
                this.realizedWidth = availableSize.Width;
            }

            if (this.Flyout != null)
            {
                flyoutPadding = this.Flyout.Padding.Left + this.Flyout.Padding.Right;
            }

            double availableHeight = double.IsInfinity(availableSize.Height) ? this.Owner.ActualHeight : availableSize.Height;

            while (this.Elements.Count > 0)
            {
                if (this.totalRealizedHeight > availableHeight)
                {
                    break;
                }

                var child = this.Elements[0];
                this.Elements.RemoveAt(0);
                this.Children.Add(child);
                child.Measure(new Size(availableSize.Width, double.PositiveInfinity));

                Canvas.SetLeft(child, 0);
                Canvas.SetTop(child, this.totalRealizedHeight);

                this.totalRealizedHeight += child.DesiredSize.Height;
                this.realizedWidth = Math.Min(Math.Max(this.realizedWidth, child.DesiredSize.Width), this.owner.ActualWidth - servicePanelWidthToDeduct - flyoutPadding);
            }

            var actualHeight = this.VerticalAlignment == VerticalAlignment.Stretch ? availableHeight : this.totalRealizedHeight;

            base.MeasureOverride(new Size(this.realizedWidth, actualHeight));
            return new Size(this.realizedWidth, actualHeight);
        }
    }
}