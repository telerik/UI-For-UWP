using System;
using System.Linq;
using Telerik.Charting;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal class AdaptiveMovingAverageKaufmanIndicatorDataSource : ValuePeriodIndicatorDataSourceBase
    {
        private int slowPeriod;
        private int fastPeriod; 

        public int SlowPeriod
        {
            get
            {
                return this.slowPeriod;
            }
            set
            {
                if (this.slowPeriod == value)
                {
                    return;
                }

                this.slowPeriod = value;

                if (this.ItemsSource != null)
                {
                    this.Rebind(false, null);
                }
            }
        }

        public int FastPeriod
        {
            get
            {
                return this.fastPeriod;
            }
            set
            {
                if (this.fastPeriod == value)
                {
                    return;
                }

                this.fastPeriod = value;

                if (this.ItemsSource != null)
                {
                    this.Rebind(false, null);
                }
            }
        }

        protected override void UpdateBinding(DataPointBindingEntry binding)
        {
            this.BindCore();
        }

        protected override void BindCore()
        {
            int period = this.Period;

            double currentAverage = 0;
            double prevKAMA = 0;
            int currentIndex = 0;

            SizedQueue currentItems = new SizedQueue(period + 1);

            foreach (var item in this.itemsSource)
            {
                double value = (double)this.ValueBinding.GetValue(item);
                currentItems.EnqueueItem(value);

                //// The raw value is used for the first elements
                if (currentIndex < period)
                {
                    currentAverage = value;
                    prevKAMA = value;
                }
                else
                {
                    currentAverage = CalculateCurrentValue(currentItems, this.slowPeriod, this.fastPeriod, prevKAMA);
                    prevKAMA = currentAverage;
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
                currentIndex++;
            }
        }

        private static double CalculateCurrentValue(SizedQueue currentItems, int slow, int fast, double prevKAMA)
        {
            int period = currentItems.Size;

            double diff = Math.Abs(currentItems[period - 1] - currentItems[0]);
            double rangesSum = 0;
            for (int i = 1; i < currentItems.Count; i++)
            {
                rangesSum += Math.Abs(currentItems[i] - currentItems[i - 1]);
            }
            double efficiencyRatio = diff / rangesSum;

            double fastConstant = 2d / (fast + 1);
            double slowConstant = 2d / (slow + 1);
            double weight = Math.Pow((efficiencyRatio * (fastConstant - slowConstant)) + slowConstant, 2);

            return prevKAMA + (weight * (currentItems.Last() - prevKAMA));
        }
    }
}
