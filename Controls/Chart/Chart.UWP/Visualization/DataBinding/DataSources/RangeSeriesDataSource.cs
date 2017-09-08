using System;
using Telerik.Charting;
using Telerik.Core;
using Windows.Foundation;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal class RangeSeriesDataSource : CategoricalSeriesDataSource
    {
        private DataPointBinding lowBinding;
        private DataPointBinding highBinding;

        public DataPointBinding LowBinding
        {
            get
            {
                return this.lowBinding;
            }
            set
            {
                if (this.lowBinding == value)
                {
                    return;
                }

                this.lowBinding = value;
                this.lowBinding.PropertyChanged += this.OnBoundItemPropertyChanged;

                if (this.ItemsSource != null)
                {
                    this.Rebind(false, null);
                }
            }
        }

        public DataPointBinding HighBinding
        {
            get
            {
                return this.highBinding;
            }
            set
            {
                if (this.highBinding == value)
                {
                    return;
                }

                this.highBinding = value;
                this.highBinding.PropertyChanged += this.OnBoundItemPropertyChanged;

                if (this.ItemsSource != null)
                {
                    this.Rebind(false, null);
                }
            }
        }

        protected override DataPoint CreateDataPoint()
        {
            return new RangeDataPoint();
        }

        protected override void ProcessDouble(DataPoint point, double value)
        {
            if (!double.IsNaN(value))
            {
                RangeDataPoint rangeDataPoint = (RangeDataPoint)point;
                rangeDataPoint.Low = Math.Min(0, value);
                rangeDataPoint.High = Math.Max(0, value);
            }
        }

        protected override void ProcessDoubleArray(DataPoint point, double[] values)
        {
            if (values.Length == 2)
            {
                RangeDataPoint rangeDataPoint = (RangeDataPoint)point;
                rangeDataPoint.Low = values[0];
                rangeDataPoint.High = Math.Max(values[1], rangeDataPoint.Low);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        protected override void ProcessNullableDoubleArray(DataPoint point, double?[] values)
        {
            if (values.Length == 2)
            {
                RangeDataPoint rangeDataPoint = (RangeDataPoint)point;
                if (values[0].HasValue)
                {
                    rangeDataPoint.Low = values[0].Value;
                }

                if (values[1].HasValue)
                {
                    rangeDataPoint.High = Math.Max(values[1].Value, rangeDataPoint.Low);
                }
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        protected override void ProcessPoint(DataPoint dataPoint, Point point)
        {
            throw new NotSupportedException();
        }

        protected override void ProcessSize(DataPoint point, Size size)
        {
            throw new NotSupportedException();
        }

        protected override void InitializeBinding(DataPointBindingEntry binding)
        {
            bool highIsValidNumber = true;
            bool lowIsValidNumber = true;

            RangeDataPoint rangeDataPoint = (RangeDataPoint)binding.DataPoint;

            if (this.highBinding != null)
            {
                object value = this.highBinding.GetValue(binding.DataItem);
                double doubleValue;
                if (NumericConverter.TryConvertToDouble(value, out doubleValue))
                {
                    rangeDataPoint.High = doubleValue;
                }
                else
                {
                    rangeDataPoint.High = 0d;
                    highIsValidNumber = false;
                }
            }

            if (this.lowBinding != null)
            {
                object value = this.lowBinding.GetValue(binding.DataItem);
                double doubleValue;
                if (NumericConverter.TryConvertToDouble(value, out doubleValue))
                {
                    rangeDataPoint.Low = doubleValue;
                }
                else
                {
                    rangeDataPoint.Low = 0d;
                    lowIsValidNumber = false;
                }
            }

            rangeDataPoint.isEmpty = !lowIsValidNumber && !highIsValidNumber;

            base.InitializeBinding(binding);
        }
    }
}
