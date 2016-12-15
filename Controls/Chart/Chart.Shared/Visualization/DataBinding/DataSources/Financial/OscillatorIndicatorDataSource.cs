using Telerik.Charting;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal class OscillatorIndicatorDataSource : ShortLongPeriodIndicatorDataSourceBase
    {
        protected override void BindCore()
        {
            this.BindCore(0);
        }

        protected override void UpdateBinding(DataPointBindingEntry binding)
        {
            this.BindCore(this.Bindings.IndexOf(binding));
        }

        private void BindCore(int startIndex)
        {
            int shortPeriod = this.ShortPeriod;
            int longPeriod = this.LongPeriod;

            SizedQueue currentItemsShort = new SizedQueue(shortPeriod);
            SizedQueue currentItemsLong = new SizedQueue(longPeriod);

            int currentIndex = 0;

            foreach (var item in this.itemsSource)
            {
                double value = (double)this.ValueBinding.GetValue(item);
                currentItemsShort.EnqueueItem(value);
                currentItemsLong.EnqueueItem(value);
                if (currentIndex >= startIndex)
                {
                    double shortAverage = MovingAverageIndicatorDataSource.CalculateCurrentValue(currentItemsShort);
                    double longAverage = MovingAverageIndicatorDataSource.CalculateCurrentValue(currentItemsLong);
                    double currentValue = (shortAverage - longAverage) / shortAverage * 100;
                    CategoricalDataPoint point;
                    if (this.Owner.Model.DataPointsInternal.Count > currentIndex)
                    {
                        point = this.Owner.Model.DataPointsInternal[currentIndex] as CategoricalDataPoint;
                        point.Value = currentValue;
                    }
                    else
                    {
                        point = this.GenerateDataPoint(item, -1) as CategoricalDataPoint;
                        point.Value = currentValue;
                        this.Owner.Model.DataPointsInternal.Add(point);
                    }
                }
                currentIndex++;
            }
        }
    }
}
