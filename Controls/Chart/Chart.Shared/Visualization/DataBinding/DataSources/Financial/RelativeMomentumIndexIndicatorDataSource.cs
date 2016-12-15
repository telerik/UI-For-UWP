using System;
using Telerik.Charting;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal class RelativeMomentumIndexIndicatorDataSource : ValuePeriodIndicatorDataSourceBase
    {
        private int momentumPeriod;

        public int MomentumPeriod
        {
            get
            {
                return this.momentumPeriod;
            }
            set
            {
                if (this.momentumPeriod == value)
                {
                    return;
                }

                this.momentumPeriod = value;

                if (this.ItemsSource != null)
                {
                    this.Rebind(false, null);
                }
            }
        }

        protected override void UpdateBinding(DataPointBindingEntry binding)
        {
            //// TODO: Optimize this to update only the effected points.
            this.BindCore();
        }

        protected override void BindCore()
        {
            int currentIndex = 0;

            int period = this.Period;
            int momentumShift = this.MomentumPeriod;

            SizedQueue values = new SizedQueue(momentumShift);
            SizedQueue upMomentumValues = new SizedQueue(period);
            SizedQueue downMomentumValues = new SizedQueue(period);

            double oldValue, value, currentIndicatorValue;
            double upMomentumAverage, downMomentumAverage;
            double up, down;

            foreach (var item in this.itemsSource)
            {
                value = (double)this.ValueBinding.GetValue(item);

                if (currentIndex == 0)
                {
                    oldValue = value;
                }
                else if (currentIndex < momentumShift)
                {
                    oldValue = values.Peek();
                }
                else
                {
                    oldValue = values.DequeueItem();
                }

                if (oldValue > value)
                {
                    up = 0;
                    down = oldValue - value;
                }
                else
                {
                    up = value - oldValue;
                    down = 0;
                }

                upMomentumValues.EnqueueItem(up);
                downMomentumValues.EnqueueItem(down);

                upMomentumAverage = MovingAverageIndicatorDataSource.CalculateCurrentValue(upMomentumValues);
                downMomentumAverage = MovingAverageIndicatorDataSource.CalculateCurrentValue(downMomentumValues);

                if (Math.Round(upMomentumAverage + downMomentumAverage, 6) == 0)
                {
                    currentIndicatorValue = 100;
                }
                else
                {
                    currentIndicatorValue = 100 * upMomentumAverage / (upMomentumAverage + downMomentumAverage);
                }

                values.EnqueueItem(value);

                CategoricalDataPoint point;
                if (this.Owner.Model.DataPointsInternal.Count > currentIndex)
                {
                    point = this.Owner.Model.DataPointsInternal[currentIndex] as CategoricalDataPoint;
                    point.Value = currentIndicatorValue;
                }
                else
                {
                    point = this.GenerateDataPoint(item, -1) as CategoricalDataPoint;
                    point.Value = currentIndicatorValue;
                    this.Owner.Model.DataPointsInternal.Add(point);
                }

                currentIndex++;
            }
        }
    }
}
