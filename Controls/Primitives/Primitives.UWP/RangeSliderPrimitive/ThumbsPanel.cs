using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Primitives.RangeSlider
{
    /// <summary>
    /// Represents a custom panel which contains a specific range slider components. It is used to custom measure and arrange its children controls.
    /// </summary>
    public class ThumbsPanel : Panel
    {
        private const int MiddleThumbIndex = 2;

        private RangeSliderPrimitive owner;
        private double middleThumbWidth = 0d;
        private double coeficient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThumbsPanel" /> class.
        /// </summary>
        public ThumbsPanel()
        {
        }

        internal RangeSliderPrimitive Owner
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

            var finalSize = new Size();

            this.Owner.SelectionStartThumb.Measure(availableSize);
            this.Owner.SelectionMiddleThumb.Measure(availableSize);
            this.Owner.SelectionEndThumb.Measure(availableSize);
            this.Owner.TrackBar.Measure(availableSize);

            if (this.Owner.Orientation == Orientation.Horizontal)
            {
                finalSize.Width = this.Owner.SelectionStartThumb.DesiredSize.Width * 2;
                finalSize.Width += this.Owner.SelectionEndThumb.DesiredSize.Width * 2;
                finalSize.Width += this.Owner.SelectionMiddleThumb.DesiredSize.Width;

                finalSize.Height = this.Owner.TrackBar.DesiredSize.Height;
                finalSize.Height = Math.Max(finalSize.Height, this.Owner.SelectionStartThumb.DesiredSize.Height);
                finalSize.Height = Math.Max(finalSize.Height, this.Owner.SelectionEndThumb.DesiredSize.Height);
                finalSize.Height = Math.Max(finalSize.Height, this.Owner.SelectionMiddleThumb.DesiredSize.Height);
            }
            else
            {
                finalSize.Width = this.Owner.TrackBar.DesiredSize.Width;
                finalSize.Width = Math.Max(finalSize.Width, this.Owner.SelectionStartThumb.DesiredSize.Width);
                finalSize.Width = Math.Max(finalSize.Width, this.Owner.SelectionEndThumb.DesiredSize.Width);
                finalSize.Width = Math.Max(finalSize.Width, this.Owner.SelectionMiddleThumb.DesiredSize.Width);

                finalSize.Height = this.Owner.SelectionStartThumb.DesiredSize.Height * 2;
                finalSize.Height += this.Owner.SelectionEndThumb.DesiredSize.Height * 2;
                finalSize.Height += this.Owner.SelectionMiddleThumb.DesiredSize.Height;
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
            if (!DesignMode.DesignModeEnabled)
            {
                var sliderPrimitiveTransformed = this.TransformToVisual(Window.Current.Content);
                this.Owner.sliderPrimitivePosition = sliderPrimitiveTransformed.TransformPoint(new Point(0, 0));
            }
            else
            {
                this.Owner.sliderPrimitivePosition = new Point(0, 0);
            }

            double widthOffset = 0d;
            double heightOffset = 0d;
            double selectionOffsets = this.owner.SelectionStartOffset + this.owner.SelectionEndOffset;

            if (this.owner.Orientation == Orientation.Horizontal)
            {
                this.owner.TrackBar.Arrange(new Rect(this.owner.SelectionStartOffset, 0, Math.Max(finalSize.Width - selectionOffsets, 0), finalSize.Height));

                this.coeficient = this.CalculateCoeficient(finalSize.Width);
                this.middleThumbWidth = Math.Round(this.CalculateMiddleThumbWidth(finalSize.Width));
                var selectionRange = this.owner.VisualSelection;
                var selectionStart = Math.Round((selectionRange.Start - this.owner.Minimum) * this.coeficient);

                widthOffset += selectionStart;

                ArrangeThumbHorizontally(this.owner.SelectionStartThumb, widthOffset, this.owner.SelectionStartThumb.DesiredSize.Width, finalSize);
                widthOffset += this.owner.SelectionStartOffset;

                ArrangeThumbHorizontally(this.owner.SelectionMiddleThumb, widthOffset, this.middleThumbWidth, finalSize);
                widthOffset += this.middleThumbWidth;

                this.CalculateRangeToolTipPosition(widthOffset);

                ArrangeThumbHorizontally(this.owner.SelectionEndThumb, widthOffset, this.owner.SelectionEndThumb.DesiredSize.Width, finalSize);
            }
            else
            {
                this.owner.TrackBar.Arrange(new Rect(0, this.owner.SelectionEndOffset, finalSize.Width, Math.Max(finalSize.Height - selectionOffsets, 0)));

                this.coeficient = this.CalculateCoeficient(finalSize.Height);
                this.middleThumbWidth = Math.Round(this.CalculateMiddleThumbWidth(finalSize.Height));

                var selectionRange = this.owner.VisualSelection;
                var selectionEnd = Math.Round((this.owner.Maximum - selectionRange.End) * this.coeficient);

                heightOffset += selectionEnd;

                ArrangeThumbVertically(this.owner.SelectionEndThumb, heightOffset, this.owner.SelectionEndThumb.DesiredSize.Height, finalSize);
                heightOffset += this.owner.SelectionEndOffset;

                ArrangeThumbVertically(this.owner.SelectionMiddleThumb, heightOffset, this.middleThumbWidth, finalSize);
                heightOffset += this.middleThumbWidth;

                this.CalculateRangeToolTipPosition(heightOffset);

                ArrangeThumbVertically(this.owner.SelectionStartThumb, heightOffset, this.owner.SelectionStartThumb.DesiredSize.Height, finalSize);
            }

            return finalSize;
        }

        private static void ArrangeThumbHorizontally(Thumb currentThumb, double widthOffset, double elementWidth, Size finalSize)
        {
            double heightOffset = finalSize.Height / 2 - currentThumb.DesiredSize.Height / 2;
            currentThumb.Arrange(new Rect(widthOffset, heightOffset, elementWidth, currentThumb.DesiredSize.Height));
        }

        private static void ArrangeThumbVertically(Thumb currentThumb, double heightOffset, double elementHeight, Size finalSize)
        {
            double widthOffset = finalSize.Width / 2 - currentThumb.DesiredSize.Width / 2;
            currentThumb.Arrange(new Rect(widthOffset, heightOffset, currentThumb.DesiredSize.Width, elementHeight));
        }

        private void CalculateRangeToolTipPosition(double offset)
        {
            if (this.Owner.Orientation == Orientation.Horizontal)
            {
                this.Owner.toolTipPosition.X = offset - (this.middleThumbWidth / 2.0 + this.Owner.rangeToolTipContent.DesiredSize.Width / 2.0);
                this.Owner.toolTipPosition.Y = -this.Owner.rangeToolTipContent.DesiredSize.Height;
                this.Owner.rangeToolTipContent.UpdateTooltipPosition(this.Owner.toolTipPosition);
            }
            else
            {
                this.Owner.toolTipPosition.Y = offset - (this.middleThumbWidth / 2.0 + this.Owner.rangeToolTipContent.DesiredSize.Height / 2.0);
                this.Owner.toolTipPosition.X = -this.Owner.rangeToolTipContent.DesiredSize.Width;
                this.Owner.rangeToolTipContent.UpdateTooltipPosition(this.Owner.toolTipPosition);
            }
        }

        private double CalculateMiddleThumbWidth(double width)
        {
            if (double.IsInfinity(width) || this.owner == null)
            {
                return 0;
            }

            var selectionRange = this.owner.VisualSelection;
            var selectionStart = Math.Round((selectionRange.Start - this.owner.Minimum) * this.coeficient);
            var selectionEnd = Math.Round(Math.Max(0, selectionRange.End - this.owner.Minimum) * this.coeficient);

            if (this.owner.Minimum == this.owner.Maximum)
            {
                selectionEnd = width - (this.owner.SelectionStartOffset + this.owner.SelectionEndOffset);
            }

            return Math.Max(0, selectionEnd - selectionStart);
        }

        private double CalculateCoeficient(double length)
        {
            double coef = 1;
            var range = this.owner.Maximum - this.owner.Minimum;

            if (range > 0 && !double.IsInfinity(length))
            {
                coef = (length - (this.owner.SelectionStartOffset + this.owner.SelectionEndOffset)) / range;
            }

            return coef;
        }
    }
}
