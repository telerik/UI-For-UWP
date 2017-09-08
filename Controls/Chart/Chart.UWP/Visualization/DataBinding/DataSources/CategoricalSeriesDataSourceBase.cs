using Telerik.Charting;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal abstract class CategoricalSeriesDataSourceBase : ChartSeriesDataSource
    {
        private DataPointBinding categoryBinding;

        public DataPointBinding CategoryBinding
        {
            get
            {
                return this.categoryBinding;
            }
            set
            {
                if (this.categoryBinding == value)
                {
                    return;
                }

                this.categoryBinding = value;
                this.categoryBinding.PropertyChanged += this.OnBoundItemPropertyChanged;

                if (this.ItemsSource != null)
                {
                    this.Rebind(false, null);
                }
            }
        }

        protected override void InitializeBinding(DataPointBindingEntry binding)
        {
            if (this.categoryBinding != null)
            {
                (binding.DataPoint as CategoricalDataPointBase).Category = this.categoryBinding.GetValue(binding.DataItem);
            }

            base.InitializeBinding(binding);
        }
    }
}
