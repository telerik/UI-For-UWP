using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Primitives.DragDrop
{
    internal class DragInitializer
    {
        internal const double DefaultStartTreshold = 4.0;

        private UIElement element;
        private Pointer currentPointer;
        private Point startDragPoint;
        private ManipulationModes originalMode;
        private CoreWindow hookedCoreWindow;

        private double startTreshold;

        public DragInitializer(UIElement element)
            : this(element, DefaultStartTreshold)
        {
        }

        public DragInitializer(UIElement element, double treshold)
        {
            this.element = element;

            this.element.PointerPressed += this.Element_PointerPressed;
            this.element.PointerReleased += this.Element_PointerReleased;

            this.element.Holding += this.Element_Holding;

            this.startTreshold = treshold;
        }

        protected virtual void StartDrag(object sender, PointerRoutedEventArgs e)
        {
            DragDrop.StartDrag(sender, e, DragDropTrigger.Drag);
        }

        protected virtual void OnPressed(PointerRoutedEventArgs e)
        {
        }

        private void Element_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            this.CleanUp(e.Pointer);
        }

        private void Element_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            this.originalMode = this.element.ManipulationMode;

            if (this.element.CapturePointer(e.Pointer))
            {
                this.currentPointer = e.Pointer;
                this.startDragPoint = e.GetCurrentPoint(this.element).Position;
                this.element.PointerMoved += this.Element_PointerMoved;

                this.hookedCoreWindow = CoreWindow.GetForCurrentThread();
                this.hookedCoreWindow.InputEnabled += this.OnCoreWindowInputEnabled;

                this.OnPressed(e);
            }
        }

        private void Element_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (this.currentPointer != null && e.Pointer.PointerId == this.currentPointer.PointerId && this.PointExceedsThreshold(e.GetCurrentPoint(this.element).Position))
            {
                this.CleanUp(e.Pointer);

                this.StartDrag(sender, e);
            }
        }

        private void OnCoreWindowInputEnabled(CoreWindow sender, InputEnabledEventArgs args)
        {
            if (!args.InputEnabled && this.currentPointer != null)
            {
                this.CleanUp(this.currentPointer);
            }
        }

        private void CleanUp(Pointer pointer)
        {
            this.element.PointerMoved -= this.Element_PointerMoved;
            this.element.ReleasePointerCapture(pointer);
            this.currentPointer = null;

            if (this.hookedCoreWindow != null)
            {
                this.hookedCoreWindow.InputEnabled -= this.OnCoreWindowInputEnabled;
                this.hookedCoreWindow = null;
            }

            this.element.Holding -= this.Element_Holding;

            this.element.ManipulationMode = this.originalMode;
        }

        private bool PointExceedsThreshold(Point point)
        {
            var xLength = this.startDragPoint.X - point.X;
            var yLength = this.startDragPoint.Y - point.Y;
            var length = Math.Sqrt(xLength * xLength + yLength * yLength);

            return length > this.startTreshold;
        }

        private void Element_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (e.HoldingState == Windows.UI.Input.HoldingState.Started && this.currentPointer != null)
            {
                this.CleanUp(this.currentPointer);
            }
        }
    }
}
