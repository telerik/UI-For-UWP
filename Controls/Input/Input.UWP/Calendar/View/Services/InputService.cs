using System;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    internal class InputService : ServiceBase<RadCalendar>
    {
        private const double SwipeGestureThreshold = 90.0d;
        private const double PinchGestureThreshold = 0.5d;

        private Pointer pointer;
        private Point lastPressedPoint;
        private Point initialSwipePoint;
        private UIElement contentPanel;

        private bool isHoldingStarted = false;
        private bool isDragging = false;
        private bool isDragInitialized = false;
        private bool canSwipe = true;

        internal InputService(RadCalendar owner)
            : base(owner)
        {
        }

        internal void AttachToContentPanel(UIElement panel)
        {
            if (panel != null)
            {
                this.contentPanel = panel;

                this.contentPanel.Tapped += this.OnContentPanelTapped;
                this.contentPanel.PointerMoved += this.OnContentPanelPointerMoved;
                this.contentPanel.PointerReleased += this.OnContentPanelPointerReleased;
                this.contentPanel.Holding += this.OnContentPanelHolding;
                this.contentPanel.PointerExited += this.OnContentPanelPointerExited;
                this.contentPanel.PointerPressed += this.OnContentPanelPointerPressed;

                this.contentPanel.ManipulationMode = ManipulationModes.TranslateY
                    | ManipulationModes.TranslateRailsY
                    | ManipulationModes.Scale;

                this.contentPanel.ManipulationStarted += this.OnContentPanelManipulationStarted;
                this.contentPanel.ManipulationCompleted += this.OnContentPanelManipulationCompleted;
            }
        }

        internal void AttachToTimeRulerPanel(UIElement panel)
        {
            if (panel != null)
            {
                this.contentPanel = panel;
                this.contentPanel.Tapped += this.OnContentPanelTapped;
            }
        }

        internal void DetachFromContentPanel()
        {
            if (this.contentPanel != null)
            {
                this.contentPanel.Tapped -= this.OnContentPanelTapped;
                this.contentPanel.PointerMoved -= this.OnContentPanelPointerMoved;
                this.contentPanel.PointerReleased -= this.OnContentPanelPointerReleased;
                this.contentPanel.Holding -= this.OnContentPanelHolding;
                this.contentPanel.PointerPressed -= this.OnContentPanelPointerPressed;
                this.contentPanel.PointerExited -= this.OnContentPanelPointerExited;

                this.contentPanel.ManipulationStarted -= this.OnContentPanelManipulationStarted;
                this.contentPanel.ManipulationCompleted -= this.OnContentPanelManipulationCompleted;
            }
        }

        internal void DetachFromTimeRulerPanel()
        {
            if (this.contentPanel != null)
            {
                this.contentPanel.Tapped -= this.OnContentPanelTapped;
            }
        }

        internal void OnDragStarted(Point lastPoint, PointerRoutedEventArgs e)
        {
            if (this.Owner.contentLayer.VisualElement.CapturePointer(e.Pointer))
            {
                this.isDragInitialized = true;
                this.pointer = e.Pointer;
                this.EnsureDragStarted(lastPoint, e);
            }
        }

        internal void EnsureDragStarted(Point lastPoint, PointerRoutedEventArgs e)
        {
            if (this.Owner.CanStartDrag(lastPoint) && this.isDragInitialized && (this.isHoldingStarted || e.Pointer.PointerDeviceType == PointerDeviceType.Mouse))
            {
                this.isDragging = true;
                this.isDragInitialized = false;

                this.Owner.OnDragStarted(lastPoint);
            }
        }

        internal void CancelDrag()
        {
            this.isDragInitialized = false;

            if (this.isDragging)
            {
                this.Owner.OnDragEnded(false);

                this.isDragging = false;
            }
        }

        private void OnContentPanelManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            this.initialSwipePoint = e.Position;
        }

        private void OnContentPanelManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (e.Cumulative.Scale < PinchGestureThreshold)
            {
                this.Owner.RaiseMoveToUpperViewCommand();
            }
            else if (this.canSwipe)
            {
                Point currentPoint = e.Position;

                if (currentPoint.Y - this.initialSwipePoint.Y >= SwipeGestureThreshold)
                {
                    this.Owner.RaiseMoveToPreviousViewCommand(1);
                }
                else if (this.initialSwipePoint.Y - currentPoint.Y >= SwipeGestureThreshold)
                {
                    this.Owner.RaiseMoveToNextViewCommand(1);
                }
            }
        }

        private void OnContentPanelPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var hitPoint = e.GetCurrentPoint(this.Owner.contentLayer.VisualElement).Position;

            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                this.canSwipe = false;
            }

            if (hitPoint != null)
            {
                this.lastPressedPoint = hitPoint;
                this.OnDragStarted(this.lastPressedPoint, e);
            }
        }

        private void OnContentPanelPointerExited(object sender, PointerRoutedEventArgs e)
        {
            this.Owner.OnContentPanelPointerExited();
        }

        private void OnContentPanelHolding(object sender, HoldingRoutedEventArgs e)
        {
            if (this.Owner.DisplayMode != CalendarDisplayMode.MonthView)
            {
                return;
            }

            this.isHoldingStarted = this.Owner.OnContentPanelHolding(e);
            this.canSwipe = false;

            e.Handled = this.isHoldingStarted;
        }

        private void OnContentPanelPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (this.isHoldingStarted)
            {
                this.OnDragStarted(this.lastPressedPoint, e);
                this.isHoldingStarted = false;
            }

            this.EnsureDragStarted(e.GetCurrentPoint(this.contentPanel).Position, e);
            this.Owner.OnContentPanelPointerMoved(e, this.isDragging);
        }

        private void OnContentPanelTapped(object sender, TappedRoutedEventArgs e)
        {
            if (this.isDragging)
            {
                return;
            }

            this.Owner.OnContentPanelTapped(e);
        }

        private void OnContentPanelPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (this.pointer != null)
            {
                this.Owner.contentLayer.VisualElement.ReleasePointerCapture(this.pointer);
                this.pointer = null;
            }

            if (this.isDragging)
            {
                this.Owner.OnDragEnded(true);
                this.isDragging = false;
            }

            if (this.Owner.visualStateLayer.holdVisual != null && this.Owner.visualStateLayer.holdVisual.Visibility == Visibility.Visible)
            {
                this.Owner.VisualStateService.UpdateHoldDecoration(null);
            }

            this.canSwipe = true;
            this.isDragInitialized = false;
            this.isHoldingStarted = false;
        }
    }
}
