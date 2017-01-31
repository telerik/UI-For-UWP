using System;
using Telerik.Core;

namespace Telerik.UI.Xaml.Controls.Primitives.Scale
{
    internal class VerticalAxisLayoutStrategy : AxisModelLayoutStrategy
    {
        private NumericalAxisModel owner;

        internal VerticalAxisLayoutStrategy(NumericalAxisModel owner)
        {
            this.owner = owner;
        }

        internal override RadSize UpdateDesiredSize(RadSize availableSize)
        {
            double desiredWidth = availableSize.Width;
            double desiredHeight = availableSize.Height;

            double tickLength = this.owner.tickSize.Width;
            double tickThickness = this.owner.tickSize.Height;

            if (this.owner.TickPlacementCache == ScaleElementPlacement.None)
            {
                if (this.owner.LabelPlacementCache == ScaleElementPlacement.None)
                {
                    desiredWidth = this.owner.LineThicknessCache;
                }
                else if (this.owner.LabelPlacementCache == ScaleElementPlacement.Center)
                {
                    desiredWidth = Math.Max(this.owner.LineThicknessCache, this.owner.MaxLabelWidth);
                }
                else
                {
                    desiredWidth = this.owner.LineThicknessCache + this.owner.MaxLabelWidth;
                }
            }
            else if (this.owner.TickPlacementCache == ScaleElementPlacement.TopLeft)
            {
                if (this.owner.LabelPlacementCache == ScaleElementPlacement.None)
                {
                    desiredWidth = this.owner.LineThicknessCache + tickLength;
                }
                else if (this.owner.LabelPlacementCache == ScaleElementPlacement.Center)
                {
                    desiredWidth = Math.Max(this.owner.MaxLabelWidth, this.owner.LineThicknessCache + tickLength + (this.owner.MaxLabelWidth / 2));
                }
                else
                {
                    desiredWidth = this.owner.LineThicknessCache + this.owner.MaxLabelWidth + tickLength;
                }
            }
            else if (this.owner.TickPlacementCache == ScaleElementPlacement.Center)
            {
                if (this.owner.LabelPlacementCache == ScaleElementPlacement.None)
                {
                    desiredWidth = Math.Max(this.owner.LineThicknessCache, tickLength);
                }
                else if (this.owner.LabelPlacementCache == ScaleElementPlacement.Center)
                {
                    desiredWidth = Math.Max(this.owner.LineThicknessCache, tickLength);
                    desiredWidth = Math.Max(desiredWidth, this.owner.MaxLabelWidth);
                }
                else
                {
                    desiredWidth = this.owner.MaxLabelWidth + Math.Max(this.owner.LineThicknessCache, tickLength);
                }
            }
            else
            {
                if (this.owner.LabelPlacementCache == ScaleElementPlacement.None)
                {
                    desiredWidth = this.owner.LineThicknessCache + tickLength;
                }
                else if (this.owner.LabelPlacementCache == ScaleElementPlacement.Center)
                {
                    desiredWidth = this.owner.LineThicknessCache + tickLength;
                    if (this.owner.LineThicknessCache < this.owner.MaxLabelWidth)
                    {
                        desiredWidth += (this.owner.MaxLabelWidth - this.owner.LineThicknessCache) / 2;
                    }
                }
                else
                {
                    desiredWidth = this.owner.MaxLabelWidth + this.owner.LineThicknessCache + tickLength;
                }
            }

            desiredHeight = this.owner.Ticks.Count * tickThickness;
            desiredHeight = Math.Max(desiredHeight, this.owner.LabelSize.Height);
            desiredHeight += this.owner.AxisLineOffset.Top + this.owner.AxisLineOffset.Bottom;

            return new RadSize(desiredWidth, desiredHeight);
        }

        internal override void ArrangeTicks()
        {
            double x, y, lineHeight;
            double tickLength = this.owner.tickSize.Width;
            double tickThickness = this.owner.tickSize.Height;

            if (this.owner.TickPlacementCache == ScaleElementPlacement.TopLeft)
            {
                x = this.owner.Line.X1 - (this.owner.LineThicknessCache / 2) - tickLength;
            }
            else if (this.owner.TickPlacementCache == ScaleElementPlacement.Center)
            {
                double fix = (tickLength % 2 == 0 && this.owner.LineThicknessCache % 2 == 1) ? 1 : 0;
                x = this.owner.Line.X1 - ((tickLength + fix) / 2);
            }
            else
            {
                x = this.owner.Line.X1 + (this.owner.LineThicknessCache / 2);
            }

            lineHeight = this.owner.Line.Y2 - this.owner.Line.Y1;

            foreach (var tick in this.owner.Ticks)
            {
                y = this.owner.Line.Y2 - tickThickness / 2.0 - (double)(tick.normalizedValue * (decimal)lineHeight);
                tick.layoutSlot = RadRect.Round(new RadRect(x, y, tickLength, tickThickness));
            }
        }

        internal override RadLine ArrangeLine(RadRect rect)
        {
            RadLine arrangedLine;
            double topPadding = 0;
            double bottomPadding = 0;
            double tickLength = this.owner.tickSize.Width;
            double tickThickness = this.owner.tickSize.Height;

            if (this.owner.LayoutMode == ScaleLayoutMode.ShortenAxisLine && this.owner.Labels.Count != 0)
            {
                bottomPadding = (this.owner.Labels[0].desiredSize.Height / 2.0) - (tickThickness / 2.0);
                topPadding = (this.owner.Labels[this.owner.Labels.Count - 1].desiredSize.Height / 2.0) - (tickThickness / 2.0);

                if (bottomPadding < 0)
                {
                    bottomPadding = 0;
                }
                if (topPadding < 0)
                {
                    topPadding = 0;
                }
            }

            arrangedLine.Y1 = (int)(rect.Y + topPadding);
            arrangedLine.Y2 = (int)(rect.Bottom - bottomPadding);

            double x = 0;
            if (this.owner.LabelPlacementCache == ScaleElementPlacement.None)
            {
                if (this.owner.TickPlacementCache == ScaleElementPlacement.TopLeft)
                {
                    x = tickLength + this.owner.LineThicknessCache / 2;
                }
                else if (this.owner.TickPlacementCache == ScaleElementPlacement.Center)
                {
                    x = Math.Max(tickLength / 2, this.owner.LineThicknessCache / 2);
                }
                else
                {
                    x = this.owner.LineThicknessCache / 2;
                }
            }
            else if (this.owner.LabelPlacementCache == ScaleElementPlacement.TopLeft)
            {
                if (this.owner.TickPlacementCache == ScaleElementPlacement.None)
                {
                    x = this.owner.MaxLabelWidth + this.owner.LineThicknessCache / 2;
                }
                else if (this.owner.TickPlacementCache == ScaleElementPlacement.TopLeft)
                {
                    x = this.owner.MaxLabelWidth + tickLength + (this.owner.LineThicknessCache / 2);
                }
                else if (this.owner.TickPlacementCache == ScaleElementPlacement.Center)
                {
                    x = this.owner.MaxLabelWidth + Math.Max(this.owner.LineThicknessCache / 2, tickLength / 2);
                }
                else
                {
                    x = this.owner.MaxLabelWidth + this.owner.LineThicknessCache / 2;
                }
            }
            else if (this.owner.LabelPlacementCache == ScaleElementPlacement.Center)
            {
                if (this.owner.TickPlacementCache == ScaleElementPlacement.None)
                {
                    x = Math.Max(this.owner.MaxLabelWidth / 2, this.owner.LineThicknessCache / 2);
                }
                else if (this.owner.TickPlacementCache == ScaleElementPlacement.TopLeft)
                {
                    x = Math.Max((this.owner.LineThicknessCache + this.owner.MaxLabelWidth) / 2, tickLength);
                }
                else if (this.owner.TickPlacementCache == ScaleElementPlacement.Center)
                {
                    x = Math.Max(this.owner.MaxLabelWidth / 2, tickLength / 2);
                    x = Math.Max(x, this.owner.LineThicknessCache / 2);
                }
                else
                {
                    x = Math.Max(this.owner.MaxLabelWidth / 2, this.owner.LineThicknessCache / 2);
                }
            }
            else
            {
                if (this.owner.TickPlacementCache == ScaleElementPlacement.TopLeft)
                {
                    x = tickLength + this.owner.LineThicknessCache / 2;
                }
                else if (this.owner.TickPlacementCache == ScaleElementPlacement.Center)
                {
                    x = Math.Max(tickLength / 2, this.owner.LineThicknessCache / 2);
                }
                else
                {
                    x = this.owner.LineThicknessCache / 2;
                }
            }

            double antiAliasOffset = this.owner.LineThickness % 2 == 1 ? 0.5 : 0;
            arrangedLine.X1 = arrangedLine.X2 = (int)x + antiAliasOffset;

            return arrangedLine;
        }

        internal override void ArrangeLabels(RadRect rect)
        {
            if (this.owner.Labels.Count > 0)
            {
                double firstLabelX;
                double lastLabelX;
                double tickLength = this.owner.tickSize.Width;
                double tickThickness = this.owner.tickSize.Height;

                if (this.owner.LabelPlacementCache == ScaleElementPlacement.TopLeft)
                {
                    if (this.owner.TickPlacementCache == ScaleElementPlacement.TopLeft)
                    {
                        firstLabelX = this.owner.Line.X1 - this.owner.Labels[0].desiredSize.Width - (this.owner.LineThicknessCache / 2) - tickLength;
                        lastLabelX = this.owner.Line.X1 - this.owner.Labels[1].desiredSize.Width - (this.owner.LineThicknessCache / 2) - tickLength;
                    }
                    else if (this.owner.TickPlacementCache == ScaleElementPlacement.Center)
                    {
                        double left = this.owner.Line.X1 - (Math.Max(this.owner.LineThicknessCache, tickLength) / 2);
                        firstLabelX = left - this.owner.Labels[0].desiredSize.Width;
                        lastLabelX = left - this.owner.Labels[1].desiredSize.Width;
                    }
                    else
                    {
                        firstLabelX = this.owner.Line.X1 - this.owner.Labels[0].desiredSize.Width - (this.owner.LineThicknessCache / 2);
                        lastLabelX = this.owner.Line.X1 - this.owner.Labels[1].desiredSize.Width - (this.owner.LineThicknessCache / 2);
                    }
                }
                else if (this.owner.LabelPlacementCache == ScaleElementPlacement.Center)
                {
                    firstLabelX = this.owner.Line.X1 - (this.owner.Labels[0].desiredSize.Width / 2);
                    lastLabelX = this.owner.Line.X1 - (this.owner.Labels[1].desiredSize.Width / 2);
                }
                else
                {
                    if (this.owner.TickPlacementCache == ScaleElementPlacement.None)
                    {
                        firstLabelX = this.owner.LineThicknessCache;
                        lastLabelX = firstLabelX;
                    }
                    else if (this.owner.TickPlacementCache == ScaleElementPlacement.TopLeft)
                    {
                        firstLabelX = tickLength + this.owner.LineThicknessCache;
                        lastLabelX = firstLabelX;
                    }
                    else if (this.owner.TickPlacementCache == ScaleElementPlacement.Center)
                    {
                        firstLabelX = Math.Max(tickLength, this.owner.LineThicknessCache);
                        lastLabelX = firstLabelX;
                    }
                    else
                    {
                        firstLabelX = tickLength + this.owner.LineThicknessCache;
                        lastLabelX = firstLabelX;
                    }
                }

                double actualTickThickness = this.owner.TickPlacementCache == ScaleElementPlacement.None ? 0 : tickThickness;
                double bottomMargin = actualTickThickness > this.owner.Labels[0].desiredSize.Height ? ((actualTickThickness + this.owner.Labels[0].desiredSize.Height) / 2) : this.owner.Labels[1].desiredSize.Height;
                double topMargin = actualTickThickness > this.owner.Labels[1].desiredSize.Height ? ((actualTickThickness - this.owner.Labels[1].desiredSize.Height) / 2) : 0;

                this.owner.Labels[0].Arrange(new RadRect(firstLabelX, rect.Bottom - bottomMargin, this.owner.Labels[0].desiredSize.Width, this.owner.Labels[0].desiredSize.Height));
                this.owner.Labels[1].Arrange(new RadRect(lastLabelX, rect.Y + topMargin, this.owner.Labels[1].desiredSize.Width, this.owner.Labels[1].desiredSize.Height));
            }
        }
    }
}
