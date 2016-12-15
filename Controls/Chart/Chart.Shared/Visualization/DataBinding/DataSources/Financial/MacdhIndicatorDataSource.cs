using System;
using System.ComponentModel;
using Telerik.Charting;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal class MacdhIndicatorDataSource : CategoricalSeriesDataSource
    {
        private int longPeriod;
        private int shortPeriod;
        private int signalPeriod;

        private double currentLongEMA, currentShortEMA;

        public int LongPeriod
        {
            get
            {
                return this.longPeriod;
            }
            set
            {
                if (this.longPeriod == value)
                {
                    return;
                }

                this.longPeriod = value;

                if (this.ItemsSource != null)
                {
                    this.Rebind(false, null);
                }
            }
        }

        public int ShortPeriod
        {
            get
            {
                return this.shortPeriod;
            }
            set
            {
                if (this.shortPeriod == value)
                {
                    return;
                }

                this.shortPeriod = value;

                if (this.ItemsSource != null)
                {
                    this.Rebind(false, null);
                }
            }
        }

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
            SizedQueue longPeriodItems = new SizedQueue(this.longPeriod);
            SizedQueue shortPeriodItems = new SizedQueue(this.shortPeriod);
            SizedQueue signalPeriodItems = new SizedQueue(this.signalPeriod);

            this.GenerateDataPoints(longPeriodItems, shortPeriodItems, signalPeriodItems);
        }

        protected virtual void GenerateDataPoints(SizedQueue longPeriodItems, SizedQueue shortPeriodItems, SizedQueue signalPeriodItems)
        {
            ChartSeriesModel model = this.Owner.Model;

            int currentIndex = 0;
            CategoricalDataPoint point;
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
                    point.Value = macd - signalEMA;
                }
                else
                {
                    point = this.GenerateDataPoint(item, -1) as CategoricalDataPoint;
                    point.Value = macd - signalEMA;
                    model.DataPointsInternal.Add(point);
                }

                currentIndex++;
            }
        }

        protected double CalculateMacdValue(SizedQueue longPeriodItems, SizedQueue shortPeriodItems, int currentIndex, object item)
        {
            double value = (double)this.ValueBinding.GetValue(item);
            longPeriodItems.EnqueueItem(value);
            shortPeriodItems.EnqueueItem(value);

            if (currentIndex < this.longPeriod)
            {
                this.currentLongEMA = MovingAverageIndicatorDataSource.CalculateCurrentValue(longPeriodItems);
            }
            else
            {
                this.currentLongEMA = ExponentialMovingAverageIndicatorDataSource.CalculateCurrentValue(false, this.longPeriod, value, this.currentLongEMA);
            }

            if (currentIndex < this.shortPeriod)
            {
                this.currentShortEMA = MovingAverageIndicatorDataSource.CalculateCurrentValue(shortPeriodItems);
            }
            else
            {
                this.currentShortEMA = ExponentialMovingAverageIndicatorDataSource.CalculateCurrentValue(false, this.shortPeriod, value, this.currentShortEMA);
            }

            return this.currentShortEMA - this.currentLongEMA;
        }

        /// <summary>
        /// Called when a property of a bound object changes.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs" /> instance containing the event data.</param>
        protected override void OnBoundItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var propertyNameValueBinding = (this.Owner as MacdhIndicator).ValueBinding as PropertyNameDataPointBinding;
            var propertyNameCategoryBinding = (this.Owner as MacdhIndicator).CategoryBinding as PropertyNameDataPointBinding;

            if (propertyNameValueBinding != null && propertyNameCategoryBinding != null &&
                e.PropertyName != propertyNameValueBinding.PropertyName && e.PropertyName != propertyNameCategoryBinding.PropertyName)
            {
                return;
            }

            base.OnBoundItemPropertyChanged(sender, e);
        }
    }
}