using System;
using Telerik.Core;

namespace Telerik.Charting
{
    internal class LogarithmicAxisModel : LinearAxisModel
    {
        internal static readonly int LogarithmBasePropertyKey = PropertyKeys.Register(typeof(LogarithmicAxisModel), "LogarithmBase", ChartAreaInvalidateFlags.All);

        // cache value to prevent extensive looking-up in the property store
        private double logBase;

        public LogarithmicAxisModel()
        {
            this.logBase = 10;
        }

        /// <summary>
        /// Gets or sets the base of the logarithm used for normalizing data points' values.
        /// </summary>
        public double LogarithmBase
        {
            get
            {
                return this.logBase;
            }
            set
            {
                this.SetValue(LogarithmBasePropertyKey, value);
            }
        }

        internal override double CalculateAutoStep(ValueRange<double> range)
        {
            if (this.IsLocalValue(NumericalAxisModel.DesiredTickCountPropertyKey))
            {
                double step = (range.maximum - range.minimum) / (this.DesiredTickCount - 1);
                return NumericalAxisModel.NormalizeStep(step);
            }

            return 1d;
        }

        internal override double TransformValue(double value)
        {
            if (value <= 0)
            {
                return double.NaN;
            }

            if (this.logBase == 10)
            {
                // Log10 gives higher precision
                return Math.Log10(value);
            }

            return Math.Log(value, this.logBase);
        }

        internal override Ohlc TransformValue(Ohlc value)
        {
            Ohlc result = new Ohlc();
            result.High = this.TransformValue(value.High);
            result.Low = this.TransformValue(value.Low);
            result.Open = this.TransformValue(value.Open);
            result.Close = this.TransformValue(value.Close);

            return result;
        }

        internal override Range TransformValue(Range value)
        {
            Range result = new Range();
            result.High = this.TransformValue(value.High);
            result.Low = this.TransformValue(value.Low);

            return result;
        }

        internal override double ReverseTransformValue(double value)
        {
            return Math.Pow(this.logBase, value);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Method is internal")]
        internal override void OnPropertyChanged(RadPropertyEventArgs e)
        {
            // update local value first and then call base to raise the PropertyChanged event (if needed)
            if (e.Key == LogarithmBasePropertyKey)
            {
                this.logBase = (double)e.NewValue;
            }

            base.OnPropertyChanged(e);
        }
    }
}