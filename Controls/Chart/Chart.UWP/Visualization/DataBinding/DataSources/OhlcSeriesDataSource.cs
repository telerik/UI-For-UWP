using System;
using Telerik.Charting;
using Telerik.Core;
using Windows.Foundation;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal class OhlcSeriesDataSource : CategoricalSeriesDataSourceBase
    {
        private DataPointBinding highBinding;
        private DataPointBinding lowBinding;
        private DataPointBinding openBinding;
        private DataPointBinding closeBinding;

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

        public DataPointBinding OpenBinding
        {
            get
            {
                return this.openBinding;
            }
            set
            {
                if (this.openBinding == value)
                {
                    return;
                }

                this.openBinding = value;
                this.openBinding.PropertyChanged += this.OnBoundItemPropertyChanged;

                if (this.ItemsSource != null)
                {
                    this.Rebind(false, null);
                }
            }
        }

        public DataPointBinding CloseBinding
        {
            get
            {
                return this.closeBinding;
            }
            set
            {
                if (this.closeBinding == value)
                {
                    return;
                }

                this.closeBinding = value;
                this.closeBinding.PropertyChanged += this.OnBoundItemPropertyChanged;

                if (this.ItemsSource != null)
                {
                    this.Rebind(false, null);
                }
            }
        }

        protected override DataPoint CreateDataPoint()
        {
            return new OhlcDataPoint();
        }

        protected override void ProcessDouble(DataPoint point, double value)
        {
            throw new NotSupportedException();
        }

        protected override void ProcessDoubleArray(DataPoint point, double[] values)
        {
            throw new NotSupportedException();
        }

        protected override void ProcessNullableDoubleArray(DataPoint point, double?[] values)
        {
            throw new NotImplementedException();
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
            OhlcDataPoint dataPoint = binding.DataPoint as OhlcDataPoint;

            if (this.highBinding != null)
            {
                object value = this.highBinding.GetValue(binding.DataItem);
                double doubleValue;
                if (NumericConverter.TryConvertToDouble(value, out doubleValue))
                {
                    dataPoint.High = doubleValue;
                }
            }

            if (this.lowBinding != null)
            {
                object value = this.lowBinding.GetValue(binding.DataItem);
                double doubleValue;
                if (NumericConverter.TryConvertToDouble(value, out doubleValue))
                {
                    dataPoint.Low = doubleValue;
                }
            }

            if (this.openBinding != null)
            {
                object value = this.openBinding.GetValue(binding.DataItem);
                double doubleValue;
                if (NumericConverter.TryConvertToDouble(value, out doubleValue))
                {
                    dataPoint.Open = doubleValue;
                }
            }

            if (this.closeBinding != null)
            {
                object value = this.closeBinding.GetValue(binding.DataItem);
                double doubleValue;
                if (NumericConverter.TryConvertToDouble(value, out doubleValue))
                {
                    dataPoint.Close = doubleValue;
                }
            }

            base.InitializeBinding(binding);
        }
    }
}
