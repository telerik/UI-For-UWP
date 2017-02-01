using System;
using Telerik.Charting;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal class MacdIndicatorDataSource : ShortLongPeriodIndicatorDataSourceBase
    {
        private int signalPeriod;

        private double currentLongEMA, currentShortEMA;

        public int SignalPeriod
        {
            get
            {
                return this.signalPeriod;
            }
            set
            {
                if (this.signalPeriod == value)
                {
                    return;
                }

                this.signalPeriod = value;

                if (this.ItemsSource != null)
                {
                    this.Rebind(false, null);
                }
            }
        }

        protected static double CalculateSignal(int signalPeriod, SizedQueue signalPeriodItems, int currentIndex, double signalEMA, double macd)
        {
            if (currentIndex < signalPeriod)
            {
                signalEMA = MovingAverageIndicatorDataSource.CalculateCurrentValue(signalPeriodItems);
            }
            else
            {
                signalEMA = ExponentialMovingAverageIndicatorDataSource.CalculateCurrentValue(false, signalPeriod, macd, signalEMA);
            }
            return signalEMA;
        }

        protected override void UpdateBinding(DataPointBindingEntry binding)
        {
            this.BindCore();
        }

        protected override void BindCore()
        {
            SizedQueue longPeriodItems = new SizedQueue(this.LongPeriod);
            SizedQueue shortPeriodItems = new SizedQueue(this.ShortPeriod);
            SizedQueue signalPeriodItems = new SizedQueue(this.signalPeriod);

            this.GenerateDataPoints(longPeriodItems, shortPeriodItems, signalPeriodItems);
        }

        protected virtual void GenerateDataPoints(SizedQueue longPeriodItems, SizedQueue shortPeriodItems, SizedQueue signalPeriodItems)
        {
            ChartSeriesModel model = this.Owner.Model;
            ChartSeriesModel signalModel = (this.Owner as MacdIndicator).SignalModel;

            int currentIndex = 0;
            CategoricalDataPoint point, point2;
            double signalEMA = 0d;
            double macd = 0d;

            foreach (var item in this.itemsSource)
            {
                macd = this.CalculateMacdValue(longPeriodItems, shortPeriodItems, currentIndex, item);
                signalPeriodItems.EnqueueItem(macd);

                signalEMA = CalculateSignal(this.signalPeriod, signalPeriodItems, currentIndex, signalEMA, macd);

                if (model.DataPointsInternal.Count > currentIndex)
                {
                    point = model.DataPointsInternal[currentIndex] as CategoricalDataPoint;
                    point.Value = macd;
                }
                else
                {
                    point = this.GenerateDataPoint(item, -1) as CategoricalDataPoint;
                    point.Value = macd;
                    model.DataPointsInternal.Add(point);
                }

                if (signalModel.DataPointsInternal.Count > currentIndex)
                {
                    point2 = signalModel.DataPointsInternal[currentIndex] as CategoricalDataPoint;
                    point2.Value = signalEMA;
                }
                else
                {
                    point2 = this.GenerateDataPoint(item, -1) as CategoricalDataPoint;
                    point2.Value = signalEMA;
                    signalModel.DataPointsInternal.Add(point2);
                }

                currentIndex++;
            }
        }

        protected double CalculateMacdValue(SizedQueue longPeriodItems, SizedQueue shortPeriodItems, int currentIndex, object item)
        {
            double value = (double)this.ValueBinding.GetValue(item);
            longPeriodItems.EnqueueItem(value);
            shortPeriodItems.EnqueueItem(value);

            if (currentIndex < this.LongPeriod)
            {
                this.currentLongEMA = MovingAverageIndicatorDataSource.CalculateCurrentValue(longPeriodItems);
            }
            else
            {
                this.currentLongEMA = ExponentialMovingAverageIndicatorDataSource.CalculateCurrentValue(false, this.LongPeriod, value, this.currentLongEMA);
            }

            if (currentIndex < this.ShortPeriod)
            {
                this.currentShortEMA = MovingAverageIndicatorDataSource.CalculateCurrentValue(shortPeriodItems);
            }
            else
            {
                this.currentShortEMA = ExponentialMovingAverageIndicatorDataSource.CalculateCurrentValue(false, this.ShortPeriod, value, this.currentShortEMA);
            }

            return this.currentShortEMA - this.currentLongEMA;
        }

        protected override void Unbind()
        {
            base.Unbind();

            MacdIndicator owner = this.Owner as MacdIndicator;
            if (owner != null)
            {
                owner.signalModel.DataPointsInternal.Clear();
            }
        }
    }
}
