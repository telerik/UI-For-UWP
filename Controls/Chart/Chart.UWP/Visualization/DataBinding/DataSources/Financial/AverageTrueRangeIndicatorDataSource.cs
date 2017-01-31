using Telerik.Charting;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal class AverageTrueRangeIndicatorDataSource : HighLowClosePeriodIndicatorDataSourceBase
    {
        protected override void UpdateBinding(DataPointBindingEntry binding)
        {
            this.BindCore();
        }

        protected override void BindCore()
        {
            int period = this.Period;

            SizedQueue currentItems = new SizedQueue(period);

            int currentIndex = 0;
            object previousItem = null;
            double value;
            foreach (var item in this.itemsSource)
            {
                value = TrueRangeIndicatorDataSource.CalculateValue(this.HighBinding, this.LowBinding, this.CloseBinding, previousItem, item);

                currentItems.EnqueueItem(value);

                double currentValue = MovingAverageIndicatorDataSource.CalculateCurrentValue(currentItems);

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

                currentIndex++;
                previousItem = item;
            }
        }
    }
}
