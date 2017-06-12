namespace Telerik.UI.Xaml.Controls.Chart
{
    internal abstract class HighLowCloseIndicatorDataSourceBase : HighLowIndicatorDataSourceBase
    {
        private DataPointBinding closeBinding;

        public DataPointBinding CloseBinding
        {
            get
            {
                return this.closeBinding;
            }
            set
            {
                if (this.closeBinding == value)
                {
                    return;
                }

                this.closeBinding = value;
                this.closeBinding.PropertyChanged += this.OnBoundItemPropertyChanged;

                if (this.ItemsSource != null)
                {
                    this.Rebind(false, null);
                }
            }
        }
    }
}
