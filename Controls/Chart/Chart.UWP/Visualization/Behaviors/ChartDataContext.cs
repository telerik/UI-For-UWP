using System.Collections.Generic;
using Windows.Foundation;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// An object of this type is used as a data context for chart's behaviors.
    /// For example the tool tip behavior can use a chart data context to populate
    /// its tool tip template with data.
    /// </summary>
    public class ChartDataContext
    {
        internal ChartDataContext(List<DataPointInfo> infos, DataPointInfo closestPoint)
        {
            this.DataPoints = infos;
            this.ClosestDataPoint = closestPoint;
        }

        /// <summary>
        /// Gets the physical point (in coordinates, relative to the chart surface) this context is associated with.
        /// </summary>
        public Point TouchLocation
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets an object that contains the closest data point to the tap location
        /// and the series object to which the data point belongs.
        /// </summary>
        public DataPointInfo ClosestDataPoint
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a list of <see cref="DataPointInfo"/> objects each of which contains the closest data
        /// point to the tap location and the point's corresponding series.
        /// </summary>
        public List<DataPointInfo> DataPoints
        {
            get;
            private set;
        }
    }
}
