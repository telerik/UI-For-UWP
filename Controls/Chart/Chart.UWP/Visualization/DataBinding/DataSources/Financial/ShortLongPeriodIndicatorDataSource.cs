using Telerik.Charting;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal abstract class ShortLongPeriodIndicatorDataSourceBase : ValueIndicatorDataSourceBase
    {
        private int shortPeriod;
        private int longPeriod;

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
    }
}
