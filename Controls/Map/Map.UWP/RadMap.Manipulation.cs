using Windows.Devices.Input;
using Windows.UI.Input;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Map
{
    public partial class RadMap
    {
        internal bool isInHold;

        private MapBehaviorCollection behaviors;
        private ManipulationModes defaultManipulationMode;

        /// <summary>
        /// Gets the collection with all the <see cref="MapBehavior"/> instances, registered with the map.
        /// </summary>
        public MapBehaviorCollection Behaviors
        {
            get
            {
                return this.behaviors;
            }
        }

        /// <summary>
        /// Called before the PointerPressed event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);

            this.NotifyPointerPressed(e);
        }

        /// <summary>
        /// Called before the PointerEntered event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
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
        /// <param name="e">Event data for the event.</param>
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
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);

            this.NotifyPointerReleased(e);
            this.isInHold = false;
        }

        /// <summary>
        /// Called before the PointerExited event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
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
        /// <param name="e">Event data for the event.</param>
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
        /// <param name="e">Event data for the event.</param>
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
        /// <param name="e">Event data for the event.</param>
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
        /// <param name="e">Event data for the event.</param>
        protected override void OnManipulationStarted(ManipulationStartedRoutedEventArgs e)
        {
            base.OnManipulationStarted(e);

            this.NotifyManipulationStarted(e);
        }

        /// <summary>
        /// Called before the ManipulationDelta event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
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
        /// <param name="e">Event data for the event.</param>
        protected override void OnManipulationCompleted(ManipulationCompletedRoutedEventArgs e)
        {
            base.OnManipulationCompleted(e);

            this.NotifyManipulationCompleted(e);

            this.isInHold = false;
        }

        /// <summary>
        /// Called before the PointerWheelChanged event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
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
        /// Called within the handler of the <see cref="E:Loaded" /> event. Allows inheritors to provide their specific logic.
        /// </summary>
        protected override void LoadCore()
        {
            base.LoadCore();

            foreach (var behavior in this.behaviors)
            {
                behavior.Load();
            }
        }

        /// <summary>
        /// Called within the handler of the <see cref="E:Unloaded" /> event. Allows inheritors to provide their specific logic.
        /// </summary>
        protected override void UnloadCore()
        {
            base.UnloadCore();

            foreach (var behavior in this.behaviors)
            {
                behavior.Unload();
            }
        }

        private void NotifyPointerPressed(PointerRoutedEventArgs e)
        {
            this.defaultManipulationMode = this.ManipulationMode;
            ManipulationModes manipulationMode = ManipulationModes.None;

            foreach (MapBehavior behavior in this.behaviors)
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

        private void NotifyPointerEntered(PointerRoutedEventArgs e)
        {
            foreach (MapBehavior behavior in this.behaviors)
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
            foreach (MapBehavior behavior in this.behaviors)
            {
                behavior.OnPointerMoved(e);

                if (e.Handled)
                {
                    break;
                }
            }
        }

        private void NotifyPointerReleased(PointerRoutedEventArgs e)
        {
            this.ManipulationMode = this.defaultManipulationMode;
            foreach (MapBehavior behavior in this.behaviors)
            {
                behavior.OnPointerReleased(e);
            }
        }

        private void NotifyPointerExited(PointerRoutedEventArgs e)
        {
            foreach (MapBehavior behavior in this.behaviors)
            {
                behavior.OnPointerExited(e);

                if (e.Handled)
                {
                    break;
                }
            }
        }

        private void NotifyTapped(TappedRoutedEventArgs e)
        {
            foreach (MapBehavior behavior in this.behaviors)
            {
                behavior.OnTapped(e);

                if (e.Handled)
                {
                    break;
                }
            }
        }

        private void NotifyDoubleTapped(DoubleTappedRoutedEventArgs e)
        {
            foreach (MapBehavior behavior in this.behaviors)
            {
                behavior.OnDoubleTapped(e);

                if (e.Handled)
                {
                    break;
                }
            }
        }

        private void NotifyHold(HoldingRoutedEventArgs e)
        {
            foreach (MapBehavior behavior in this.behaviors)
            {
                behavior.OnHoldStarted(e);
            }
        }

        private void NotifyHoldCompleted(HoldingRoutedEventArgs e)
        {
            foreach (MapBehavior behavior in this.behaviors)
            {
                behavior.OnHoldCompleted(e);
            }
        }

        private void NotifyManipulationStarted(ManipulationStartedRoutedEventArgs e)
        {
            foreach (MapBehavior behavior in this.behaviors)
            {
                behavior.OnManipulationStarted(e);

                if (e.Handled)
                {
                    break;
                }
            }
        }

        private void NotifyManipulationDelta(ManipulationDeltaRoutedEventArgs e)
        {
            foreach (MapBehavior behavior in this.behaviors)
            {
                behavior.OnManipulationDelta(e);

                if (e.Handled)
                {
                    break;
                }
            }
        }

        private void NotifyManipulationCompleted(ManipulationCompletedRoutedEventArgs e)
        {
            foreach (MapBehavior behavior in this.behaviors)
            {
                behavior.OnManipulationCompleted(e);

                if (e.Handled)
                {
                    break;
                }
            }
        }

        private void NotifyPointerWheelChanged(PointerRoutedEventArgs e)
        {
            foreach (MapBehavior behavior in this.behaviors)
            {
                behavior.OnPointerWheelChanged(e);

                if (e.Handled)
                {
                    break;
                }
            }
        }

        private void NotifyViewChanged(ViewChangedContext context)
        {
            if (!this.IsTemplateApplied)
            {
                return;
            }

            foreach (var layer in this.layers)
            {
                layer.OnViewChanged(context);
            }

            foreach (MapBehavior behavior in this.behaviors)
            {
                behavior.OnMapViewChanged(context);
            }
        }
    }
}
