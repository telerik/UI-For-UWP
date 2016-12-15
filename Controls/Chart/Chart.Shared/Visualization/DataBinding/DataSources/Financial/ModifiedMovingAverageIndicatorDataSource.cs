using Telerik.Charting;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal class ModifiedMovingAverageIndicatorDataSource : ValuePeriodIndicatorDataSourceBase
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
            double sum = 0d;
            double currentSimpleAverage = MovingAverageIndicatorDataSource.CalculateCurrentValue(currentItems);

            for (int i = 0; i < itemsCount; i++)
            {
                sum += (itemsCount - ((2 * i) + 1)) / 2d * currentItems[i];
            }
            return currentSimpleAverage + ((6 * sum) / itemsCount / (itemsCount + 1));
        }

        private void BindCore(int startIndex)
        {
            int period = this.Period;
            int currentIndex = 0;
            double currentAverage = 0d;

            SizedQueue currentItems = new SizedQueue(period);

            foreach (var item in this.itemsSource)
            {
                double value = (double)this.ValueBinding.GetValue(item);
                currentItems.EnqueueItem(value);

                if (currentIndex >= startIndex)
                {
                    if (currentIndex < period - 1)
                    {
                        currentAverage = MovingAverageIndicatorDataSource.CalculateCurrentValue(currentItems);
                    }
                    else
                    {
                        currentAverage = CalculateCurrentValue(currentItems);
                    }

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
