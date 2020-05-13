using System;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Primitives.DragDrop
{
    internal abstract class DragVisualContext
    {
        internal static readonly Thickness InfinityThickness = new Thickness(double.PositiveInfinity);

        private Thickness maxPositionOffset = InfinityThickness;

        public event EventHandler DragVisualCleared;

        public abstract FrameworkElement DragVisualHost { get; }

        public Point DragStartPosition { get; set; }

        public DragPositionMode PositionRestriction { get; set; }
        
        public Thickness MaxPositionOffset
        {
            get { return this.maxPositionOffset; }
            set { this.maxPositionOffset = value; }
        }

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
