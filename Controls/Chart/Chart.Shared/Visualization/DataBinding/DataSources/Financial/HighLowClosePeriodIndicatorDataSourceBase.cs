namespace Telerik.UI.Xaml.Controls.Chart
{
    internal abstract class HighLowClosePeriodIndicatorDataSourceBase : HighLowCloseIndicatorDataSourceBase
    {
        private int period;

        public int Period
        {
            get
            {
                return this.period;
            }
            set
            {
                if (this.period == value)
                {
                    return;
                }

                this.period = value;

                if (this.ItemsSource != null)
                {
                    this.Rebind(false, null);
                }
            }
        }
    }
}
