using System;
using Telerik.Charting;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal class TrueRangeIndicatorDataSource : HighLowCloseIndicatorDataSourceBase
    {
        internal static double CalculateValue(DataPointBinding highBinding, DataPointBinding lowBinding, DataPointBinding closeBinding, object previousItem, object currentItem)
        {
            double previousClose, high, low, value;

            high = (double)highBinding.GetValue(currentItem);
            low = (double)lowBinding.GetValue(currentItem);
            if (previousItem != null)
            {
                previousClose = (double)closeBinding.GetValue(previousItem);
            }
            else
            {
                previousClose = (double)closeBinding.GetValue(currentItem);
            }

            value = high - low;

            if (previousItem != null)
            {
                value = CalculateValue(previousClose, low, high);
            }

            return value;
        }

        protected override void UpdateBinding(DataPointBindingEntry binding)
        {
            int index = this.Bindings.IndexOf(binding);
            var item = binding.DataItem;

            double high = (double)this.HighBinding.GetValue(item);
            double low = (double)this.LowBinding.GetValue(item);
            double value = high - low;

            if (index > 0)
            {
                var previousItem = this.Bindings[index - 1].DataItem;
                double previousClose = (double)this.CloseBinding.GetValue(previousItem);
                value = CalculateValue(previousClose, low, high);
            }

            (binding.DataPoint as CategoricalDataPoint).Value = value;

            DataPointBindingEntry nextBinding = null;
            if (index < this.Bindings.Count - 1)
            {
                nextBinding = this.Bindings[index + 1];
                var nextItem = nextBinding.DataItem;
                double nextHigh = (double)this.HighBinding.GetValue(nextItem);
                double nextLow = (double)this.LowBinding.GetValue(nextItem);
                double close = (double)this.CloseBinding.GetValue(item);

                double nextValue = CalculateValue(close, nextLow, nextHigh);
                (nextBinding.DataPoint as CategoricalDataPoint).Value = nextValue;
            }

            base.UpdateBinding(binding);
        }

        protected override void BindCore()
        {
            double value;
            int currentIndex = 0;
            object previousItem = null;
            foreach (var item in this.itemsSource)
            {
                value = CalculateValue(this.HighBinding, this.LowBinding, this.CloseBinding, previousItem, item);

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
                currentIndex++;

                previousItem = item;
            }
        }

        private static double CalculateValue(double previousClose, double low, double high)
        {
            low = Math.Min(low, previousClose);
            high = Math.Max(high, previousClose);

            return high - low;
        }
    }
}
