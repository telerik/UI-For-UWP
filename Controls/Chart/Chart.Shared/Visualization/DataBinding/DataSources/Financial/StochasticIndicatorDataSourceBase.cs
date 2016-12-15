namespace Telerik.UI.Xaml.Controls.Chart
{
    internal abstract class StochasticIndicatorDataSourceBase : HighLowCloseIndicatorDataSourceBase
    {
        private int mainPeriod;
        private int signalPeriod;

        public int MainPeriod
        {
            get
            {
                return this.mainPeriod;
            }
            set
            {
                if (this.mainPeriod == value)
                {
                    return;
                }

                this.mainPeriod = value;

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
    }
}
