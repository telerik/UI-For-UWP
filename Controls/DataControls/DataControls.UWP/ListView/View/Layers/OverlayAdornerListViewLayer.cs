using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Data.ListView;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data
{
    internal class OverlayAdornerListViewLayer : ListViewLayer, IDataStatusListener
    {
        private DataProviderStatus currentStatus;
        private ListViewBusyOverlayControl busyControl;

        internal bool IsBusy
        {
            get
            {
                return this.currentStatus != DataProviderStatus.Ready &&
                    this.currentStatus != DataProviderStatus.RequestingData &&
                    this.currentStatus != DataProviderStatus.Faulted;
            }
        }

        public void OnDataStatusChanged(DataProviderStatus status)
        {
            if (this.busyControl != null && this.Owner.IsBusyIndicatorEnabled)
            {
                var isBusy = status != DataProviderStatus.Ready && status != DataProviderStatus.RequestingData && status != DataProviderStatus.Faulted;

                if (this.busyControl.IsBusy != isBusy)
                {
                    this.busyControl.IsBusy = isBusy;
                }
            }

            this.currentStatus = status;
        }

        internal void ArrangeOverlay(Rect arrangeRect)
        {
            if (this.busyControl != null && this.Owner.IsBusyIndicatorEnabled && this.IsBusy)
            {
                Canvas.SetTop(this.busyControl, arrangeRect.Top);
                Canvas.SetLeft(this.busyControl, arrangeRect.Left);
                this.busyControl.Arrange(arrangeRect);
            }
        }

        protected internal override void AttachUI(Panel parent)
        {
            base.AttachUI(parent);

            this.busyControl = new ListViewBusyOverlayControl();
            this.AddVisualChild(this.busyControl);

            this.OnDataStatusChanged(this.currentStatus);
        }
    }
}
