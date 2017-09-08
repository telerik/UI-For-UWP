using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Data.ListView;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data
{
    internal class ScrollableAdornerListViewLayer : ListViewLayer
    {
        private ListViewAutoDataLoadingControl loadingControl;

        public IDataStatusListener Listener
        {
            get
            {
                if (this.loadingControl == null)
                {
                    this.loadingControl = new ListViewAutoDataLoadingControl();
                }

                return this.loadingControl;
            }
        }

        internal void OnOwnerArranging(Rect rect)
        {
            if (this.loadingControl != null)
            {
                var arrangeRect = new Rect(rect.X, rect.Bottom - this.loadingControl.DesiredSize.Height, rect.Width, this.loadingControl.DesiredSize.Height);

                Canvas.SetTop(this.loadingControl, arrangeRect.Top);
                Canvas.SetLeft(this.loadingControl, arrangeRect.Left);

                this.loadingControl.Arrange(arrangeRect);
            }
        }

        protected internal override void DetachUI(Panel parent)
        {
            this.RemoveVisualChild(this.Listener as UIElement);

            base.DetachUI(parent);
        }

        protected internal override void AttachUI(Panel parent)
        {
            base.AttachUI(parent);

            var control = this.Listener as UIElement;

            if (control != null)
            {
                this.AddVisualChild(control);
                control.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }
        }
    }
}
