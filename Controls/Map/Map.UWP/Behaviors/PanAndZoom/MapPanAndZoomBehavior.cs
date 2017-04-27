using System;
using Telerik.UI.Drawing;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Map
{
    /// <summary>
    /// Represents a map behavior that handles Pinch and Drag gestures and manipulates the relevant properties of the associated <see cref="RadMap"/> instance.
    /// </summary>
    public class MapPanAndZoomBehavior : MapBehavior
    {
        /// <summary>
        /// Identifies the <see cref="IsPanEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPanEnabledProperty =
            DependencyProperty.Register(nameof(IsPanEnabled), typeof(bool), typeof(MapPanAndZoomBehavior), new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="ZoomMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ZoomModeProperty =
            DependencyProperty.Register(nameof(ZoomMode), typeof(MapZoomMode), typeof(MapPanAndZoomBehavior), new PropertyMetadata(MapZoomMode.ZoomToPoint));

        /// <summary>
        /// Identifies the <see cref="DoubleTapAction"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DoubleTapActionProperty =
            DependencyProperty.Register(nameof(DoubleTapAction), typeof(MapDoubleTapAction), typeof(MapPanAndZoomBehavior), new PropertyMetadata(MapDoubleTapAction.ZoomToPoint));
     
        private const double MouseWheelZoomAmount = 0.33;
        private const double DoubleTapZoomAmount = 1d;
        private bool scaleManipulationInProgress = false;

        /// <summary>
        /// Gets or sets a value indicating whether the Translation manipulation is handled by the behavior.
        /// </summary>
        public bool IsPanEnabled
        {
            get
            {
                return (bool)this.GetValue(IsPanEnabledProperty);
            }
            set
            {
                this.SetValue(IsPanEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the current <see cref="MapZoomMode"/> value that specifies how a Pinch gesture is processed by the behavior.
        /// </summary>
        public MapZoomMode ZoomMode
        {
            get 
            { 
                return (MapZoomMode)this.GetValue(ZoomModeProperty); 
            }
            set 
            { 
                this.SetValue(ZoomModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating how the double tap action is processed.
        /// </summary>
        /// <value>
        /// The double tap action.
        /// </value>
        public MapDoubleTapAction DoubleTapAction
        {
            get
            {
                return (MapDoubleTapAction)this.GetValue(DoubleTapActionProperty);
            }
            set
            {
                this.SetValue(DoubleTapActionProperty, value);
            }
        }

        /// <summary>
        /// Gets the desired manipulation mode to be used by this <see cref="MapBehavior" /> instance.
        /// </summary>
        protected internal override ManipulationModes DesiredManipulationMode
        {
            get
            {
                ManipulationModes desired = ManipulationModes.None;
                if (this.IsPanEnabled)
                {
                    desired |= ManipulationModes.TranslateX | ManipulationModes.TranslateY;
                }

                if (desired != ManipulationModes.None)
                {
                    // add inertia to the translation
                    desired |= ManipulationModes.TranslateInertia;
                }

                if (this.ZoomMode != MapZoomMode.None)
                {
                    desired |= ManipulationModes.Scale;
                }

                return desired;
            }
        }

        /// <summary>
        /// Method is internal for the sake of unit-testing.
        /// </summary>
        /// <param name="zoomPoint">The zoom point.</param>
        /// <param name="newZoomLevel">The new zoom level.</param>
        internal void OnZoomScale(Point zoomPoint, double newZoomLevel)
        {
            double clampedZoomLevel = this.ClampZoomLevel(newZoomLevel);
            if (this.map.ZoomLevel == clampedZoomLevel)
            {
                return;
            }

            if (this.ZoomMode == MapZoomMode.ZoomToPoint)
            {
                this.ZoomToPoint(zoomPoint, clampedZoomLevel);
            }
            else if (this.ZoomMode == MapZoomMode.ZoomToCenter)
            {
                this.ZoomToCenter(clampedZoomLevel);
            }
        }

        /// <summary>
        /// Method is internal for the sake of unit-testing.
        /// </summary>
        /// <param name="translation">The translation.</param>
        internal void OnPan(Point translation)
        {
            if (!this.IsPanEnabled)
            {
                return;
            }

            // NOTE: We can set directly the scroll offset but we need to pass through the coercion mechanism for the Center property instead.
            var centerAfterPan = this.map.SpatialReference.ConvertGeographicToLogicalCoordinate(this.map.Center);

            double pixelFactorX = this.map.ViewportWidth / this.map.CurrentSize.Width;
            double pixelFactorY = this.map.ViewportWidth * this.map.AspectRatio / this.map.CurrentSize.Height;

            centerAfterPan.X -= translation.X * pixelFactorX;
            centerAfterPan.Y -= translation.Y * pixelFactorY;

            double zoom = this.map.ZoomLevel;
            var context = new ViewChangedContext()
            {
                PreviousZoomLevel = zoom,
                PreviousCenter = this.map.Center,
                NewCenter = map.SpatialReference.ConvertLogicalToGeographicCoordinate(centerAfterPan),
                NewZoomLevel = zoom
            };

            this.ExecuteViewChangedCommand(context);
        }

        /// <summary>
        /// Executes the <see cref="DoubleTapAction"/> associated with the behavior.
        /// </summary>
        /// <param name="args">The <see cref="Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs"/> instance containing the event data.</param>
        protected internal override void OnDoubleTapped(DoubleTappedRoutedEventArgs args)
        {
            base.OnDoubleTapped(args);

            if (args == null)
            {
                return;
            }

            if (this.DoubleTapAction == MapDoubleTapAction.ZoomToPoint)
            {
                double newZoomLevel = this.ClampZoomLevel(map.ZoomLevel + DoubleTapZoomAmount);
                if (this.map.ZoomLevel == newZoomLevel)
                {
                    return;
                }

                this.ZoomToPoint(args.GetPosition(this.map), newZoomLevel);
            }
            else if (this.DoubleTapAction == MapDoubleTapAction.Reset)
            {
                this.map.ClearValue(RadMap.ZoomLevelProperty);
                this.map.ClearValue(RadMap.CenterProperty);
            }
        }

        /// <summary>
        /// Handles the <see cref="E:RadMap.ManipulationStarted" /> event of the owning <see cref="RadMap" /> instance.
        /// </summary>
        protected internal override void OnManipulationStarted(ManipulationStartedRoutedEventArgs args)
        {
            base.OnManipulationStarted(args);

            this.scaleManipulationInProgress = false;
        }

        /// <summary>
        /// Handles the <see cref="E:RadMap.ManipulationCompleted" /> event of the owning <see cref="RadMap" /> instance.
        /// </summary>
        protected internal override void OnManipulationCompleted(ManipulationCompletedRoutedEventArgs args)
        {
            base.OnManipulationCompleted(args);

            this.scaleManipulationInProgress = false;
        }

        /// <summary>
        /// Processes the Scale and Translation properties of the Delta manipulation.
        /// The event is handled if the owning map is not in "Hold" state.
        /// </summary>
        protected internal override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs args)
        {
            base.OnManipulationDelta(args);

            if (this.map.isInHold)
            {
                return;
            }

            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (args.Delta.Scale != 1f)
            {
                this.scaleManipulationInProgress = true;

                this.OnZoomScale(args.Position, map.ZoomLevel + (args.Delta.Scale - 1));
                args.Handled = true;

                return;
            }

            Point translation = args.Delta.Translation;
            if (!this.scaleManipulationInProgress && (translation.X != 0 || translation.Y != 0))
            {
                this.OnPan(translation);
                args.Handled = true;
            }
        }

        /// <summary>
        /// Handles the <see cref="E:RadMap.PointerWheelChanged"/> event of the owning <see cref="RadMap"/> instance.
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

            PointerPoint currentPoint = args.GetCurrentPoint(this.map);
            int delta = currentPoint.Properties.MouseWheelDelta;
            double zoomDelta = MouseWheelZoomAmount * Math.Sign(delta);

            double oldZoom = this.map.ZoomLevel;

            this.OnZoomScale(currentPoint.Position, this.map.ZoomLevel + zoomDelta);

            args.Handled = oldZoom != this.map.ZoomLevel;
        }

        private void ZoomToPoint(Point zoomPoint, double newZoomLevel)
        {
            var doublePoint = new DoublePoint() { X = zoomPoint.X, Y = zoomPoint.Y };

            // calculate source point BEFORE the zoom is applied since this changes the ViewportWidth value
            var logicalSourcePoint = this.map.ConvertPixelToLogicalCoordinate(doublePoint);

            newZoomLevel = Math.Max(newZoomLevel, 1);
            var context = new ViewChangedContext()
            {
                PreviousZoomLevel = this.map.ZoomLevel,
                PreviousCenter = this.map.Center,
                NewZoomLevel = newZoomLevel
            };
            
            this.map.ZoomLevel = newZoomLevel;

            var logicalTargetPoint = this.map.ConvertPixelToLogicalCoordinate(doublePoint);

            var shift = new DoublePoint() { X = logicalSourcePoint.X - logicalTargetPoint.X, Y = logicalSourcePoint.Y - logicalTargetPoint.Y };

            var logicalCenter = this.map.SpatialReference.ConvertGeographicToLogicalCoordinate(this.map.Center);
            logicalCenter.X += shift.X;
            logicalCenter.Y += shift.Y;

            context.NewCenter = this.map.SpatialReference.ConvertLogicalToGeographicCoordinate(logicalCenter);
            this.ExecuteViewChangedCommand(context);
        }

        private void ZoomToCenter(double newZoomLevel)
        {
            var logicalSourcePoint = this.map.SpatialReference.ConvertGeographicToLogicalCoordinate(this.map.Center);

            newZoomLevel = Math.Max(newZoomLevel, 1);
            var context = new ViewChangedContext()
            {
                PreviousZoomLevel = this.map.ZoomLevel,
                PreviousCenter = this.map.Center,
                NewZoomLevel = newZoomLevel
            };

            this.map.ZoomLevel = newZoomLevel;

            var logicalTargetPoint = this.map.SpatialReference.ConvertGeographicToLogicalCoordinate(this.map.Center);

            Point shift = new Point(logicalSourcePoint.X - logicalTargetPoint.X, logicalSourcePoint.Y - logicalTargetPoint.Y);

            logicalTargetPoint.X += shift.X;
            logicalTargetPoint.Y += shift.Y;

            context.NewCenter = this.map.SpatialReference.ConvertLogicalToGeographicCoordinate(logicalTargetPoint);
            this.ExecuteViewChangedCommand(context);
        }

        private void ExecuteViewChangedCommand(ViewChangedContext context)
        {
            if (context.PreviousCenter == context.NewCenter && context.PreviousZoomLevel == context.NewZoomLevel)
            {
                // no actual change, do not raise the ViewChanged command
                return;
            }

            if (!this.map.CommandService.ExecuteCommand(CommandId.ViewChanged, context))
            {
                // command isn't executed, rollback the previous zoom level (Center value does not need to be rolled back as it hasn't been changed yet).
                this.map.ZoomLevel = context.PreviousZoomLevel;
            }
        }

        private double ClampZoomLevel(double newZoom)
        {
            double minZoom = this.map.MinZoomLevel;
            double maxZoom = this.map.MaxZoomLevel;

            if (newZoom < minZoom)
            {
                newZoom = minZoom;
            }
            else if (newZoom > maxZoom)
            {
                newZoom = maxZoom;
            }

            return newZoom;
        }
    }
}
