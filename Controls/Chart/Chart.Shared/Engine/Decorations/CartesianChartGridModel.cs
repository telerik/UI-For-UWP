using System;
using System.Collections.Generic;
using Telerik.Core;

namespace Telerik.Charting
{
    internal class CartesianChartGridModel : ChartGridModel
    {
        internal List<GridStripe> xStripes;
        internal List<GridStripe> yStripes;

        public CartesianChartGridModel()
        {
            this.xStripes = new List<GridStripe>();
            this.yStripes = new List<GridStripe>();
        }

        internal override RadRect ArrangeOverride(RadRect rect)
        {
            this.xStripes.Clear();
            this.yStripes.Clear();

            var chartArea = this.GetChartArea<CartesianChartAreaModel>();
            if (chartArea.primaryFirstAxis != null)
            {
                this.BuildXStripes(chartArea.primaryFirstAxis, rect);
            }

            if (chartArea.primarySecondAxis != null)
            {
                this.BuildYStripes(chartArea.primarySecondAxis, rect);
            }

            return rect;
        }

        private void BuildXStripes(AxisModel xAxis, RadRect rect)
        {
            double width;
            double thickness = xAxis.TickThickness;
            double thicknessOffset = (int)(thickness / 2);

            int plotOriginX = (int)(this.GetChartArea().view.PlotOriginX * rect.Width);
            int plotOriginY = (int)(this.GetChartArea().view.PlotOriginY * rect.Height);

            foreach (AxisTickModel tick in xAxis.MajorTicks)
            {
                AxisTickModel nextMajor = tick.NextMajorTick;
                if (nextMajor == null)
                {
                    break;
                }

                width = Math.Abs(nextMajor.layoutSlot.X - tick.layoutSlot.X);
                var right = xAxis.IsInverse ? nextMajor.layoutSlot.X : tick.layoutSlot.X;
                GridStripe stripe = new GridStripe();
                stripe.BorderRect = new RadRect(right + plotOriginX + thicknessOffset, rect.Y, width, rect.Height);
                stripe.FillRect = new RadRect(right + plotOriginY + thicknessOffset + thickness, rect.Y, width - thickness, rect.Height);
                stripe.AssociatedTick = tick;

                this.xStripes.Add(stripe);
            }
        }

        private void BuildYStripes(AxisModel yAxis, RadRect rect)
        {
            double height;
            double thickness = yAxis.TickThickness;
            double thicknessOffset = (int)(thickness / 2);

            int plotOriginX = (int)(this.GetChartArea().view.PlotOriginX * rect.Width);
            int plotOriginY = (int)(this.GetChartArea().view.PlotOriginY * rect.Height);

            foreach (AxisTickModel tick in yAxis.MajorTicks)
            {
                AxisTickModel nextMajor = tick.NextMajorTick;
                if (nextMajor == null)
                {
                    break;
                }

                height = Math.Abs(tick.layoutSlot.Y - nextMajor.layoutSlot.Y);
                var top = yAxis.IsInverse ? nextMajor.layoutSlot.Y : tick.layoutSlot.Y;

                GridStripe stripe = new GridStripe();
                stripe.BorderRect = new RadRect(rect.X, top + plotOriginY - height + thicknessOffset, rect.Width, height);
                stripe.FillRect = new RadRect(rect.X, top + plotOriginY - height + thickness, rect.Width, height - thickness);
                stripe.AssociatedTick = tick;

                this.yStripes.Add(stripe);
            }
        }
    }
}