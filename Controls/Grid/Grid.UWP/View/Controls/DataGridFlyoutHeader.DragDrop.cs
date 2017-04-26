using System;
using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop.Reorder;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    public partial class DataGridFlyoutHeader : IDragDropElement, IReorderItem
    {
        private bool skipHitTest;
        private ReorderItemsCoordinator reorderCoordinator;
        private int logicalIndex;

        internal event EventHandler<DragSurfaceRequestedEventArgs> DragSurfaceRequested;

        bool IDragDropElement.SkipHitTest
        {
            get
            {
                return this.skipHitTest;
            }
            set
            {
                this.skipHitTest = value;
            }
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

        DependencyObject IReorderItem.Visual
        {
            get
            {
                return this;
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
            get
            {
                return this.RenderSize;
            }
        }
        bool IDragDropElement.CanDrop(DragContext dragContext)
        {
            if (dragContext == null)
            {
                return false;
            }

            return dragContext.PayloadData is ReorderItemsDragOperation;
        }

        bool IDragDropElement.CanStartDrag(DragDropTrigger trigger, object initializeContext)
        {
            //// TODO: check CanStartReorder parameter
            return this.CanStartDrag(trigger, initializeContext);
        }

        void IDragDropElement.DragEnter(DragContext context)
        {
        }

        void IDragDropElement.DragLeave(DragContext context)
        {
        }

        void IDragDropElement.DragOver(DragContext context)
        {
            if (context == null)
            {
                return;
            }

            var data = context.PayloadData as ReorderItemsDragOperation;

            if (data != null)
            {
                var sourceHeader = this.reorderCoordinator.Host.ElementAt(data.CurrentSourceReorderIndex) as DataGridFlyoutHeader;

                //// TODO: check CanStartReorder parameter
                ////var canReorder = this.ParentGrid.DragBehavior.CanReorder(sourceHeader.DataContext as GroupDescriptorBase, this.DataContext as GroupDescriptorBase);
                var canReorder = this.CanReorder(sourceHeader.DataContext, this.DataContext);

                if (canReorder)
                {
                    var rect = context.GetDragVisualBounds(this);

                    if ((rect.Bottom > this.ActualHeight / 2 && this.logicalIndex > data.CurrentSourceReorderIndex) ||
                        (rect.Y < this.ActualHeight / 2 && this.logicalIndex < data.CurrentSourceReorderIndex))
                    {
                        var newIndex = this.reorderCoordinator.ReorderItem(data.CurrentSourceReorderIndex, this);
                        data.CurrentSourceReorderIndex = newIndex;
                    }
                }
            }
        }

        DragStartingContext IDragDropElement.DragStarting(DragDropTrigger trigger, object initializeContext)
        {
            var dragVisual = this.ParentGrid.DragBehavior.GetReorderVisual(this);

            if (dragVisual == null)
            {
                DataGridFlyoutHeader header = this.CreateHeader();

                dragVisual = header;
            }

            // TODO: Consider exposing this through separate control.
            dragVisual.DataContext = this.DataContext;
            this.Opacity = 0.0;

            var surface = this.OnDragSurfaceRequested();

            var payload = new ReorderItemsDragOperation(this);

            this.ParentGrid.DragBehavior.OnReorderStarted(this.DataContext as GroupDescriptorBase);

            return new DragStartingContext { DragVisual = dragVisual, Payload = payload, DragSurface = surface, HitTestStrategy = new ColumnHeaderHittestStrategy(this, surface.RootElement) };
        }

        void IDragDropElement.OnDragDropComplete(DragCompleteContext dragContext)
        {
            if (dragContext != null)
            {
                this.ParentGrid.DragBehavior.OnReorderCompleted(this, dragContext.DragSuccessful);
            }
        }

        void IDragDropElement.OnDragging(DragContext dragContext)
        {
        }

        void IDragDropElement.OnDragVisualCleared(DragCompleteContext dragContext)
        {
            this.IsHitTestVisible = true;
            this.Opacity = 1;
        }

        void IDragDropElement.OnDrop(DragContext dragContext)
        {
            if (dragContext == null)
            {
                return;
            }

            var data = dragContext.PayloadData as ReorderItemsDragOperation;
            if (data != null)
            {
                this.reorderCoordinator.CommitReorderOperation(data.InitialSourceIndex, data.CurrentSourceReorderIndex);
            }
        }

        internal virtual bool CanStartDrag(DragDropTrigger trigger, object initializeContext)
        {
            return false;
        }

        internal virtual bool CanReorder(object sourceContext, object context)
        {
            return false;
        }

        /// <summary>
        /// Creates an instance of the <see cref="DataGridFlyoutGroupHeader"/>.
        /// </summary>
        protected virtual DataGridFlyoutHeader CreateHeader()
        {
            DataGridFlyoutGroupHeader header = new DataGridFlyoutGroupHeader();
            header.Width = this.ActualWidth;
            header.BottomGlyphOpacity = 0.0;
            header.OuterBorderVisibility = Visibility.Visible;
            return header;
        }

        private IDragSurface OnDragSurfaceRequested()
        {
            var args = new DragSurfaceRequestedEventArgs();

            if (this.DragSurfaceRequested != null)
            {
                this.DragSurfaceRequested(this, args);
            }

            return args.DragSurface;
        }
    }
}
