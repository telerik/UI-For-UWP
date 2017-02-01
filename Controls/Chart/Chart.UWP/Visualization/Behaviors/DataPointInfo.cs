using System;
using Telerik.Charting;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// This class contains the closest data point to a tap location
    /// as well as the point's corresponding series object.
    /// </summary>
    public class DataPointInfo
    {
        private ChartSeriesModel seriesModel;
        private object displayContent;
        private object displayHeader;
        private DataPoint dataPoint;
        private int priority;

        internal DataPointInfo()
        {
            // TODO: Series / Indicator priority in the TrackBallInfoControl should be user configurable.
            this.Priority = 1;
        }
        
        /// <summary>
        /// Gets the series object that contains the data point.
        /// </summary>
        public ChartSeries Series
        {
            get
            {
                if (this.seriesModel == null)
                {
                    return null;
                }

                return (ChartSeries)this.seriesModel.presenter;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the layout slot of the data point contains the touch location.
        /// </summary>
        public bool ContainsTouchLocation
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the data point in the series object that is closest to the tap location.
        /// </summary>
        public DataPoint DataPoint
        {
            get
            {
                return this.dataPoint;
            }
            internal set
            {
                this.dataPoint = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the series which host the associated data point.
        /// </summary>
        public object DisplayHeader
        {
            get
            {
                if (this.displayHeader != null)
                {
                    return this.displayHeader;
                }

                ChartSeries chartSeries = this.Series;
                if (chartSeries != null)
                {
                    return string.IsNullOrEmpty(chartSeries.DisplayName) ? chartSeries.GetType().Name : chartSeries.DisplayName;
                }

                return string.Empty;
            }
            set
            {
                this.displayHeader = value;
            }
        }

        /// <summary>
        /// Gets or sets the object that visually represents the value of the associated point.
        /// </summary>
        public object DisplayContent
        {
            get
            {
                if (this.displayContent != null)
                {
                    return this.displayContent;
                }

                return this.dataPoint.GetTooltipValue();
            }
            set
            {
                this.displayContent = value;
            }
        }

        internal ChartSeriesModel SeriesModel
        {
            get
            {
                return this.seriesModel;
            }
            set
            {
                this.seriesModel = value;
                this.OnSeriesModelChanged();
            }
        }

        internal int Priority
        {
            get
            {
                return this.priority;
            }
            set
            {
                this.priority = value;
            }
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        public override string ToString()
        {
            return this.DisplayHeader + Environment.NewLine + this.DisplayContent;
        }

        private void OnSeriesModelChanged()
        {
            if (this.Series is IndicatorBase)
            {
                this.Priority = 100;
            }
        }
    }
}
