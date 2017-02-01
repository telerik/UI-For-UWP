using System;
using Telerik.Core;

namespace Telerik.Charting
{
    internal abstract class AxisModelLayoutStrategy
    {
        internal AxisModel owner;

        protected int shouldFitLabelsMultiLine = 0;

        protected double maxLabelHeight;
        protected double maxLabelWidth;

        // This is a ratio that determines whether the labels on the horizontal axis are overlapping.
        // Ratios greater than one means that the labels are overlapping and that a non-overlapping layout
        // strategy needs to be chosen. The strategy is determined based on te value of the LabelLayoutMode property.
        protected int totalLabelWidthToAvailableWidth = 0;

        internal abstract AxisLastLabelVisibility DefaultLastLabelVisibility
        {
            get;
        }

        internal abstract double GetZoom();

        internal abstract void ApplyLayoutRounding();

        internal abstract void UpdateTicksVisibility(RadRect clipRect);

        internal abstract void Arrange(RadRect availableRect);

        /// <summary>
        /// Gets the currently visible axis range within the [0, 1] order.
        /// We are using decimal here for higher precision; the Double type generates ridiculous floating-point errors - e.g. 1.2 / 0.2 != 6 but rather 5.999999999999991.
        /// </summary>
        internal abstract ValueRange<decimal> GetVisibleRange(RadSize availableSize);

        internal abstract RadSize GetDesiredSize(RadSize availableSize);

        internal abstract RadThickness GetDesiredMargin(RadSize availableSize);

        protected abstract void ArrangeLabelMultiline(AxisLabelModel label, RadRect rect);

        protected abstract void ArrangeLabelNone(AxisLabelModel label, RadRect rect);
    }
}
