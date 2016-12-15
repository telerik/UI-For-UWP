using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Input.RangeSlider
{
    /// <summary>
    /// Represents a custom panel which hosts the RadRangeSlider controls (ScalePrimitive/RangeSliderPrimitive) and arranges them in the desired layout.
    /// </summary>
    public class RangeSliderPanel : Panel
    {
        private RadRangeSlider owner;

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeSliderPanel"/> class.
        /// </summary>
        public RangeSliderPanel()
        {
        }

        internal RadRangeSlider Owner
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

        /// <summary>
        /// Called in the measure layout pass to determine the desired size.
        /// </summary>
        /// <param name="availableSize">The available size that was given by the layout system.</param>
        protected override Size MeasureOverride(Size availableSize)
        {
            base.MeasureOverride(availableSize);
            Size finalSize = new Size();

            this.Owner.ScaleTopLeft.Measure(availableSize);
            this.Owner.RangeSlider.Measure(availableSize);
            this.Owner.ScaleBottomRight.Measure(availableSize);

            if (this.Owner.Orientation == Orientation.Horizontal)
            {
                finalSize.Height = this.Owner.ScaleTopLeft.DesiredSize.Height;
                finalSize.Height += this.Owner.ScaleBottomRight.DesiredSize.Height;
                finalSize.Height += this.Owner.RangeSlider.DesiredSize.Height;

                double maxWidth = Math.Max(this.Owner.ScaleTopLeft.DesiredSize.Width, this.Owner.RangeSlider.DesiredSize.Width);
                maxWidth = Math.Max(maxWidth, this.Owner.ScaleBottomRight.DesiredSize.Width);
                finalSize.Width = maxWidth;
            }
            else
            {
                finalSize.Width = this.Owner.ScaleTopLeft.DesiredSize.Width;
                finalSize.Width += this.Owner.ScaleBottomRight.DesiredSize.Width;
                finalSize.Width += this.Owner.RangeSlider.DesiredSize.Width;

                double maxHeight = Math.Max(this.Owner.ScaleTopLeft.DesiredSize.Height, this.Owner.RangeSlider.DesiredSize.Height);
                maxHeight = Math.Max(maxHeight, this.Owner.ScaleBottomRight.DesiredSize.Height);
                finalSize.Height = maxHeight;

                if (finalSize.Height > availableSize.Height)
                {
                    finalSize.Height = availableSize.Height;
                }
            }

            return finalSize;
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
            Size size = base.ArrangeOverride(finalSize);

            if (this.owner.Orientation == Orientation.Horizontal)
            {
                this.ArrangeHorizontal(finalSize);
            }
            else
            {
                this.ArrangeVertical(finalSize);
            }

            return size;
        }

        private void ArrangeHorizontal(Size finalSize)
        {
            double leftOffset = Math.Max(Math.Max(this.Owner.RangeSlider.SelectionStartOffset, this.Owner.ScaleTopLeft.AxisLineOffset.Left), this.Owner.ScaleBottomRight.AxisLineOffset.Left);

            double rightOffset = Math.Max(Math.Max(this.Owner.RangeSlider.SelectionEndOffset, this.Owner.ScaleTopLeft.AxisLineOffset.Right), this.Owner.ScaleBottomRight.AxisLineOffset.Right);
            double top = 0;
            double scaleTickMargin = 1;

            double left = leftOffset - this.Owner.ScaleTopLeft.AxisLineOffset.Left;
            double scaleRightMargin = rightOffset - this.Owner.ScaleTopLeft.AxisLineOffset.Right;
            double width = Math.Max(finalSize.Width - left - scaleRightMargin, 0);
            double height = Math.Min(this.Owner.ScaleTopLeft.DesiredSize.Height, finalSize.Height);

            this.Owner.ScaleTopLeft.Arrange(new Rect(left, top - scaleTickMargin, width, height));
            top = height;

            left = 0;
            width = finalSize.Width;

            if (this.owner.RangeSlider.SelectionStartOffset < leftOffset)
            {
                left = leftOffset - this.owner.RangeSlider.SelectionStartOffset;
                width -= left;
            }

            if (this.owner.RangeSlider.SelectionEndOffset < rightOffset)
            {
                width -= rightOffset - this.owner.RangeSlider.SelectionEndOffset;
            }

            height = Math.Min(this.Owner.RangeSlider.DesiredSize.Height, finalSize.Height - top);

            this.Owner.RangeSlider.Arrange(new Rect(left, top, width, height));

            top += height;

            left = leftOffset - this.Owner.ScaleBottomRight.AxisLineOffset.Left;
            scaleRightMargin = rightOffset - this.Owner.ScaleBottomRight.AxisLineOffset.Right;
            width = Math.Max(finalSize.Width - left - scaleRightMargin, 0);
            height = Math.Min(this.Owner.ScaleBottomRight.DesiredSize.Height, finalSize.Height - top);
            this.Owner.ScaleBottomRight.Arrange(new Rect(left, top + scaleTickMargin, width, height));
        }

        private void ArrangeVertical(Size finalSize)
        {
            double topOffset = Math.Max(Math.Max(this.Owner.RangeSlider.SelectionEndOffset, this.Owner.ScaleTopLeft.AxisLineOffset.Right), this.Owner.ScaleBottomRight.AxisLineOffset.Right);

            double bottomOffset = Math.Max(Math.Max(this.Owner.RangeSlider.SelectionStartOffset, this.Owner.ScaleTopLeft.AxisLineOffset.Left), this.Owner.ScaleBottomRight.AxisLineOffset.Left);

            double leftOffset = 0;
            double scaleTickMargin = 1;

            double top = topOffset - this.Owner.ScaleTopLeft.AxisLineOffset.Top;
            double scaleBottomMargin = bottomOffset - this.Owner.ScaleTopLeft.AxisLineOffset.Bottom;
            double height = Math.Max(finalSize.Height - top - scaleBottomMargin, 0);
            double width = Math.Min(this.Owner.ScaleTopLeft.DesiredSize.Width, finalSize.Width);
            this.Owner.ScaleTopLeft.Arrange(new Rect(leftOffset - scaleTickMargin, top, width, height));

            leftOffset += width;

            top = 0;
            height = finalSize.Height;

            if (this.owner.RangeSlider.SelectionStartOffset < bottomOffset)
            {
                top = bottomOffset - this.owner.RangeSlider.SelectionStartOffset;
                height -= top;
            }

            if (this.owner.RangeSlider.SelectionEndOffset < topOffset)
            {
                height -= topOffset - this.owner.RangeSlider.SelectionEndOffset;
            }

            width = Math.Min(this.Owner.RangeSlider.DesiredSize.Width, finalSize.Width);
            this.Owner.RangeSlider.Arrange(new Rect(leftOffset, top, width, height));

            leftOffset += width;

            top = topOffset - this.Owner.ScaleBottomRight.AxisLineOffset.Top;
            scaleBottomMargin = bottomOffset - this.Owner.ScaleBottomRight.AxisLineOffset.Bottom;
            height = Math.Max(finalSize.Height - top - scaleBottomMargin, 0);
            width = Math.Min(this.Owner.ScaleBottomRight.DesiredSize.Width, finalSize.Width);
            this.Owner.ScaleBottomRight.Arrange(new Rect(leftOffset + scaleTickMargin, top, width, height));
        }
    }
}
