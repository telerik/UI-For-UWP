using System.Linq;
using Telerik.Charting;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal class StochasticFastIndicatorDataSource : StochasticIndicatorDataSourceBase
    {
        protected override void UpdateBinding(DataPointBindingEntry binding)
        {
            this.BindCore();
        }

        protected override void BindCore()
        {
            int currentIndex = 0;

            StochasticFastIndicator owner = this.Owner as StochasticFastIndicator;
            ChartSeriesModel model = this.Owner.Model;
            ChartSeriesModel signalModel = owner.SignalModel;

            int mainPeriod = this.MainPeriod;
            int signalPeriod = this.SignalPeriod;

            SizedQueue highValues = new SizedQueue(mainPeriod);
            SizedQueue lowValues = new SizedQueue(mainPeriod);
            SizedQueue stochValues = new SizedQueue(signalPeriod);

            foreach (var item in this.itemsSource)
            {
                double high = (double)this.HighBinding.GetValue(item);
                double low = (double)this.LowBinding.GetValue(item);
                double close = (double)this.CloseBinding.GetValue(item);

                double mainValue = CalculateMainValue(highValues, lowValues, high, low, close);

                stochValues.EnqueueItem(mainValue);

                double signalValue = MovingAverageIndicatorDataSource.CalculateCurrentValue(stochValues);

                CategoricalDataPoint point, point2;
                if (model.DataPointsInternal.Count > currentIndex)
                {
                    point = model.DataPointsInternal[currentIndex] as CategoricalDataPoint;
                    point.Value = mainValue;
                }
                else
                {
                    point = this.GenerateDataPoint(item, -1) as CategoricalDataPoint;
                    point.Value = mainValue;
                    model.DataPointsInternal.Add(point);
                }

                if (signalModel.DataPointsInternal.Count > currentIndex)
                {
                    point2 = signalModel.DataPointsInternal[currentIndex] as CategoricalDataPoint;
                    point2.Value = signalValue;
                }
                else
                {
                    point2 = this.GenerateDataPoint(item, -1) as CategoricalDataPoint;
                    point2.Value = signalValue;
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
