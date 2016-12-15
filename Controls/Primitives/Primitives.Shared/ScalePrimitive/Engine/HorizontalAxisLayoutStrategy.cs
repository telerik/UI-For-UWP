using System;
using Telerik.Core;

namespace Telerik.UI.Xaml.Controls.Primitives.Scale
{
    internal class HorizontalAxisLayoutStrategy : AxisModelLayoutStrategy
    {
        private NumericalAxisModel owner;

        internal HorizontalAxisLayoutStrategy(NumericalAxisModel owner)
        {
            this.owner = owner;
        }

        internal override RadSize UpdateDesiredSize(RadSize availableSize)
        {
            double desiredWidth = availableSize.Width;
            double desiredHeight = availableSize.Height;

            double tickLength = this.owner.tickSize.Height;
            double tickThickness = this.owner.tickSize.Width;

            if (this.owner.TickPlacementCache == ScaleElementPlacement.None)
            {
                if (this.owner.LabelPlacementCache == ScaleElementPlacement.None)
                {
                    desiredHeight = this.owner.LineThicknessCache;
                }
                else if (this.owner.LabelPlacementCache == ScaleElementPlacement.Center)
                {
                    desiredHeight = Math.Max(this.owner.LineThicknessCache, this.owner.MaxLabelHeight);
                }
                else
                {
                    desiredHeight = this.owner.LineThicknessCache + this.owner.MaxLabelHeight;
                }
            }
            else if (this.owner.TickPlacementCache == ScaleElementPlacement.TopLeft)
            {
                if (this.owner.LabelPlacementCache == ScaleElementPlacement.None)
                {
                    desiredHeight = this.owner.LineThicknessCache + tickLength;
                }
                else if (this.owner.LabelPlacementCache == ScaleElementPlacement.Center)
                {
                    desiredHeight = Math.Max(this.owner.MaxLabelHeight, this.owner.LineThicknessCache + tickLength + (this.owner.MaxLabelHeight / 2));
                }
                else
                {
                    desiredHeight = this.owner.LineThicknessCache + this.owner.MaxLabelHeight + tickLength;
                }
            }
            else if (this.owner.TickPlacementCache == ScaleElementPlacement.Center)
            {
                if (this.owner.LabelPlacementCache == ScaleElementPlacement.None)
                {
                    desiredHeight = Math.Max(this.owner.LineThicknessCache, tickLength);
                }
                else if (this.owner.LabelPlacementCache == ScaleElementPlacement.Center)
                {
                    desiredHeight = Math.Max(this.owner.LineThicknessCache, tickLength);
                    desiredHeight = Math.Max(desiredHeight, this.owner.MaxLabelHeight);
                }
                else
                {
                    desiredHeight = this.owner.MaxLabelHeight + Math.Max(this.owner.LineThicknessCache, tickLength);
                }
            }
            else
            {
                if (this.owner.LabelPlacementCache == ScaleElementPlacement.None)
                {
                    desiredHeight = this.owner.LineThicknessCache + tickLength;
                }
                else if (this.owner.LabelPlacementCache == ScaleElementPlacement.Center)
                {
                    desiredHeight = this.owner.LineThicknessCache + tickLength;
                    if (this.owner.LineThicknessCache < this.owner.MaxLabelHeight)
                    {
                        desiredHeight += (this.owner.MaxLabelHeight - this.owner.LineThicknessCache) / 2;
                    }
                }
                else
                {
                    desiredHeight = this.owner.MaxLabelHeight + this.owner.LineThicknessCache + tickLength;
                }
            }

            desiredWidth = this.owner.Ticks.Count * tickThickness;
            desiredWidth = Math.Max(desiredWidth, this.owner.LabelSize.Width);
            desiredWidth += this.owner.AxisLineOffset.Left + this.owner.AxisLineOffset.Right;

            return new RadSize(desiredWidth, desiredHeight);
        }

        internal override void ArrangeTicks()
        {
            double x, y, lineWidth;
            double tickLength = this.owner.tickSize.Height;
            double tickThickness = this.owner.tickSize.Width;

            if (this.owner.TickPlacementCache == ScaleElementPlacement.TopLeft)
            {
                y = this.owner.Line.Y1 - (this.owner.LineThicknessCache / 2) - tickLength;
            }
            else if (this.owner.TickPlacementCache == ScaleElementPlacement.Center)
            {
                double fix = (tickLength % 2 == 0 && this.owner.LineThicknessCache % 2 == 1) ? 1 : 0;
                y = this.owner.Line.Y1 - ((tickLength + fix) / 2);
            }
            else
            {
                y = this.owner.Line.Y1 + (this.owner.LineThicknessCache / 2);
            }

            lineWidth = this.owner.Line.X2 - this.owner.Line.X1;

            foreach (var tick in this.owner.Ticks)
            {
                x = this.owner.Line.X1 - tickThickness / 2.0 + (double)(tick.normalizedValue * (decimal)lineWidth);
                tick.layoutSlot = RadRect.Round(new RadRect(x, y, tickThickness, tickLength));
            }
        }

        internal override RadLine ArrangeLine(RadRect rect)
        {
            RadLine arrangedLine;
            double leftPadding = 0;
            double rightPadding = 0;
            double tickLength = this.owner.tickSize.Height;
            double tickThickness = this.owner.tickSize.Width;

            if (this.owner.LayoutMode == ScaleLayoutMode.ShortenAxisLine && this.owner.Labels.Count != 0)
            {
                leftPadding = (this.owner.Labels[0].desiredSize.Width / 2.0) - (tickThickness / 2.0);
                rightPadding = (this.owner.Labels[this.owner.Labels.Count - 1].desiredSize.Width / 2.0) - (tickThickness / 2.0);

                if (leftPadding < 0)
                {
                    leftPadding = 0;
                }
                if (rightPadding < 0)
                {
                    rightPadding = 0;
                }
            }

            arrangedLine.X1 = (int)(rect.X + leftPadding);
            arrangedLine.X2 = (int)(rect.Right - rightPadding);

            double y = 0;
            if (this.owner.LabelPlacementCache == ScaleElementPlacement.None)
            {
                if (this.owner.TickPlacementCache == ScaleElementPlacement.TopLeft)
                {
                    y = tickLength + this.owner.LineThicknessCache / 2;
                }
                else if (this.owner.TickPlacementCache == ScaleElementPlacement.Center)
                {
                    y = Math.Max(tickLength / 2, this.owner.LineThicknessCache / 2);
                }
                else
                {
                    y = this.owner.LineThicknessCache / 2;
                }
            }
            else if (this.owner.LabelPlacementCache == ScaleElementPlacement.TopLeft)
            {
                if (this.owner.TickPlacementCache == ScaleElementPlacement.None)
                {
                    y = this.owner.MaxLabelHeight + this.owner.LineThicknessCache / 2;
                }
                else if (this.owner.TickPlacementCache == ScaleElementPlacement.TopLeft)
                {
                    y = this.owner.MaxLabelHeight + tickLength + (this.owner.LineThicknessCache / 2);
                }
                else if (this.owner.TickPlacementCache == ScaleElementPlacement.Center)
                {
                    y = this.owner.MaxLabelHeight + Math.Max(this.owner.LineThicknessCache / 2, tickLength / 2);
                }
                else
                {
                    y = this.owner.MaxLabelHeight + this.owner.LineThicknessCache / 2;
                }
            }
            else if (this.owner.LabelPlacementCache == ScaleElementPlacement.Center)
            {
                if (this.owner.TickPlacementCache == ScaleElementPlacement.None)
                {
                    y = Math.Max(this.owner.MaxLabelHeight / 2, this.owner.LineThicknessCache / 2);
                }
                else if (this.owner.TickPlacementCache == ScaleElementPlacement.TopLeft)
                {
                    y = Math.Max((this.owner.LineThicknessCache + this.owner.MaxLabelHeight) / 2, tickLength);
                }
                else if (this.owner.TickPlacementCache == ScaleElementPlacement.Center)
                {
                    y = Math.Max(this.owner.MaxLabelHeight / 2, tickLength / 2);
                    y = Math.Max(y, this.owner.LineThicknessCache / 2);
                }
                else
                {
                    y = Math.Max(this.owner.MaxLabelHeight / 2, this.owner.LineThicknessCache / 2);
                }
            }
            else
            {
                if (this.owner.TickPlacementCache == ScaleElementPlacement.TopLeft)
                {
                    y = tickLength + this.owner.LineThicknessCache / 2;
                }
                else if (this.owner.TickPlacementCache == ScaleElementPlacement.Center)
                {
                    y = Math.Max(tickLength / 2, this.owner.LineThicknessCache / 2);
                }
                else
                {
                    y = this.owner.LineThicknessCache / 2;
                }
            }

            double antiAliasOffset = this.owner.LineThickness % 2 == 1 ? 0.5 : 0;
            arrangedLine.Y1 = arrangedLine.Y2 = (int)y + antiAliasOffset;

            return arrangedLine;
        }

        internal override void ArrangeLabels(RadRect rect)
        {
            if (this.owner.Labels.Count > 0)
            {
                double firstLabelY;
                double lastLabelY;
                double tickLength = this.owner.tickSize.Height;
                double tickThickness = this.owner.tickSize.Width;

                if (this.owner.LabelPlacementCache == ScaleElementPlacement.TopLeft)
                {
                    firstLabelY = 0;
                    lastLabelY = 0;
                }
                else if (this.owner.LabelPlacementCache == ScaleElementPlacement.Center)
                {
                    firstLabelY = this.owner.Line.Y1 - (this.owner.Labels[0].desiredSize.Height / 2);
                    lastLabelY = this.owner.Line.Y1 - (this.owner.Labels[1].desiredSize.Height / 2);
                }
                else
                {
                    if (this.owner.TickPlacementCache == ScaleElementPlacement.None)
                    {
                        firstLabelY = this.owner.LineThicknessCache;
                        lastLabelY = firstLabelY;
                    }
                    else if (this.owner.TickPlacementCache == ScaleElementPlacement.TopLeft)
                    {
                        firstLabelY = tickLength + this.owner.LineThicknessCache;
                        lastLabelY = firstLabelY;
                    }
                    else if (this.owner.TickPlacementCache == ScaleElementPlacement.Center)
                    {
                        firstLabelY = Math.Max(tickLength, this.owner.LineThicknessCache);
                        lastLabelY = firstLabelY;
                    }
                    else
                    {
                        firstLabelY = tickLength + this.owner.LineThicknessCache;
                        lastLabelY = firstLabelY;
                    }
                }

                double actualTickThickness = this.owner.TickPlacementCache == ScaleElementPlacement.None ? 0 : tickThickness;
                double leftMargin = actualTickThickness > this.owner.Labels[0].desiredSize.Width ? ((actualTickThickness - this.owner.Labels[0].desiredSize.Width) / 2) : 0;
                double rightMargin = actualTickThickness > this.owner.Labels[1].desiredSize.Width ? ((actualTickThickness + this.owner.Labels[1].desiredSize.Width) / 2) : this.owner.Labels[1].desiredSize.Width;
                this.owner.Labels[0].Arrange(new RadRect(rect.X + leftMargin, firstLabelY, this.owner.Labels[0].desiredSize.Width, this.owner.Labels[0].desiredSize.Height));
                this.owner.Labels[1].Arrange(new RadRect(rect.Right - rightMargin, lastLabelY, this.owner.Labels[1].desiredSize.Width, this.owner.Labels[1].desiredSize.Height));
            }
        }
    }
}
