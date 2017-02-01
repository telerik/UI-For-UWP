using Telerik.Charting;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal class TrixIndicatorDataSource : ValuePeriodIndicatorDataSourceBase
    {
        protected override void BindCore()
        {
            int period = this.Period;
            SizedQueue currentItems = new SizedQueue(period);
            SizedQueue emaOneItems = new SizedQueue(period);
            SizedQueue emaTwoItems = new SizedQueue(period);

            int currentIndex = 0;
            double emaOne, emaTwo, emaThree;
            double lastEmaOne = 0;
            double lastEmaTwo = 0;
            double lastEmaThree = 0;
            double currentValue;

            foreach (var item in this.itemsSource)
            {
                double value = (double)this.ValueBinding.GetValue(item);
                currentItems.EnqueueItem(value);

                if (currentIndex < period)
                {
                    emaOne = MovingAverageIndicatorDataSource.CalculateCurrentValue(currentItems);
                    emaOneItems.EnqueueItem(emaOne);
                    emaTwo = MovingAverageIndicatorDataSource.CalculateCurrentValue(emaOneItems);
                    emaTwoItems.EnqueueItem(emaTwo);
                    emaThree = MovingAverageIndicatorDataSource.CalculateCurrentValue(emaTwoItems);
                }
                else
                {
                    emaOne = ExponentialMovingAverageIndicatorDataSource.CalculateCurrentValue(false, period, value, lastEmaOne);
                    emaOneItems.EnqueueItem(emaOne);
                    emaTwo = ExponentialMovingAverageIndicatorDataSource.CalculateCurrentValue(false, period, emaOne, lastEmaTwo);
                    emaTwoItems.EnqueueItem(emaTwo);
                    emaThree = ExponentialMovingAverageIndicatorDataSource.CalculateCurrentValue(false, period, emaTwo, lastEmaThree);
                }

                if (currentIndex == 0)
                {
                    currentValue = 0;
                }
                else
                {
                    currentValue = 100 * (emaThree - lastEmaThree) / emaThree;
                }

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

                lastEmaOne = emaOne;
                lastEmaTwo = emaTwo;
                lastEmaThree = emaThree;
                currentIndex++;
            }
        }

        protected override void UpdateBinding(DataPointBindingEntry binding)
        {
            this.BindCore();
        }
    }
}
