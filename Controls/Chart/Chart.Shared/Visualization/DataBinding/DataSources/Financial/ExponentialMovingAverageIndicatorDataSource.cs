using Telerik.Charting;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal class ExponentialMovingAverageIndicatorDataSource : ValuePeriodIndicatorDataSourceBase
    {
        public bool IsModified { get; set; }

        internal static double CalculateCurrentValue(bool isModified, int period, double value, double prevEMA)
        {
            double multiplier = isModified ? 1d / period : 2d / (1 + period);
            return CalculateCurrentValue(multiplier, value, prevEMA);
        }

        protected override void BindCore()
        {
            int period = this.Period;
            SizedQueue currentItems = new SizedQueue(period);

            double currentAverage = 0;
            double prevEMA = 0;
            int currentIndex = 0;
            double multiplier = this.IsModified ? 1d / period : 2d / (1 + period);

            foreach (var item in this.itemsSource)
            {
                double value = (double)this.ValueBinding.GetValue(item);

                //// The first values are calculated as SMA
                if (currentIndex < period)
                {
                    currentItems.EnqueueItem(value);
                    currentAverage = MovingAverageIndicatorDataSource.CalculateCurrentValue(currentItems);
                }
                else
                {
                    currentAverage = CalculateCurrentValue(multiplier, value, prevEMA);
                }

                prevEMA = currentAverage;

                CategoricalDataPoint point;
                if (this.Owner.Model.DataPointsInternal.Count > currentIndex)
                {
                    point = this.Owner.Model.DataPointsInternal[currentIndex] as CategoricalDataPoint;
                    point.Value = currentAverage;
                }
                else
                {
                    point = this.GenerateDataPoint(item, -1) as CategoricalDataPoint;
                    point.Value = currentAverage;
                    this.Owner.Model.DataPointsInternal.Add(point);
                }

                currentIndex++;
            }
        }

        protected override void UpdateBinding(DataPointBindingEntry binding)
        {
            this.BindCore();
        }

        private static double CalculateCurrentValue(double multiplier, double value, double prevEMA)
        {
            return prevEMA + (multiplier * (value - prevEMA));
        }
    }
}
