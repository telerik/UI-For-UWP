using Telerik.UI.Xaml.Controls.Data.ListView.View.Controls;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop.Reorder;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data.ListView
{
    public partial class RadListViewItem : IReorderItem
    {
        private ReorderItemsCoordinator reorderCoordinator;
        private int logicalIndex;
        private bool isHandleEnabled;

        private string defaultHandleIconPathLight = "ms-appx:///Telerik.UI.Xaml.Controls.Data.UWP/ListView/Resources/reorder-handle-light.png";
        private string defaultHandleIconPathDark = "ms-appx:///Telerik.UI.Xaml.Controls.Data.UWP/ListView/Resources/reorder-handle-dark.png";

        /// <summary>
        /// Gets the path of the handle icon.
        /// </summary>
        public string HandleIconPath
        {
            get
            {
                return Application.Current.RequestedTheme == ApplicationTheme.Light ? this.defaultHandleIconPathLight : this.defaultHandleIconPathDark;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether if handling is enabled.
        /// </summary>
        internal bool IsHandleEnabled
        {
            get
            {
                return this.isHandleEnabled;
            }
            set
            {
                if (this.isHandleEnabled != value)
                {
                    this.isHandleEnabled = value;
                    this.PrepareReorderHandle();
                    this.ChangeVisualState(true);
                }
            }
        }

        DependencyObject IReorderItem.Visual
        {
            get { return this; }
        }

        int IReorderItem.LogicalIndex
        {
            get
            {
                return this.logicalIndex;
            }
            set
            {
                this.logicalIndex = value;
            }
        }

        Point IReorderItem.ArrangePosition
        {
            get
            {
                return new Point(Canvas.GetLeft(this), Canvas.GetTop(this));
            }
            set
            {
                Canvas.SetLeft(this, value.X);
                Canvas.SetTop(this, value.Y);
            }
        }

        Size IReorderItem.ActualSize
        {
            get { return this.RenderSize; }
        }

        ReorderItemsCoordinator IReorderItem.Coordinator
        {
            get
            {
                return this.reorderCoordinator;
            }
            set
            {
                this.reorderCoordinator = value;
            }
        }

        internal DragStartingContext InitializeReorder()
        {
            var dragVisual = this.ListView.DragBehavior.GetReorderVisual(this);

            if (dragVisual == null)
            {
                this.dragVisual = this.ListView.GetContainerForItem();

                this.dragVisual.Orientation = this.Orientation;
                this.dragVisual.Width = this.ActualWidth;
                this.dragVisual.Height = this.ActualHeight;
                this.dragVisual.ListView = this.ListView;
                this.dragVisual.isDragContent = true;
                this.dragVisual.IsSelected = this.IsSelected;
                this.ListView.PrepareContainerForItem(this.dragVisual, this.DataContext);
            }

            (dragVisual as IDragDropElement).SkipHitTest = true;

            DragDrop.SetDragPositionMode(this, this.ListView.Orientation == Orientation.Vertical ? DragPositionMode.RailY : DragPositionMode.RailX);

            this.Opacity = 0.0;

            var surface = new CanvasDragSurface(this.ListView.childrenPanel as FrameworkElement, this.ListView.childrenPanel as Canvas, true);

            var payload = new ReorderItemsDragOperation(this, this.DataContext);

            this.ListView.DragBehavior.OnReorderStarted(this.DataContext);

            return new DragStartingContext { DragVisual = dragVisual, Payload = payload, DragSurface = surface, HitTestStrategy = new ReorderListViewItemHitTestStrategy(this, surface.RootElement) };
        }

        private void FinalizeReorder(DragCompleteContext context)
        {
            if (context != null)
            {
                var data = context.PayloadData as ReorderItemsDragOperation;

                if (data != null)
                {
                    this.ListView.DragBehavior.OnDragDropCompleted(this, context.DragSuccessful);

                    var itemReordered = context.DragSuccessful;

                    if (!itemReordered && this.reorderCoordinator != null)
                    {
                        this.reorderCoordinator.CancelReorderOperation(this, data.InitialSourceIndex);
                    }
                }
            }
        }
    }
}