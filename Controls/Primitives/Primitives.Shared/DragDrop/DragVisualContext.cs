using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Primitives.DragDrop
{
    internal abstract class DragVisualContext
    {
        public event EventHandler DragVisualCleared;

        public abstract FrameworkElement DragVisualHost { get; }

        public Point DragStartPosition { get; set; }

        public DragPositionMode PositionRestriction { get; set; }

        internal virtual void PrepareDragVisual(FrameworkElement content)
        {
            this.DragVisualHost.Visibility = Visibility.Visible;
        }

        internal virtual void ClearDragVisual()
        {
            this.DragVisualHost.Visibility = Visibility.Collapsed;

            this.OnDragVisualCleared();
        }

        private void OnDragVisualCleared()
        {
            if (this.DragVisualCleared != null)
            {
                this.DragVisualCleared(this, EventArgs.Empty);
            }
        }
    }
}
