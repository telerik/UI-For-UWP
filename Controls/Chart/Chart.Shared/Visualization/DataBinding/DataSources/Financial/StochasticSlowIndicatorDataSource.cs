using System.Linq;
using Telerik.Charting;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal class StochasticSlowIndicatorDataSource : StochasticIndicatorDataSourceBase
    {
        private int slowingPeriod;

        public int SlowingPeriod
        {
            get
            {
                return this.slowingPeriod;
            }
            set
            {
                if (this.slowingPeriod == value)
                {
                    return;
                }

                this.slowingPeriod = value;

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
            int currentIndex = 0;

            StochasticSlowIndicator owner = this.Owner as StochasticSlowIndicator;
            ChartSeriesModel model = this.Owner.Model;
            ChartSeriesModel signalModel = owner.SignalModel;

            int currentMainPeriod = this.MainPeriod;
            int currentSlowingPeriod = this.SlowingPeriod;
            int currentSignalPeriod = this.SignalPeriod;

            SizedQueue highValues = new SizedQueue(currentMainPeriod);
            SizedQueue lowValues = new SizedQueue(currentMainPeriod);
            SizedQueue fastStochValues = new SizedQueue(currentSlowingPeriod);
            SizedQueue slowStochValues = new SizedQueue(currentSignalPeriod);

            foreach (var item in this.itemsSource)
            {
                double high = (double)this.HighBinding.GetValue(item);
                double low = (double)this.LowBinding.GetValue(item);
                double close = (double)this.CloseBinding.GetValue(item);

                double fastStochValue = CalculateMainValue(highValues, lowValues, high, low, close);
                fastStochValues.EnqueueItem(fastStochValue);

                double slowStochValue = MovingAverageIndicatorDataSource.CalculateCurrentValue(fastStochValues);
                slowStochValues.EnqueueItem(slowStochValue);

                double slowSignalValue = MovingAverageIndicatorDataSource.CalculateCurrentValue(slowStochValues);

                CategoricalDataPoint point, point2;
                if (model.DataPointsInternal.Count > currentIndex)
                {
                    point = model.DataPointsInternal[currentIndex] as CategoricalDataPoint;
                    point.Value = slowStochValue;
                }
                else
                {
                    point = this.GenerateDataPoint(item, -1) as CategoricalDataPoint;
                    point.Value = slowStochValue;
                    model.DataPointsInternal.Add(point);
                }

                if (signalModel.DataPointsInternal.Count > currentIndex)
                {
                    point2 = signalModel.DataPointsInternal[currentIndex] as CategoricalDataPoint;
                    point2.Value = slowSignalValue;
                }
                else
                {
                    point2 = this.GenerateDataPoint(item, -1) as CategoricalDataPoint;
                    point2.Value = slowSignalValue;
                    signalModel.DataPointsInternal.Add(point2);
                }

                currentIndex++;
            }
        }

        private static double CalculateMainValue(SizedQueue highValues, SizedQueue lowValues, double high, double low, double close)
        {
            highValues.EnqueueItem(high);
            lowValues.EnqueueItem(low);

            double max = highValues.Max();
            double min = lowValues.Min();

            double mainValue = (close - min) / (max - min) * 100;
            return mainValue;
        }
    }
}
