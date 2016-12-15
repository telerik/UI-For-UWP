using System;
using Telerik.Charting;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal class BollingerBandsIndicatorDataSource : ValuePeriodIndicatorDataSourceBase
    {
        private int standardDeviations;

        public int StandardDeviations
        {
            get
            {
                return this.standardDeviations;
            }
            set
            {
                if (this.standardDeviations == value)
                {
                    return;
                }

                this.standardDeviations = value;

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
            BollingerBandsIndicator indicator = this.Owner as BollingerBandsIndicator;
            int period = this.Period;
            double deviations = this.StandardDeviations;
            SizedQueue currentItems = new SizedQueue(period);

            int currentIndex = 0;
            double stdDeviation = 0;

            foreach (var item in this.itemsSource)
            {
                object val = this.ValueBinding.GetValue(item);
                currentItems.EnqueueItem((double)val);
                double currentAverage = MovingAverageIndicatorDataSource.CalculateCurrentValue(currentItems);

                // TODO: Nested loop, possible performance degrade for large data
                stdDeviation = CalculateStandardDeviation(currentItems, currentAverage);

                CategoricalDataPoint point, secondPoint;
                if (indicator.Model.DataPointsInternal.Count > currentIndex)
                {
                    point = this.Owner.Model.DataPointsInternal[currentIndex] as CategoricalDataPoint;
                    point.Value = currentAverage + (deviations * stdDeviation);

                    secondPoint = indicator.lowerBandModel.DataPointsInternal[currentIndex] as CategoricalDataPoint;
                    secondPoint.Value = currentAverage - (deviations * stdDeviation);
                }
                else
                {
                    point = this.GenerateDataPoint(item, -1) as CategoricalDataPoint;
                    point.Value = currentAverage + (deviations * stdDeviation);
                    indicator.model.DataPointsInternal.Add(point);

                    secondPoint = this.GenerateDataPoint(item, -1) as CategoricalDataPoint;
                    secondPoint.Value = currentAverage - (deviations * stdDeviation);
                    indicator.lowerBandModel.DataPointsInternal.Add(secondPoint);
                }

                currentIndex++;
            }
        }

        protected override void Unbind()
        {
            base.Unbind();
            (this.Owner as BollingerBandsIndicator).lowerBandModel.DataPointsInternal.Clear();
        }

        private static double CalculateStandardDeviation(SizedQueue items, double average)
        {
            double sum = 0;
            for (int i = 0; i < items.Count; i++)
            {
                double item = items[i];
                double entry = Math.Pow(item - average, 2);
                sum += entry;
            }

            return Math.Sqrt(sum / items.Size);
        }
    }
}
