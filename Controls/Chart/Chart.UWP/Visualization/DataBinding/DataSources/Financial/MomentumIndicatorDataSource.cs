using System.Collections.Generic;
using Telerik.Charting;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal class MomentumIndicatorDataSource : ValuePeriodIndicatorDataSourceBase
    {
        protected virtual double CalculateValue(double currentValue, double olderValue)
        {
            return (currentValue / olderValue) * 100;
        }

        protected override void UpdateBinding(DataPointBindingEntry binding)
        {
            int index = this.Bindings.IndexOf(binding);
            var item = binding.DataItem;
            double value = (double)this.ValueBinding.GetValue(item);
            double previousItemValue, nextItemValue;
            int period = this.Period;

            object previousItem, nextItem;
            if (index < period)
            {
                previousItem = this.Bindings[0].DataItem;
            }
            else
            {
                previousItem = this.Bindings[index - period].DataItem;
            }

            previousItemValue = (double)this.ValueBinding.GetValue(previousItem);
            
            double currentValue = this.CalculateValue(value, previousItemValue);
            (binding.DataPoint as CategoricalDataPoint).Value = currentValue;

            if (index + period < this.Bindings.Count)
            {
                DataPointBindingEntry nextItemBinding = this.Bindings[index + period];
                nextItem = nextItemBinding.DataItem;
                nextItemValue = (double)this.ValueBinding.GetValue(nextItem);
                double nextValue = this.CalculateValue(nextItemValue, value);
                (nextItemBinding.DataPoint as CategoricalDataPoint).Value = nextValue;
            }            
        }

        protected override void BindCore()
        {
            int period = this.Period;
            List<double> currentItems = new List<double>();

            int currentIndex = 0;

            foreach (var item in this.itemsSource)
            {
                double value = (double)this.ValueBinding.GetValue(item);

                if (currentItems.Count >= period + 1)
                {
                    currentItems.RemoveAt(0);
                }

                currentItems.Add(value);

                double currentValue = this.CalculateValue(currentItems[currentItems.Count - 1], currentItems[0]);
                CategoricalDataPoint point;
                point = this.GenerateDataPoint(item, -1) as CategoricalDataPoint;
                point.Value = currentValue;
                this.Owner.Model.DataPointsInternal.Add(point);
                currentIndex++;
            }
        }
    }
}
