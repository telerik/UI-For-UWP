using Telerik.Charting;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal class WeightedMovingAverageIndicatorDataSource : ValuePeriodIndicatorDataSourceBase
    {
        protected override void BindCore()
        {
            this.BindCore(0);
        }

        protected override void UpdateBinding(DataPointBindingEntry binding)
        {
            this.BindCore(this.Bindings.IndexOf(binding));
        }

        private static double CalculateCurrentValue(SizedQueue currentItems)
        {
            int itemsCount = currentItems.Count;

            double weightedSum = 0;
            for (int i = 0; i < itemsCount; i++)
            {
                weightedSum += currentItems[i] * (i + 1);
            }
            double divider = itemsCount * (itemsCount + 1) / 2;

            double currentAverage = weightedSum / divider;
            return currentAverage;
        }

        private void BindCore(int startIndex)
        {
            int period = this.Period;
            SizedQueue currentItems = new SizedQueue(period);

            int currentIndex = 0;

            foreach (var item in this.itemsSource)
            {
                object val = this.ValueBinding.GetValue(item);
                currentItems.EnqueueItem((double)val);

                if (currentIndex >= startIndex)
                {
                    double currentAverage = CalculateCurrentValue(currentItems);
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
                }
                currentIndex++;
            }
        }
    }
}
