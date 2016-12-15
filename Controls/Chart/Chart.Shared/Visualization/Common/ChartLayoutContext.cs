using System;
using Telerik.Charting;
using Telerik.Core;
using Windows.Foundation;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal struct ChartLayoutContext
    {
        public static readonly ChartLayoutContext Invalid = new ChartLayoutContext(RadChartBase.InfinitySize, RadChartBase.InfinitySize, RadChartBase.InfinityPoint, RadRect.Empty);

        public Size AvailableSize;
        public Size Scale;
        public Point PlotOrigin;
        public RadRect ClipRect;
        public ChartLayoutFlags Flags;
        public bool IsEmpty; // determines whether the EmptyContent of the chart is currently displayed

        public ChartLayoutContext(Size availableSize, Size scale, Point plotOrigin, RadRect clip)
        {
            this.AvailableSize = availableSize;
            this.Scale = scale;
            this.PlotOrigin = plotOrigin;
            this.ClipRect = clip;
            this.Flags = ChartLayoutFlags.None;
            this.IsEmpty = false;
        }
    }
}
