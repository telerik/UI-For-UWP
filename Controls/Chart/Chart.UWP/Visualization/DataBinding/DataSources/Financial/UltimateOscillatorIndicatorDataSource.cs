using System;
using Telerik.Charting;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal class UltimateOscillatorIndicatorDataSource : HighLowClosePeriodIndicatorDataSourceBase
    {
        private int period2;
        private int period3;

        public int Period2
        {
            get
            {
                return this.period2;
            }
            set
            {
                if (this.period2 == value)
                {
                    return;
                }

                this.period2 = value;

                if (this.ItemsSource != null)
                {
                    this.Rebind(false, null);
                }
            }
        }

        public int Period3
        {
            get
            {
                return this.period3;
            }
            set
            {
                if (this.period3 == value)
                {
                    return;
                }

                this.period3 = value;

                if (this.ItemsSource != null)
                {
                    this.Rebind(false, null);
                }
            }
        }

        internal static double CalculateCurrentValue(SizedQueue items)
        {
            return items.RunningSum / items.Count;
        }

        protected override void UpdateBinding(DataPointBindingEntry binding)
        {
            this.BindCore();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "period3", Justification = "Better readability"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "period2", Justification = "Better readability")]
        protected override void BindCore()
        {
            int period = this.Period;
            int period2 = this.Period2;
            int period3 = this.Period3;

            SizedQueue items = new SizedQueue(period);
            SizedQueue items2 = new SizedQueue(period2);
            SizedQueue items3 = new SizedQueue(period3);

            SizedQueue trueRangeItems = new SizedQueue(period);
            SizedQueue trueRangeItems2 = new SizedQueue(period2);
            SizedQueue trueRangeItems3 = new SizedQueue(period3);

            int currentIndex = 0;
            double trueHigh, trueLow, trueRange;
            double high, low, close, range;
            double average1, average2, average3;
            double trueRangeAverage1, trueRangeAverage2, trueRangeAverage3;

            double previousClose = 0;

            foreach (var item in this.itemsSource)
            {
                high = (double)this.HighBinding.GetValue(item);
                low = (double)this.LowBinding.GetValue(item);
                close = (double)this.CloseBinding.GetValue(item);

                if (currentIndex == 0)
                {
                    previousClose = close;
                }

                trueHigh = Math.Max(high, previousClose);
                trueLow = Math.Min(low, previousClose);
                trueRange = trueHigh - trueLow;
                range = close - trueLow;

                items.EnqueueItem(range);
                items2.EnqueueItem(range);
                items3.EnqueueItem(range);

                trueRangeItems.EnqueueItem(trueRange);
                trueRangeItems2.EnqueueItem(trueRange);
                trueRangeItems3.EnqueueItem(trueRange);

                average1 = MovingAverageIndicatorDataSource.CalculateCurrentValue(items);
                average2 = MovingAverageIndicatorDataSource.CalculateCurrentValue(items2);
                average3 = MovingAverageIndicatorDataSource.CalculateCurrentValue(items3);

                trueRangeAverage1 = MovingAverageIndicatorDataSource.CalculateCurrentValue(trueRangeItems);
                trueRangeAverage2 = MovingAverageIndicatorDataSource.CalculateCurrentValue(trueRangeItems2);
                trueRangeAverage3 = MovingAverageIndicatorDataSource.CalculateCurrentValue(trueRangeItems3);

                double value = 100 * ((4 * average1 / trueRangeAverage1) + (2 * average2 / trueRangeAverage2) + (average3 / trueRangeAverage3)) / 7d;

               CategoricalDataPoint point;
                if (this.Owner.Model.DataPointsInternal.Count > currentIndex)
                {
                    point = this.Owner.Model.DataPointsInternal[currentIndex] as CategoricalDataPoint;
                    point.Value = value;
                }
                else
                {
                    point = this.GenerateDataPoint(item, -1) as CategoricalDataPoint;
                    point.Value = value;
                    this.Owner.Model.DataPointsInternal.Add(point);
                }

                previousClose = close;
                currentIndex++;
            }
        }
    }
}
