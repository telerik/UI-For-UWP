using Telerik.Core;
namespace Telerik.Charting
{
    /// <summary>
    /// Defines the root of the whole chart.
    /// </summary>
    public interface IChartView : IChartElementPresenter, IView
    {
        /// <summary>
        /// Gets the current scale applied along the horizontal direction.
        /// </summary>
        double ZoomWidth
        {
            get;
        }

        /// <summary>
        /// Gets the current scale applied along the vertical direction.
        /// </summary>
        double ZoomHeight
        {
            get;
        }

        /// <summary>
        /// Gets the X-coordinate of the top-left corner where the layout should start from.
        /// </summary>
        double PlotOriginX
        {
            get;
        }

        /// <summary>
        /// Gets the Y-coordinate of the top-left corner where the layout should start from.
        /// </summary>
        double PlotOriginY
        {
            get;
        }

        /// <summary>
        /// Gets the rect that encloses the plot area in view coordinates - that is without the zoom factor applied and with the pan offset calculated.
        /// </summary>
        RadRect PlotAreaClip
        {
            get;
        }
    }
}
