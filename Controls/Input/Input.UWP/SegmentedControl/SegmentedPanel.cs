using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Input
{
    /// <summary>
    /// A custom panel that holds the elements in a <see cref="SegmentedItemsControl"/> control.
    /// </summary>
    public class SegmentedPanel : Panel
    {
        /// <summary>
        /// Gets the parent <see cref="SegmentedItemsControl"/>.
        /// </summary>
        public SegmentedItemsControl Owner { get; internal set; }

        internal void UpdateSegmentsCornerRadius()
        {
            if (this.Children.Count == 0)
            {
                return;
            }

            if (this.Children.Count == 1)
            {
                var segment = this.Children[0] as Segment;
                this.UpdateEndSegments(segment, segment);
            }
            else if (this.Children.Count > 1)
            {
                this.UpdateEndSegments(this.Children.First() as Segment, this.Children.Last() as Segment);
            }
        }

        /// <inheritdoc/>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.Children.Count == 0)
            {
                return finalSize;
            }

            double currentX = 0;

            var separatorWidth = this.GetSeparatorWidth();
            var separatorsTotalWidth = (this.Children.Count - 1) * separatorWidth;

            var itemsWidthMode = this.Owner != null ? this.Owner.SegmentWidthMode : SegmentWidthMode.Equal;
            var itemWidth = double.PositiveInfinity;

            if (itemsWidthMode == SegmentWidthMode.Equal)
            {
                itemWidth = (finalSize.Width - separatorsTotalWidth) / this.Children.Count;
            }

            var topOffset = this.Owner != null ? this.Owner.BorderThickness.Top : 0;
            var leftOffset = this.Owner != null ? this.Owner.BorderThickness.Left : 0;

            var decoratorsContext = new List<Point>();

            for (var i = 0; i < this.Children.Count; i++)
            {
                var segment = this.Children[i] as Segment;

                var width = itemsWidthMode == SegmentWidthMode.Auto ? segment.DesiredSize.Width : itemWidth;

                var layoutSlot = new Rect(currentX, 0, width, finalSize.Height);
                segment.Arrange(layoutSlot);

                segment.LayoutSlot = new Rect(layoutSlot.X + leftOffset, layoutSlot.Y + topOffset, layoutSlot.Width, layoutSlot.Height);

                currentX += width;

                decoratorsContext.Add(new Point(currentX, 0));
                currentX += separatorWidth;
            }

            if (this.Owner != null && decoratorsContext.Count >= 2)
            {
                decoratorsContext.RemoveAt(decoratorsContext.Count - 1);
                this.Owner.ArrangeSeparators(decoratorsContext, finalSize);
            }

            return finalSize;
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.Children.Count == 0)
            {
                return base.MeasureOverride(availableSize);
            }

            this.UpdateSegmentsCornerRadius();

            double accumulatedWidth = 0;
            double accumulatedHeight = 0;

            var separatorsTotalWidth = (this.Children.Count - 1) * this.GetSeparatorWidth();

            var itemsWidthMode = this.Owner != null ? this.Owner.SegmentWidthMode : SegmentWidthMode.Equal;
            var itemWidth = double.PositiveInfinity;

            if (itemsWidthMode == SegmentWidthMode.Equal)
            {
                var availableWidth = double.IsInfinity(availableSize.Width) ? this.DesiredSize.Width : availableSize.Width;
                itemWidth = Math.Max((availableWidth - separatorsTotalWidth) / this.Children.Count, 0);
            }

            foreach (var child in this.Children)
            {
                child.Measure(new Size(itemWidth, double.PositiveInfinity));
                accumulatedWidth += itemsWidthMode == SegmentWidthMode.Equal ? itemWidth : child.DesiredSize.Width;
                accumulatedHeight = Math.Max(accumulatedHeight, child.DesiredSize.Height);
            }

            var size = new Size(accumulatedWidth + separatorsTotalWidth, accumulatedHeight);

            return size;
        }

        /// <summary>
        /// Updates the look of the end segments, e.g. <see cref="Segment.CornerRadius"/>.
        /// </summary>
        /// <param name="first">The first segment.</param>
        /// <param name="last">The last segment.</param>
        protected virtual void UpdateEndSegments(Segment first, Segment last)
        {
            if (this.Owner != null)
            {
                var tl = Math.Max(0, this.Owner.CornerRadius.TopLeft - Math.Max(this.Owner.BorderThickness.Left, this.Owner.BorderThickness.Top) / 2);
                var tr = Math.Max(0, this.Owner.CornerRadius.TopRight - Math.Max(this.Owner.BorderThickness.Right, this.Owner.BorderThickness.Top) / 2);
                var bl = Math.Max(0, this.Owner.CornerRadius.BottomLeft - Math.Max(this.Owner.BorderThickness.Left, this.Owner.BorderThickness.Bottom) / 2);
                var br = Math.Max(0, this.Owner.CornerRadius.BottomRight - Math.Max(this.Owner.BorderThickness.Right, this.Owner.BorderThickness.Bottom) / 2);

                if (first.Equals(last))
                {
                    first.CornerRadius = new CornerRadius(tl, tr, br, bl);
                }
                else
                {
                    first.CornerRadius = new CornerRadius(tl, 0, 0, bl);
                    last.CornerRadius = new CornerRadius(0, tr, br, 0);
                }
            }
        }

        private double GetSeparatorWidth()
        {
            if (this.Owner != null && !double.IsNaN(this.Owner.SeparatorWidth))
            {
                return this.Owner.SeparatorWidth;
            }

            return 0d;
        }
    }
}
