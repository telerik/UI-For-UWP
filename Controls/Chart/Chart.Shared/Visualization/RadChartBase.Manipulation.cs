using System;
using System.Collections.Generic;
using Telerik.Charting;
using Telerik.Core;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;

namespace Telerik.UI.Xaml.Controls.Chart
{
    public partial class RadChartBase
    {
        internal bool isInHold;
        internal bool manipulating;

        private ChartBehaviorCollection behaviors;
        private ChartDataContext lastDataContext;
        private ManipulationModes defaultManipulationMode;

        /// <summary>
        /// Gets a collection of chart behaviors. For example a ChartToolTipBehavior can
        /// be added to this collection which will enable tooltips on certain gestures.
        /// </summary>
        public ChartBehaviorCollection Behaviors
        {
            get
            {
                return this.behaviors;
            }
        }

        internal ChartDataContext GetDataContext(Point physicalOrigin, ChartPointDistanceCalculationMode pointDistanceMode)
        {
            // The tool tip location has to be translated because the data points are laid out in a larger layout slot if there is a zoom factor.
            // Also, the absolute value of the pan offset is used in the transformation because the pan offset is applied as a negative value to
            // the visualization of the plot area in order to simulate pan behavior.
            RadRect plotAreaClip = this.PlotAreaClip;
            physicalOrigin.X += Math.Abs(plotAreaClip.Width * this.scrollOffsetCache.X);
            physicalOrigin.Y += Math.Abs(plotAreaClip.Height * this.scrollOffsetCache.Y);

            if (this.lastDataContext != null && this.lastDataContext.TouchLocation == physicalOrigin)
            {
                return this.lastDataContext;
            }

            this.lastDataContext = this.GetDataContextCore(physicalOrigin, pointDistanceMode);

            return this.lastDataContext;
        }

        internal override void OnUIUpdated()
        {
            base.OnUIUpdated();

            this.NotifyUIUpdated();
        }

        /// <summary>
        /// Called before the PointerPressed event occurs.
        /// </summary>
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);

            this.NotifyPointerPressed(e);
        }

        /// <summary>
        /// Called before the PointerEntered event occurs.
        /// </summary>
        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            base.OnPointerEntered(e);

            if (e == null)
            {
                return;
            }

            // if the DeviceType is touch, we will process it through the Manipulation methods
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Touch)
            {
                return;
            }

            this.NotifyPointerEntered(e);
        }

        /// <summary>
        /// Called before the PointerMoved event occurs.
        /// </summary>
        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            base.OnPointerMoved(e);

            if (e == null)
            {
                return;
            }

            // if the DeviceType is touch, we will process it through the Manipulation methods
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Touch)
            {
                return;
            }

            this.NotifyPointerMoved(e);
        }

        /// <summary>
        /// Called before the PointerReleased event occurs.
        /// </summary>
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);

            if (e == null)
            {
                return;
            }

            this.NotifyPointerReleased(e);
            this.isInHold = false;
        }

        /// <summary>
        /// Called before the PointerExitedPressed event occurs.
        /// </summary>
        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            base.OnPointerExited(e);

            if (e == null)
            {
                return;
            }

            // if the DeviceType is touch, we will process it through the Manipulation methods
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Touch)
            {
                return;
            }

            this.NotifyPointerExited(e);
        }

        /// <summary>
        /// Called before the Tapped event occurs.
        /// </summary>
        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            base.OnTapped(e);

            if (e == null)
            {
                return;
            }

            this.NotifyTapped(e);
        }

        /// <summary>
        /// Called before the DoubleTapped event occurs.
        /// </summary>
        protected override void OnDoubleTapped(DoubleTappedRoutedEventArgs e)
        {
            base.OnDoubleTapped(e);

            if (e == null)
            {
                return;
            }

            this.NotifyDoubleTapped(e);
        }

        /// <summary>
        /// Called before the Holding event occurs.
        /// </summary>
        protected override void OnHolding(HoldingRoutedEventArgs e)
        {
            base.OnHolding(e);

            if (e == null)
            {
                return;
            }

            if (e.HoldingState == HoldingState.Started)
            {
                this.isInHold = true;
                this.NotifyHold(e);
            }
            else if (e.HoldingState == HoldingState.Completed)
            {
                this.isInHold = false;
                this.NotifyHoldCompleted(e);
            }
        }

        /// <summary>
        /// Called before the ManipulationStarted event occurs.
        /// </summary>
        protected override void OnManipulationStarted(ManipulationStartedRoutedEventArgs e)
        {
            base.OnManipulationStarted(e);

            if (e == null)
            {
                return;
            }

            this.NotifyManipulationStarted(e);
            this.manipulating = true;
        }

        /// <summary>
        /// Called before the ManipulationDelta event occurs.
        /// </summary>
        protected override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs e)
        {
            base.OnManipulationDelta(e);

            if (e == null)
            {
                return;
            }

            this.NotifyManipulationDelta(e);
        }

        /// <summary>
        /// Called before the ManipulationCompleted event occurs.
        /// </summary>
        protected override void OnManipulationCompleted(ManipulationCompletedRoutedEventArgs e)
        {
            base.OnManipulationCompleted(e);

            if (e == null)
            {
                return;
            }

            this.NotifyManipulationCompleted(e);
            this.isInHold = false;

            this.manipulating = false;
        }

        /// <summary>
        /// Called before the PointerWheelChanged event occurs.
        /// </summary>
        protected override void OnPointerWheelChanged(PointerRoutedEventArgs e)
        {
            base.OnPointerWheelChanged(e);

            if (e == null)
            {
                return;
            }

            this.NotifyPointerWheelChanged(e);
        }

        /// <summary>
        /// Gets <see cref="ChartDataContext"/> associated with a gives physical location.
        /// </summary>
        /// <param name="tapLocation">The relative physical position of the requested data context.</param>
        /// <param name="pointDistanceMode">The point distance calculation mode to be used in finding closest point.</param>
        /// <returns>Returns <see cref="ChartDataContext"/> object holding information for the requested physical location.</returns>
        protected virtual ChartDataContext GetDataContextCore(Point tapLocation, ChartPointDistanceCalculationMode pointDistanceMode)
        {
            List<DataPointInfo> closestPoints = new List<DataPointInfo>();
            double totalMinDistance = double.PositiveInfinity;

            DataPointInfo closestPoint = null;

            foreach (ChartSeriesModel series in this.chartArea.Series)
            {
                if (!series.Presenter.IsVisible)
                {
                    continue;
                }

                DataPointInfo currentClosestDataPoint = null;
                double minDistance = double.PositiveInfinity;
                ChartSeries visualSeries = series.presenter as ChartSeries;

                foreach (DataPoint dataPoint in series.DataPointsInternal)
                {
                    if (dataPoint.isEmpty)
                    {
                        continue;
                    }

                    double distance;
                    if (dataPoint.ContainsPosition(tapLocation.X, tapLocation.Y))
                    {
                        distance = 0;
                    }
                    else
                    {
                        distance = visualSeries.GetDistanceToPoint(dataPoint, tapLocation, pointDistanceMode);
                    }

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        if (currentClosestDataPoint == null)
                        {
                            currentClosestDataPoint = new DataPointInfo();
                        }

                        currentClosestDataPoint.DataPoint = dataPoint;
                        currentClosestDataPoint.SeriesModel = series;
                    }

                    if (distance < totalMinDistance)
                    {
                        totalMinDistance = distance;
                        closestPoint = currentClosestDataPoint;
                        if (distance == 0)
                        {
                            closestPoint.ContainsTouchLocation = true;
                        }
                    }
                }

                if (currentClosestDataPoint != null)
                {
                    closestPoints.Add(currentClosestDataPoint);
                }
            }

            return new ChartDataContext(closestPoints, closestPoint) { TouchLocation = tapLocation };
        }

        private void NotifyManipulationDelta(ManipulationDeltaRoutedEventArgs args)
        {
            foreach (ChartBehavior behavior in this.behaviors)
            {
                behavior.OnManipulationDelta(args);

                if (args.Handled)
                {
                    break;
                }
            }
        }

        private void NotifyManipulationStarted(ManipulationStartedRoutedEventArgs args)
        {
            foreach (ChartBehavior behavior in this.behaviors)
            {
                behavior.OnManipulationStarted(args);

                if (args.Handled)
                {
                    break;
                }
            }
        }

        private void NotifyManipulationCompleted(ManipulationCompletedRoutedEventArgs args)
        {
            foreach (ChartBehavior behavior in this.behaviors)
            {
                behavior.OnManipulationCompleted(args);

                if (args.Handled)
                {
                    break;
                }
            }
        }

        private void NotifyTapped(TappedRoutedEventArgs args)
        {
            foreach (ChartBehavior behavior in this.behaviors)
            {
                behavior.OnTapped(args);

                if (args.Handled)
                {
                    break;
                }
            }
        }

        private void NotifyDoubleTapped(DoubleTappedRoutedEventArgs args)
        {
            foreach (ChartBehavior behavior in this.behaviors)
            {
                behavior.OnDoubleTapped(args);

                if (args.Handled)
                {
                    break;
                }
            }
        }

        private void NotifyHold(HoldingRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in this.behaviors)
            {
                behavior.OnHoldStarted(e);
            }
        }

        private void NotifyHoldCompleted(HoldingRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in this.behaviors)
            {
                behavior.OnHoldCompleted(e);
            }
        }

        private void NotifyPointerEntered(PointerRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in this.behaviors)
            {
                behavior.OnPointerEntered(e);

                if (e.Handled)
                {
                    break;
                }
            }
        }

        private void NotifyPointerMoved(PointerRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in this.behaviors)
            {
                behavior.OnPointerMoved(e);

                if (e.Handled)
                {
                    break;
                }
            }
        }

        private void NotifyPointerExited(PointerRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in this.behaviors)
            {
                behavior.OnPointerExited(e);

                if (e.Handled)
                {
                    break;
                }
            }
        }

        private void NotifyPointerPressed(PointerRoutedEventArgs e)
        {
            this.defaultManipulationMode = this.ManipulationMode;
            ManipulationModes manipulationMode = ManipulationModes.None;

            foreach (ChartBehavior behavior in this.behaviors)
            {
                manipulationMode |= behavior.DesiredManipulationMode;

                behavior.OnPointerPressed(e);
                if (e.Handled)
                {
                    break;
                }
            }

            if (manipulationMode != ManipulationModes.None)
            {
                this.ManipulationMode = manipulationMode;
            }
        }

        private void NotifyPointerWheelChanged(PointerRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in this.behaviors)
            {
                behavior.OnPointerWheelChanged(e);
                if (e.Handled)
                {
                    break;
                }
            }
        }

        private void NotifyPointerReleased(PointerRoutedEventArgs e)
        {
            this.ManipulationMode = this.defaultManipulationMode;
            foreach (ChartBehavior behavior in this.behaviors)
            {
                behavior.OnPointerReleased(e);
            }
        }

        private void NotifyUIUpdated()
        {
            foreach (ChartBehavior behavior in this.behaviors)
            {
                behavior.OnChartUIUpdated();
            }
        }

        partial void OnUnloadedPartial()
        {
            foreach (var behavior in this.behaviors)
            {
                behavior.Unload();
            }

            this.manipulating = false;
        }

        partial void OnLoadedPartial()
        {
            foreach (var behavior in this.behaviors)
            {
                behavior.Load();
            }
        }

        partial void InitManipulation()
        {
            this.behaviors = new ChartBehaviorCollection(this);
        }
    }
}
