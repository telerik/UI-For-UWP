using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Grid.View
{
    internal class XamlOverlayAdornerLayer : SharedUILayer, IDataStatusListener
    {
        private DataProviderStatus currentStatus;

        private DataGridBusyOverlayControl busyControl;

        private DataGridBusyOverlayControl BusyControl
        {
            get
            {
                return this.busyControl;
            }
        }

        public void OnDataStatusChanged(DataProviderStatus status)
        {
            if (this.busyControl != null && this.Owner.IsBusyIndicatorEnabled)
            {
                var isBusy = status != DataProviderStatus.Ready && status != DataProviderStatus.RequestingData && status != DataProviderStatus.Faulted;

                if (this.BusyControl.IsBusy != isBusy)
                {
                    this.BusyControl.IsBusy = isBusy;

                    if (isBusy)
                    {
                        this.Owner.HitTestService.Suspend();
                    }
                    else
                    {
                        this.Owner.HitTestService.Resume();
                    }
                }
            }

            this.currentStatus = status;
        }

        internal void ArrangeOverlay(Rect arrangeRect)
        {
            if (this.busyControl != null && this.Owner.IsBusyIndicatorEnabled)
            {
                Canvas.SetTop(this.busyControl, arrangeRect.Top);
                Canvas.SetLeft(this.busyControl, arrangeRect.Left);
                this.busyControl.Width = arrangeRect.Width;
                this.busyControl.Height = arrangeRect.Height;
            }
        }

        protected internal override void AttachUI(Windows.UI.Xaml.Controls.Panel parent)
        {
            base.AttachUI(parent);

            this.busyControl = new DataGridBusyOverlayControl();
            this.AddVisualChild(this.busyControl);

            this.OnDataStatusChanged(this.currentStatus);
        }
    }
}
