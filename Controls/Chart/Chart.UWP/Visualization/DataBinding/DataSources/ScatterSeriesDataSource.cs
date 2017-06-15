using System;
using Telerik.Charting;
using Telerik.Core;
using Windows.Foundation;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal class ScatterSeriesDataSource : ChartSeriesDataSource
    {
        private DataPointBinding xValueBinding;
        private DataPointBinding yValueBinding;

        public DataPointBinding XValueBinding
        {
            get
            {
                return this.xValueBinding;
            }
            set
            {
                if (this.xValueBinding == value)
                {
                    return;
                }

                this.xValueBinding = value;
                this.xValueBinding.PropertyChanged += this.OnBoundItemPropertyChanged;

                if (this.ItemsSource != null)
                {
                    this.Rebind(false, null);
                }
            }
        }

        public DataPointBinding YValueBinding
        {
            get
            {
                return this.yValueBinding;
            }
            set
            {
                if (this.xValueBinding == value)
                {
                    return;
                }

                this.yValueBinding = value;
                this.yValueBinding.PropertyChanged += this.OnBoundItemPropertyChanged;

                if (this.ItemsSource != null)
                {
                    this.Rebind(false, null);
                }
            }
        }

        protected override DataPoint CreateDataPoint()
        {
            return new ScatterDataPoint();
        }

        protected override void ProcessDouble(DataPoint point, double value)
        {
            (point as ScatterDataPoint).XValue = value;
        }

        protected override void ProcessDoubleArray(DataPoint point, double[] values)
        {
            ScatterDataPoint scatterPoint = point as ScatterDataPoint;
            if (values.Length > 0)
            {
                scatterPoint.XValue = values[0];
            }
            if (values.Length > 1)
            {
                scatterPoint.YValue = values[1];
            }
        }

        protected override void ProcessNullableDoubleArray(DataPoint point, double?[] values)
        {
            ScatterDataPoint scatterPoint = point as ScatterDataPoint;
            if (values.Length > 0 && values[0].HasValue)
            {
                scatterPoint.XValue = values[0].Value;
            }
            if (values.Length > 1 && values[1].HasValue)
            {
                scatterPoint.YValue = values[1].Value;
            }
        }

        protected override void ProcessSize(DataPoint point, Size size)
        {
            ScatterDataPoint scatterPoint = point as ScatterDataPoint;
            scatterPoint.XValue = size.Width;
            scatterPoint.YValue = size.Height;
        }

        protected override void ProcessPoint(DataPoint dataPoint, Point point)
        {
            ScatterDataPoint scatterPoint = dataPoint as ScatterDataPoint;
            scatterPoint.XValue = point.X;
            scatterPoint.YValue = point.Y;
        }

        protected override void InitializeBinding(DataPointBindingEntry binding)
        {
            ScatterDataPoint dataPoint = binding.DataPoint as ScatterDataPoint;

            if (this.xValueBinding != null)
            {
                object xValue = this.xValueBinding.GetValue(binding.DataItem);
                double doubleValue;
                if (NumericConverter.TryConvertToDouble(xValue, out doubleValue))
                {
                    dataPoint.XValue = doubleValue;
                }
            }

            if (this.yValueBinding != null)
            {
                object yValue = this.yValueBinding.GetValue(binding.DataItem);
                double doubleValue;
                if (NumericConverter.TryConvertToDouble(yValue, out doubleValue))
                {
                    dataPoint.YValue = doubleValue;
                }
            }

            base.InitializeBinding(binding);
        }
    }
}
