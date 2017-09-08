using System;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Primitives.DragDrop
{
    internal class DragDropOperation : IDragDropOperation
    {
        private IDragSurface dragSurface;
        private DragVisualContext dragVisualContext;

        private Pointer capturedPointer;
        private Point startDragPosition;
        private Point relativeStartPosition;

        private IDragDropElement sourceElement;
        private IDragDropElement targetElement;

        private DragContext dragContext;
        private DragCompleteContext dragCompleteContext;

        private VirtualKey lastKeyModifier;

        private DragHitTestStrategy hitTestStrategy;

        private Rect dragVisualBounds;
        private Point restrictedDragPoint;

        private Control originalFocusedElement;
        private FocusState originalFocusState;

        private bool capturePending;

        public DragDropOperation(DragStartingContext startContext, IDragDropElement source, DragPositionMode positionMode, Pointer pointer, Point startClickPosition, Point relativeStartPosition)
        {
            this.sourceElement = source;

            this.dragSurface = startContext.DragSurface;
            this.dragContext = new DragContext(startContext.Payload, this);
            this.hitTestStrategy = startContext.HitTestStrategy ?? new DragHitTestStrategy(this.dragSurface.RootElement);

            this.dragVisualContext = this.dragSurface.CreateDragContext();
            this.dragVisualContext.DragStartPosition = new Point(startClickPosition.X - relativeStartPosition.X - startContext.DragVisual.Margin.Left, startClickPosition.Y - relativeStartPosition.Y - startContext.DragVisual.Margin.Top);
            this.dragVisualContext.PositionRestriction = positionMode;

            this.dragVisualContext.PrepareDragVisual(startContext.DragVisual);

            this.startDragPosition = startClickPosition;
            this.relativeStartPosition = relativeStartPosition;

            this.InitializeVisual();

            this.dragVisualContext.DragVisualHost.PointerMoved += this.DragVisualHost_PointerMoved;
            this.dragVisualContext.DragVisualHost.PointerReleased += this.DragVisualHost_PointerReleased;
            this.dragVisualContext.DragVisualHost.PointerCaptureLost += this.DragVisualHost_PointerCaptureLost;

            if (pointer != null && this.dragVisualContext.DragVisualHost.CapturePointer(pointer))
            {
                this.capturedPointer = pointer;
                this.originalFocusedElement = FocusManager.GetFocusedElement() as Control;
                if (this.originalFocusedElement != null)
                {
                    this.originalFocusState = this.originalFocusedElement.FocusState;
                }
                ((Control)this.dragVisualContext.DragVisualHost).Focus(FocusState.Programmatic);
            }
            else if (pointer != null)
            {
                this.CancelDrag();
            }
            else
            {
                this.capturePending = true;
            }

            this.dragSurface.RootElement.KeyDown += this.RootElement_KeyDown;
        }

        public IDragDropElement Source
        {
            get
            {
                return this.sourceElement;
            }
        }

        public Point RelativeStartPosition
        {
            get
            {
                return this.relativeStartPosition;
            }
            set
            {
                this.relativeStartPosition = value;
            }
        }

        public Rect GetDragVisualBounds(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentException();
            }

            return this.dragSurface.RootElement.TransformToVisual(element).TransformBounds(this.dragVisualBounds);
        }

        public Point GetDragVisualPosition(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentException();
            }

            return this.dragSurface.RootElement.TransformToVisual(element).TransformPoint(this.restrictedDragPoint);
        }

        public void EndDrag()
        {
            if (this.targetElement == null || this.lastKeyModifier == VirtualKey.Escape || !this.targetElement.CanDrop(this.dragContext))
            {
                this.CancelDrag();
            }
            else
            {
                this.OnDrop();
            }

            this.dragVisualContext.DragVisualCleared += this.DragSurface_DragVisualCleared;
            this.dragSurface.CompleteDrag(this.dragVisualContext, this.dragCompleteContext.DragSuccessful);
            this.OnDragDropComplete();

            if (this.originalFocusedElement != null)
            {
                this.originalFocusedElement.Focus(this.originalFocusState);
                this.originalFocusedElement = null;
            }
        }

        private void InitializeVisual()
        {
            this.dragSurface.PositionDragHost(this.dragVisualContext, this.startDragPosition, this.relativeStartPosition);
        }

        private void RootElement_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            this.lastKeyModifier = e.Key;

            if (this.lastKeyModifier == VirtualKey.Escape)
            {
                this.CancelDrag();
            }
        }

        private void DragVisualHost_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerId == this.capturedPointer.PointerId)
            {
                this.EndDrag();
            }
        }

        private void DragVisualHost_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            this.dragVisualContext.DragVisualHost.ReleasePointerCapture(this.capturedPointer);
        }

        private void DragVisualHost_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (this.capturePending)
            {
                this.capturePending = false;
                if (!this.EnsurePoinerCapture(e))
                {
                    return;
                }
            }

            var dragPoint = e.GetCurrentPoint(this.dragSurface.RootElement).Position;
            this.dragVisualBounds = this.dragSurface.PositionDragHost(this.dragVisualContext, dragPoint, this.relativeStartPosition);
            this.restrictedDragPoint = this.GetRestrictedPosition(dragPoint, this.dragVisualContext.PositionRestriction);

            IDragDropElement newTarget = this.hitTestStrategy.GetTarget(this.dragVisualBounds, this.restrictedDragPoint);

            if (this.targetElement != newTarget)
            {
                this.OnDragLeave();

                this.targetElement = newTarget;

                this.OnDragEnter();
            }
            else
            {
                this.OnDragOver();
            }

            this.OnDragging();
        }

        private bool EnsurePoinerCapture(PointerRoutedEventArgs e)
        {
            if (this.capturedPointer != null)
            {
                return true;
            }

            this.capturedPointer = e.Pointer;

            if (this.capturedPointer != null && this.dragVisualContext.DragVisualHost.CapturePointer(this.capturedPointer))
            {
                this.originalFocusedElement = FocusManager.GetFocusedElement() as Control;
                if (this.originalFocusedElement != null)
                {
                    this.originalFocusState = this.originalFocusedElement.FocusState;
                }
                ((Control)this.dragVisualContext.DragVisualHost).Focus(FocusState.Programmatic);

                return true;
            }
            else
            {
                this.CancelDrag();
                return false;
            }
        }

        private Point GetRestrictedPosition(Point pointerPosition, DragPositionMode positionRestriction)
        {
            Point result = pointerPosition;
            switch (positionRestriction)
            {
                case DragPositionMode.RailX:
                    result = new Point(pointerPosition.X, this.startDragPosition.Y);
                    break;
                case DragPositionMode.RailY:
                    result = new Point(this.startDragPosition.X, pointerPosition.Y);
                    break;
                case DragPositionMode.Free:
                default:
                    break;
            }

            return result;
        }

        private void OnDragEnter()
        {
            if (this.targetElement != null)
            {
                this.targetElement.DragEnter(this.dragContext);
            }
        }

        private void OnDragLeave()
        {
            if (this.targetElement != null)
            {
                this.targetElement.DragLeave(this.dragContext);
            }
        }

        private void OnDragOver()
        {
            if (this.targetElement != null)
            {
                this.targetElement.DragOver(this.dragContext);
            }
        }

        private void OnDragging()
        {
            if (this.targetElement != null)
            {
                this.targetElement.OnDragging(this.dragContext);
            }
        }

        private void OnDrop()
        {
            this.OnDragLeave();

            this.dragCompleteContext = new DragCompleteContext(this.dragContext.PayloadData, this, true);

            if (this.targetElement != null)
            {
                this.dragCompleteContext.Destination = this.targetElement;

                this.targetElement.OnDrop(this.dragContext);
            }
        }
   
        private void DragSurface_DragVisualCleared(object sender, EventArgs e)
        {
            this.dragVisualContext.DragVisualCleared -= this.DragSurface_DragVisualCleared;

            this.OnDragVisualCleared();
            this.Reset();
        }

        private void OnDragDropComplete()
        {
            if (this.sourceElement != null)
            {
                this.sourceElement.OnDragDropComplete(this.dragCompleteContext);
            }
        }

        private void OnDragVisualCleared()
        {
            if (this.sourceElement != null)
            {
                this.sourceElement.OnDragVisualCleared(this.dragCompleteContext);
            }
        }

        private void CancelDrag()
        {
            if (this.capturedPointer == null)
            {
                return;
            }

            this.dragVisualContext.DragVisualHost.ReleasePointerCapture(this.capturedPointer);
            this.dragCompleteContext = new DragCompleteContext(this.dragContext.PayloadData, this, false);

            this.OnDragLeave();
        }

        private void Reset()
        {
            this.dragVisualContext.DragVisualHost.PointerMoved -= this.DragVisualHost_PointerMoved;
            this.dragVisualContext.DragVisualHost.PointerReleased -= this.DragVisualHost_PointerReleased;
            this.dragVisualContext.DragVisualHost.PointerCaptureLost -= this.DragVisualHost_PointerCaptureLost;

            this.dragSurface.RootElement.KeyDown -= this.RootElement_KeyDown;

            this.dragVisualBounds = Rect.Empty;
            this.restrictedDragPoint = new Point();

            DragDrop.OnOperationFinished(this);
        }
    }
}