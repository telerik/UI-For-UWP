using System;
using Telerik.Charting;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a chart behavior that handles Pinch and Drag gestures and manipulates the Zoom and ScrollOffset properties of the associated <see cref="RadChartBase"/> instance.
    /// </summary>
    public class ChartPanAndZoomBehavior : ChartBehavior
    {
        private const double MouseWheelZoomAmount = 0.33;
        private ChartPanZoomMode zoomMode = ChartPanZoomMode.Both;
        private ChartPanZoomMode panMode = ChartPanZoomMode.Both;
        private bool handleDoubleTap = true;

        /// <summary>
        /// Gets or sets a value indicating whether a Double-tap gesture will be handled by the behavior to reset the current Zoom and ScrollOffset values.
        /// </summary>
        public bool HandleDoubleTap
        {
            get
            {
                return this.handleDoubleTap;
            }
            set
            {
                this.handleDoubleTap = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ChartPanZoomMode"/> value that specifies how the chart will respond to a zoom gesture.
        /// </summary>
        public ChartPanZoomMode ZoomMode
        {
            get
            {
                return this.zoomMode;
            }
            set
            {
                this.zoomMode = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ChartPanZoomMode"/> value that specifies how the chart will respond to a pan gesture.
        /// </summary>
        public ChartPanZoomMode PanMode
        {
            get
            {
                return this.panMode;
            }
            set
            {
                this.panMode = value;
            }
        }

        internal override ManipulationModes DesiredManipulationMode
        {
            get
            {
                ManipulationModes desired = ManipulationModes.None;
                if ((this.panMode & ChartPanZoomMode.Horizontal) == ChartPanZoomMode.Horizontal)
                {
                    desired |= ManipulationModes.TranslateX | ManipulationModes.TranslateRailsX;
                }
                if ((this.panMode & ChartPanZoomMode.Vertical) == ChartPanZoomMode.Vertical)
                {
                    desired |= ManipulationModes.TranslateY | ManipulationModes.TranslateRailsY;
                }

                if (desired != ManipulationModes.None)
                {
                    // add inertia to the translation
                    desired |= ManipulationModes.TranslateInertia;
                }

                if (this.zoomMode != ChartPanZoomMode.None)
                {
                    desired |= ManipulationModes.Scale;
                }
                
                return desired;
            }
        }

        internal void OnPan(Point translation)
        {
            Point newOffset = this.chart.scrollOffsetCache;
            if ((this.panMode & ChartPanZoomMode.Horizontal) == ChartPanZoomMode.Horizontal)
            {
                newOffset.X += translation.X / this.chart.chartArea.plotArea.layoutSlot.Width;
            }
            if ((this.panMode & ChartPanZoomMode.Vertical) == ChartPanZoomMode.Vertical)
            {
                newOffset.Y += translation.Y / this.chart.chartArea.plotArea.layoutSlot.Height;
            }

            this.chart.ScrollOffset = newOffset;
        }

        /// <summary>
        /// Method is internal for the sake of unit-testing.
        /// </summary>
        internal void OnZoomScale(double scale)
        {
            this.ApplyNewZoom(scale * this.chart.zoomCache.Width, scale * this.chart.zoomCache.Height);
        }

        /// <summary>
        /// If <see cref="HandleDoubleTap"/> is true, resets the current ScrollOffset and Zoom properties of the owning chart to their default values.
        /// </summary>
        protected internal override void OnDoubleTapped(DoubleTappedRoutedEventArgs args)
        {
            base.OnDoubleTapped(args);

            if (this.handleDoubleTap)
            {
                this.chart.Zoom = new Size(1, 1);
            }
        }

        /// <summary>
        /// Processes the Scale and Translation properties of the Delta manipulation.
        /// The event is handled if the owning chart is not in "Hold" state.
        /// </summary>
        protected internal override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs args)
        {
            base.OnManipulationDelta(args);

            if (this.chart.isInHold)
            {
                return;
            }

            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (args.Delta.Scale != 1f)
            {
                // zooming is a heavier operation and is processed slowly on a low-end device
                // check the time stamp and skip some
                this.OnZoomScale(args.Delta.Scale);
                args.Handled = true;
                return;
            }

            Point translation = args.Delta.Translation;
            if (translation.X != 0 || translation.Y != 0)
            {
                this.OnPan(translation);
                args.Handled = true;
            }
        }

        /// <summary>
        /// Handles the <see cref="E:RadChartBase.PointerWheelChanged"/> event of the owning <see cref="RadChartBase"/> instance.
        /// </summary>
        protected internal override void OnPointerWheelChanged(PointerRoutedEventArgs args)
        {
            base.OnPointerWheelChanged(args);

            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (args.Pointer.PointerDeviceType != PointerDeviceType.Mouse)
            {
                return;
            }

            if (args.Pointer.IsInContact)
            {
                return;
            }

            int delta = args.GetCurrentPoint(this.chart).Properties.MouseWheelDelta;
            double zoomDelta = MouseWheelZoomAmount * Math.Sign(delta);

            // raise the flag to update the ScrollOffset accordingly
            bool isManipulating = this.chart.manipulating;
            this.chart.manipulating = true;

            Size oldZoom = this.chart.zoomCache;
            this.ApplyNewZoom(zoomDelta + this.chart.zoomCache.Width, zoomDelta + this.chart.zoomCache.Height);
            args.Handled = oldZoom != this.chart.zoomCache;

            this.chart.manipulating = isManipulating;
        }

        private void ApplyNewZoom(double zoomX, double zoomY)
        {
            double newZoomX = this.chart.zoomCache.Width;
            if ((this.zoomMode & ChartPanZoomMode.Horizontal) == ChartPanZoomMode.Horizontal)
            {
                newZoomX = Math.Max(zoomX, 1);
            }
            double newZoomY = this.chart.zoomCache.Height;
            if ((this.zoomMode & ChartPanZoomMode.Vertical) == ChartPanZoomMode.Vertical)
            {
                newZoomY = Math.Max(zoomY, 1);
            }

            this.chart.Zoom = new Size(newZoomX, newZoomY);
        }
    }
}
