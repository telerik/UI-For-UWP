using Telerik.Charting;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal class MovingAverageIndicatorDataSource : ValuePeriodIndicatorDataSourceBase
    {
        internal static double CalculateCurrentValue(SizedQueue items)
        {
            return items.RunningSum / items.Count;
        }

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
