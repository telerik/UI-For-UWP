using System;
using Telerik.Charting;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal class RelativeStrengthIndexIndicatorDataSource : ValuePeriodIndicatorDataSourceBase
    {
        protected override void BindCore()
        {
            int period = this.Period;
            SizedQueue losses = new SizedQueue(period);
            SizedQueue gains = new SizedQueue(period);

            double prevValue = 0;
            double currentValue = 0;
            double lossesAverage = 0;
            double gainsAverage = 0;
            int currentIndex = 0;
            foreach (var item in this.itemsSource)
            {
                double value = (double)this.ValueBinding.GetValue(item);
                double difference = 0d;
                if (currentIndex > 0)
                {
                    difference = Math.Abs(value - prevValue);
                }

                double gain = 0;
                double loss = 0;

                if (value > prevValue)
                {
                    gain = difference;
                    loss = 0d;
                }
                else
                {
                    gain = 0d;
                    loss = difference;
                }

                gains.EnqueueItem(gain);
                losses.EnqueueItem(loss);

                if (currentIndex < period)
                {
                    lossesAverage = MovingAverageIndicatorDataSource.CalculateCurrentValue(losses);
                    gainsAverage = MovingAverageIndicatorDataSource.CalculateCurrentValue(gains);
                }
                else
                {
                    gainsAverage = ExponentialMovingAverageIndicatorDataSource.CalculateCurrentValue(false, period, gain, gainsAverage);
                    lossesAverage = ExponentialMovingAverageIndicatorDataSource.CalculateCurrentValue(false, period, loss, lossesAverage);

                    currentValue = 100 - (100 / (1 + (gainsAverage / lossesAverage)));
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

                prevValue = value;
                currentIndex++;
            }
        }

        protected override void UpdateBinding(DataPointBindingEntry binding)
        {
            this.BindCore();
        }
    }
}
