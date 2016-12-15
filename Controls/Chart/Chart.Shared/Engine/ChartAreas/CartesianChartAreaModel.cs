using System;
using System.Collections.Generic;
using System.Diagnostics;
using Telerik.Core;

namespace Telerik.Charting
{
    internal class CartesianChartAreaModel : ChartAreaModelWithAxes
    {
        internal const string NoHorizontalAxisKey = "NoHorizontalAxis";
        internal const string NoVerticalAxisKey = "NoVerticalAxis";

        // TODO: Remove this if it is not used.

        /// <summary>
        /// Gets the <see cref="CartesianChartGridModel"/> instance that decorates the background of this plot area.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public CartesianChartGridModel Grid
        {
            get
            {
                return this.grid as CartesianChartGridModel;
            }
        }

        internal override Tuple<object, object> ConvertPointToData(RadPoint coordinates)
        {
            return this.ConvertPointToData(coordinates, this.primaryFirstAxis, this.primarySecondAxis);
        }

        internal Tuple<object, object> ConvertPointToData(RadPoint coordinates, AxisModel firstAxis, AxisModel secondAxis)
        {
            object firstValue = null;
            object secondValue = null;

            if (this.view != null)
            {
                RadRect plotArea = this.plotArea.layoutSlot;

                double panOffsetX = this.view.PlotOriginX * plotArea.Width;
                double panOffsetY = this.view.PlotOriginY * plotArea.Height;

                RadRect plotAreaVirtualSize = new RadRect(plotArea.X, plotArea.Y, plotArea.Width * this.view.ZoomWidth, plotArea.Height * this.view.ZoomHeight);

                if (firstAxis != null && this.FirstAxes.Contains(firstAxis) && firstAxis.isUpdated)
                {
                    firstValue = firstAxis.ConvertPhysicalUnitsToData(coordinates.X - panOffsetX, plotAreaVirtualSize);
                }

                if (secondAxis != null && this.SecondAxes.Contains(secondAxis) && secondAxis.isUpdated)
                {
                    secondValue = secondAxis.ConvertPhysicalUnitsToData(coordinates.Y - panOffsetY, plotAreaVirtualSize);
                }
            }

            return new Tuple<object, object>(firstValue, secondValue);
        }

        internal override RadPoint ConvertDataToPoint(Tuple<object, object> data)
        {
            return this.ConvertDataToPoint(data, this.primaryFirstAxis, this.primarySecondAxis);
        }

        internal RadPoint ConvertDataToPoint(Tuple<object, object> data, AxisModel firstAxis, AxisModel secondAxis)
        {
            RadPoint coordinates = new RadPoint(double.NaN, double.NaN);
            if (this.view != null)
            {
                RadRect plotArea = this.plotArea.layoutSlot;
                RadRect plotAreaVirtualSize = new RadRect(plotArea.X, plotArea.Y, plotArea.Width * this.view.ZoomWidth, plotArea.Height * this.view.ZoomHeight);

                if (firstAxis != null && this.FirstAxes.Contains(firstAxis) && firstAxis.isUpdated)
                {
                    AxisPlotInfo plotInfo = firstAxis.CreatePlotInfo(data.Item1);
                    if (plotInfo != null)
                    {
                        coordinates.X = plotInfo.CenterX(plotAreaVirtualSize);
                    }
                }

                if (secondAxis != null && this.SecondAxes.Contains(secondAxis) && secondAxis.isUpdated)
                {
                    AxisPlotInfo plotInfo = secondAxis.CreatePlotInfo(data.Item2);
                    if (plotInfo != null)
                    {
                        coordinates.Y = plotInfo.CenterY(plotAreaVirtualSize);
                    }
                }
            }

            return coordinates;
        }

        internal override void ApplyLayoutRounding()
        {
            foreach (var axis in this.FirstAxes)
            {
                axis.ApplyLayoutRounding();
            }
            foreach (var axis in this.SecondAxes)
            {
                axis.ApplyLayoutRounding();
            }

            foreach (var seriesCombineStrategy in this.SeriesCombineStrategies.EnumerateValues())
            {
                if (seriesCombineStrategy.HasCombination)
                {
                    seriesCombineStrategy.ApplyLayoutRounding(this);
                }
                else
                {
                    // ask each series to apply layout rounding
                    foreach (ChartSeriesModel series in seriesCombineStrategy.NonCombinedSeries)
                    {
                        series.ApplyLayoutRounding();
                    }
                }
            }
        }

        internal override IEnumerable<string> GetNotLoadedReasons()
        {
            if (this.FirstAxes.Count == 0)
            {
                yield return NoHorizontalAxisKey;
            }

            if (this.SecondAxes.Count == 0)
            {
                yield return NoVerticalAxisKey;
            }
        }

        protected override RadRect ArrangeAxes(RadRect availableRect)
        {
            RadSize availableSize = new RadSize(availableRect.Width, availableRect.Height);

            // Populate stacks
            AxisStack[] stacks = this.PrepareAxesStacks(availableSize);
            AxisStack leftStack = stacks[0];
            AxisStack topStack = stacks[1];
            AxisStack rightStack = stacks[2];
            AxisStack bottomStack = stacks[3];

            RadRect plotAreaRect = CalculatePlotAreaRect(availableRect, leftStack, topStack, rightStack, bottomStack);

            int maxIterations = 10;
            int currentIteration = 0;

            // axes may need several passes to adjust their desired size due to label fit mode
            bool isArrangeValid;
            do
            {
                isArrangeValid = true;

                // Although this seems an anti-pattern, it actually is safety coding
                // The logic behind axes layout is not completely verified yet and we do not want to enter an endless loop
                if (currentIteration > maxIterations)
                {
                    Debug.Assert(false, "Entering endless loop");
                    break;
                }

                if (!leftStack.IsEmpty)
                {
                    double lastRightPoint = plotAreaRect.X;
                    foreach (var axis in leftStack.axes)
                    {
                        var finalRect = new RadRect(lastRightPoint - axis.desiredSize.Width, plotAreaRect.Y, axis.desiredSize.Width, plotAreaRect.Height);
                        lastRightPoint = finalRect.X;
                        RadSize lastAxisDesiredSize = axis.desiredSize;
                        axis.Arrange(finalRect);
                        if (axis.desiredSize.Width != lastAxisDesiredSize.Width)
                        {
                            leftStack.desiredWidth += axis.desiredSize.Width - lastAxisDesiredSize.Width;
                            isArrangeValid = false;
                        }
                    }
                }

                if (!topStack.IsEmpty)
                {
                    double lastBottomPoint = plotAreaRect.Y;
                    foreach (var axis in topStack.axes)
                    {
                        var finalRect = new RadRect(plotAreaRect.X, lastBottomPoint - axis.desiredSize.Height, plotAreaRect.Width, axis.desiredSize.Height);
                        lastBottomPoint = finalRect.Y;
                        RadSize lastAxisDesiredSize = axis.desiredSize;
                        axis.Arrange(finalRect);
                        if (axis.desiredSize.Height != lastAxisDesiredSize.Height)
                        {
                            topStack.desiredHeight += axis.desiredSize.Height - lastAxisDesiredSize.Height;
                            isArrangeValid = false;
                        }
                    }
                }

                if (!rightStack.IsEmpty)
                {
                    double lastLeftPoint = plotAreaRect.Right;
                    foreach (var axis in rightStack.axes)
                    {
                        var finalRect = new RadRect(lastLeftPoint, plotAreaRect.Y, axis.desiredSize.Width, plotAreaRect.Height);
                        lastLeftPoint = finalRect.Right;
                        RadSize lastAxisDesiredSize = axis.desiredSize;
                        axis.Arrange(finalRect);
                        if (axis.desiredSize.Width != lastAxisDesiredSize.Width)
                        {
                            rightStack.desiredWidth += axis.desiredSize.Width - lastAxisDesiredSize.Width;
                            isArrangeValid = false;
                        }
                    }
                }

                if (!bottomStack.IsEmpty)
                {
                    double lastTopPoint = plotAreaRect.Bottom;
                    foreach (var axis in bottomStack.axes)
                    {
                        var finalRect = new RadRect(plotAreaRect.X, lastTopPoint, plotAreaRect.Width, axis.desiredSize.Height);
                        lastTopPoint = finalRect.Bottom;
                        RadSize lastAxisDesiredSize = axis.desiredSize;
                        axis.Arrange(finalRect);
                        if (axis.desiredSize.Height != finalRect.Height)
                        {
                            bottomStack.desiredHeight += axis.desiredSize.Height - lastAxisDesiredSize.Height;
                            isArrangeValid = false;
                        }
                    }
                }

                if (!isArrangeValid)
                {
                    plotAreaRect = CalculatePlotAreaRect(availableRect, leftStack, topStack, rightStack, bottomStack);
                }
                currentIteration++;
            }
            while (!isArrangeValid);

            return plotAreaRect;
        }

        private static RadRect CalculatePlotAreaRect(RadRect availableRect, AxisStack leftStack, AxisStack topStack, AxisStack rightStack, AxisStack bottomStack)
        {
            RadPoint topLeft = new RadPoint();

            double finalLeftRectWidth = leftStack.desiredWidth + leftStack.desiredMargin.Left + leftStack.desiredMargin.Right;
            double maxLeftMargin = Math.Max(topStack.desiredMargin.Left, bottomStack.desiredMargin.Left);
            topLeft.X = Math.Max(finalLeftRectWidth, maxLeftMargin);

            double finalTopRectHeight = topStack.desiredHeight + topStack.desiredMargin.Top + topStack.desiredMargin.Bottom;
            double maxTopMargin = Math.Max(leftStack.desiredMargin.Top, rightStack.desiredMargin.Top);
            topLeft.Y = Math.Max(finalTopRectHeight, maxTopMargin);

            RadPoint bottomRight = new RadPoint();

            double finalRightRectWidth = rightStack.desiredWidth + rightStack.desiredMargin.Left + rightStack.desiredMargin.Right;
            double maxRightMargin = Math.Max(topStack.desiredMargin.Right, bottomStack.desiredMargin.Right);
            bottomRight.X = availableRect.Width - Math.Max(finalRightRectWidth, maxRightMargin);

            double finalBottomRectHeight = bottomStack.desiredHeight + bottomStack.desiredMargin.Top + bottomStack.desiredMargin.Bottom;
            double maxBottomMargin = Math.Max(leftStack.desiredMargin.Bottom, rightStack.desiredMargin.Bottom);
            bottomRight.Y = availableRect.Height - Math.Max(finalBottomRectHeight, maxBottomMargin);

            RadRect plotAreaRect = new RadRect(topLeft, bottomRight);
            return RadRect.Round(plotAreaRect);
        }

        private AxisStack[] PrepareAxesStacks(RadSize availableSize)
        {
            // horizontal stacks
            AxisStack leftStack;
            AxisStack rightStack;
            List<AxisModel> leftAxes = new List<AxisModel>();
            List<AxisModel> rightAxes = new List<AxisModel>();
            foreach (var axis in this.SecondAxes)
            {
                if (axis.HorizontalLocation == AxisHorizontalLocation.Left)
                {
                    leftAxes.Add(axis);
                }
                else
                {
                    rightAxes.Add(axis);
                }
            }
            leftStack = new AxisStack(leftAxes);
            rightStack = new AxisStack(rightAxes);

            // vertical stacks
            AxisStack topStack;
            AxisStack bottomStack;
            List<AxisModel> topAxes = new List<AxisModel>();
            List<AxisModel> bottomAxes = new List<AxisModel>();
            foreach (var axis in this.FirstAxes)
            {
                if (axis.VerticalLocation == AxisVerticalLocation.Bottom)
                {
                    bottomAxes.Add(axis);
                }
                else
                {
                    topAxes.Add(axis);
                }
            }
            bottomStack = new AxisStack(bottomAxes);
            topStack = new AxisStack(topAxes);

            leftStack.Measure(availableSize);
            topStack.Measure(availableSize);
            rightStack.Measure(availableSize);
            bottomStack.Measure(availableSize);

            return new AxisStack[] { leftStack, topStack, rightStack, bottomStack };
        }

        private class AxisStack
        {
            internal RadThickness desiredMargin;
            internal double desiredWidth = double.NaN, desiredHeight = double.NaN;
            internal IList<AxisModel> axes;

            public AxisStack(IList<AxisModel> axes)
            {
                this.axes = axes;
            }

            public bool IsEmpty
            {
                get
                {
                    return this.axes.Count == 0;
                }
            }

            public void Measure(RadSize availableSize)
            {
                this.desiredWidth = 0;
                this.desiredHeight = 0;
                for (int i = 0; i < this.axes.Count; i++)
                {
                    AxisModel axis = this.axes[i];
                    axis.Measure(availableSize);
                    this.desiredWidth = this.desiredWidth + axis.desiredSize.Width;
                    this.desiredHeight = this.desiredHeight + axis.desiredSize.Height;
                    RadThickness margin = axis.desiredMargin;
                    this.desiredMargin.Left = Math.Max(this.desiredMargin.Left, margin.Left);
                    this.desiredMargin.Top = Math.Max(this.desiredMargin.Top, margin.Top);
                    this.desiredMargin.Right = Math.Max(this.desiredMargin.Right, margin.Right);
                    this.desiredMargin.Bottom = Math.Max(this.desiredMargin.Bottom, margin.Bottom);
                }
            }
        }
    }
}