using System;
using Telerik.Charting;
using Telerik.Core;
using Windows.Foundation;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal class PolarSeriesDataSource : ChartSeriesDataSource
    {
        private DataPointBinding valueBinding;
        private DataPointBinding angleBinding;

        public DataPointBinding ValueBinding
        {
            get
            {
                return this.valueBinding;
            }
            set
            {
                if (this.valueBinding == value)
                {
                    return;
                }

                this.valueBinding = value;
                this.valueBinding.PropertyChanged += this.OnBoundItemPropertyChanged;

                if (this.ItemsSource != null)
                {
                    this.Rebind(false, null);
                }
            }
        }

        public DataPointBinding AngleBinding
        {
            get
            {
                return this.angleBinding;
            }
            set
            {
                if (this.angleBinding == value)
                {
                    return;
                }

                this.angleBinding = value;
                this.angleBinding.PropertyChanged += this.OnBoundItemPropertyChanged;

                if (this.ItemsSource != null)
                {
                    this.Rebind(false, null);
                }
            }
        }

        protected override DataPoint CreateDataPoint()
        {
            return new PolarDataPoint();
        }

        protected override void ProcessDouble(DataPoint point, double value)
        {
            (point as PolarDataPoint).Value = value;
        }

        protected override void ProcessDoubleArray(DataPoint point, double[] values)
        {
            PolarDataPoint polarPoint = point as PolarDataPoint;
            if (values.Length > 0)
            {
                polarPoint.Value = values[0];
            }
            if (values.Length > 1)
            {
                polarPoint.Angle = values[1];
            }
        }

        protected override void ProcessNullableDoubleArray(DataPoint point, double?[] values)
        {
            PolarDataPoint polarPoint = point as PolarDataPoint;
            if (values.Length > 0 && values[0].HasValue)
            {
                polarPoint.Value = values[0].Value;
            }
            if (values.Length > 1 && values[1].HasValue)
            {
                polarPoint.Angle = values[1].Value;
            }
        }

        protected override void ProcessSize(DataPoint point, Size size)
        {
            PolarDataPoint polarPoint = point as PolarDataPoint;
            polarPoint.Value = size.Width;
            polarPoint.Angle = size.Height;
        }

        protected override void ProcessPoint(DataPoint dataPoint, Point point)
        {
            PolarDataPoint polarPoint = dataPoint as PolarDataPoint;
            polarPoint.Value = point.X;
            polarPoint.Angle = point.Y;
        }

        protected override void InitializeBinding(DataPointBindingEntry binding)
        {
            PolarDataPoint dataPoint = binding.DataPoint as PolarDataPoint;

            if (this.valueBinding != null)
            {
                object value = this.valueBinding.GetValue(binding.DataItem);
                double doubleValue;
                if (NumericConverter.TryConvertToDouble(value, out doubleValue))
                {
                    dataPoint.Value = doubleValue;
                }
            }

            if (this.angleBinding != null)
            {
                object angle = this.angleBinding.GetValue(binding.DataItem);
                double doubleValue;
                if (NumericConverter.TryConvertToDouble(angle, out doubleValue))
                {
                    dataPoint.Angle = doubleValue;
                }
            }

            base.InitializeBinding(binding);
        }
    }
}
