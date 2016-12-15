using System.Collections;
using System.Collections.Generic;

namespace Telerik.Charting
{
    /// <summary>
    /// Represents chart series that consist of data points.
    /// </summary>
    /// <typeparam name="T">Must be a <see cref="DataPoint"/>.</typeparam>
    internal abstract class DataPointSeriesModel<T> : ChartSeriesModel where T : DataPoint
    {
        private DataPointCollection<T> dataPoints;

        public DataPointSeriesModel()
        {
            this.dataPoints = new DataPointCollection<T>(this);
        }

        /// <summary>
        /// Gets the collection of data points contained in this instance.
        /// </summary>
        public DataPointCollection<T> DataPoints
        {
            get
            {
                return this.dataPoints;
            }
        }

        internal override IList<DataPoint> DataPointsInternal
        {
            get
            {
                return this.dataPoints;
            }
        }
    }
}
