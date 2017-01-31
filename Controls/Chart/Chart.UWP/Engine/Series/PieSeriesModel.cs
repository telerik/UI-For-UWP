using System;
using Telerik.Core;

namespace Telerik.Charting
{
    internal class PieSeriesModel : DataPointSeriesModel<PieDataPoint>
    {
        internal const string DefaultLabelFormat = "p0";

        internal static readonly int RangePropertyKey = PropertyKeys.Register(typeof(PieSeriesModel), "Range", ChartAreaInvalidateFlags.All);
        internal static readonly int LabelFormatPropertyKey = PropertyKeys.Register(typeof(PieSeriesModel), "LabelFormat", ChartAreaInvalidateFlags.All);

        internal string labelFormat = DefaultLabelFormat;
        internal bool isDataPrepared;

        private double total;
        private double maxOffset;

        public PieSeriesModel()
        {
            this.TrackPropertyChanged = true;
        }

        /// <summary>
        /// Gets or sets the string used to format the Percent value of each data point. Defaults to 'p0'.
        /// </summary>
        public string LabelFormat
        {
            get
            {
                return this.labelFormat;
            }
            set
            {
                this.SetValue(LabelFormatPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="AngleRange"/> structure that defines the starting and sweep angles of the pie.
        /// </summary>
        public AngleRange Range
        {
            get
            {
                return this.GetTypedValue<AngleRange>(RangePropertyKey, AngleRange.Default);
            }
            set
            {
                this.SetValue(RangePropertyKey, value);
            }
        }

        /// <summary>
        /// Gets the maximum point offset from the center.
        /// </summary>
        public double MaxOffsetFromCenter
        {
            get
            {
                return this.maxOffset;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Method is internal")]
        internal override void OnPropertyChanged(RadPropertyEventArgs e)
        {
            if (e.Key == LabelFormatPropertyKey)
            {
                this.labelFormat = (string)e.NewValue;
            }
            else if (e.Key == RangePropertyKey)
            {
                this.UpdateDataPoints();
            }

            base.OnPropertyChanged(e);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Method is internal")]
        internal override void ProcessMessage(Message message)
        {
            base.ProcessMessage(message);

            if (message.Id == Node.PropertyChangedMessage)
            {
                this.isDataPrepared = false;
            }
        }

        internal override RadRect ArrangeOverride(RadRect rect)
        {
            if (!this.isDataPrepared)
            {
                this.UpdateTotal();
                this.UpdateDataPoints();
                this.isDataPrepared = true;
            }
            else
            {
                this.UpdateMaxOffset();
            }

            return rect;
        }

        internal override ModifyChildrenResult CanAddChild(Node child)
        {
            if (child is PieDataPoint)
            {
                return ModifyChildrenResult.Accept;
            }

            return base.CanAddChild(child);
        }

        internal override void OnDataPointsModified()
        {
            base.OnDataPointsModified();

            this.isDataPrepared = false;
        }

        private void UpdateDataPoints()
        {
            AngleRange range = this.Range;
            double startAngle = range.startAngle;

            foreach (PieDataPoint point in this.DataPoints)
            {
                if (point.isEmpty)
                {
                    continue;
                }

                point.normalizedValue = point.Value / this.total;
                point.startAngle = startAngle;
                point.sweepAngle = point.normalizedValue * range.sweepAngle;

                if (range.sweepDirection == ChartSweepDirection.Clockwise)
                {
                    startAngle += point.sweepAngle; 
                }
                else
                {
                    startAngle -= point.sweepAngle;
                }
            }
        }

        private void UpdateTotal()
        {
            this.total = 0d;
            this.maxOffset = 0d;

            foreach (PieDataPoint point in this.DataPoints)
            {
                if (point.isEmpty)
                {
                    continue;
                }

                this.total += point.Value;
                this.maxOffset = Math.Max(this.maxOffset, point.OffsetFromCenter);
            }
        }

        private void UpdateMaxOffset()
        {
            this.maxOffset = 0d;

            foreach (PieDataPoint point in this.DataPoints)
            {
                this.maxOffset = Math.Max(this.maxOffset, point.OffsetFromCenter);
            }
        }
    }
}