using System;
using Telerik.Charting;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal class CommodityChannelIndicatorDataSource : HighLowClosePeriodIndicatorDataSourceBase
    {
        protected override void UpdateBinding(DataPointBindingEntry binding)
        {
            this.BindCore();
        }

        protected override void BindCore()
        {
            SizedQueue typicalPrices = new SizedQueue(this.Period);

            int currentIndex = 0;
            foreach (var item in this.itemsSource)
            {
                double high = (double)this.HighBinding.GetValue(item);
                double low  = (double)this.LowBinding.GetValue(item);
                double close = (double)this.CloseBinding.GetValue(item);

                double typicalPrice = (high + low + close) / 3d;
                typicalPrices.EnqueueItem(typicalPrice);

                double currentValue = 0d;

                if (currentIndex > 0)
                {
                    double typicalPriceMA = MovingAverageIndicatorDataSource.CalculateCurrentValue(typicalPrices);

                    double meanDeviation = 0;
                    foreach (var price in typicalPrices)
                    {
                        meanDeviation += Math.Abs(typicalPriceMA - price);
                    }
                    meanDeviation /= typicalPrices.Count;

                    currentValue = (typicalPrice - typicalPriceMA) / 0.015 / meanDeviation;
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

                currentIndex++;
            }
        }
    }
}
