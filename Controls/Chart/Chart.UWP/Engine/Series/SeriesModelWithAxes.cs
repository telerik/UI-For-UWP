using System;

namespace Telerik.Charting
{
    internal abstract class SeriesModelWithAxes<T> : DataPointSeriesModel<T>, IPlotAreaElementModelWithAxes
        where T : DataPoint
    {
        internal AxisModel firstAxis;
        internal AxisModel secondAxis;

        public AxisModel FirstAxis
        {
            get
            {
                if (this.firstAxis != null)
                {
                    return this.firstAxis;
                }

                var area = this.GetChartArea<ChartAreaModelWithAxes>();
                if (area == null)
                {
                    return null;
                }

                if (area.primaryFirstAxis != null)
                {
                    return area.primaryFirstAxis;
                }

                if (area.FirstAxes.Count > 0)
                {
                    return area.FirstAxes[0];
                }

                return null;
            }
        }

        public AxisModel SecondAxis
        {
            get
            {
                if (this.secondAxis != null)
                {
                    return this.secondAxis;
                }

                var area = this.GetChartArea<ChartAreaModelWithAxes>();
                if (area == null)
                {
                    return null;
                }

                if (area.primarySecondAxis != null)
                {
                    return area.primarySecondAxis;
                }

                if (area.SecondAxes.Count > 0)
                {
                    return area.SecondAxes[0];
                }

                return null;
            }
        }

        public virtual void AttachAxis(AxisModel axis, AxisType type)
        {
            if (type == AxisType.First)
            {
                this.firstAxis = axis;
            }
            else
            {
                this.secondAxis = axis;
            }
        }

        public void DetachAxis(AxisModel axis)
        {
            if (this.firstAxis == axis)
            {
                this.firstAxis = null;
            }
            else if (this.secondAxis == axis)
            {
                this.secondAxis = null;
            }
        }

        internal override bool GetIsPlotInverse(AxisPlotDirection plotDirection)
        {
            var axisModel = ((plotDirection & AxisPlotDirection.Horizontal) == AxisPlotDirection.Horizontal) ? this.FirstAxis : this.SecondAxis;

            return axisModel != null ? axisModel.IsInverse : false;
        }
    }
}