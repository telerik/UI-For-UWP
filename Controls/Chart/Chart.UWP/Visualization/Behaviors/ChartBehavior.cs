using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// This is the base class for all chart behaviors.
    /// </summary>
    public abstract class ChartBehavior : DependencyObject
    {
        internal RadChartBase chart;

        /// <summary>
        /// Gets the <see cref="RadChartBase"/> instance to which this behavior is attached.
        /// </summary>
        public RadChartBase Chart
        {
            get
            {
                return this.chart;
            }
        }

        internal virtual ManipulationModes DesiredManipulationMode
        {
            get
            {
                return ManipulationModes.None;
            }
        }

        /// <summary>
        /// Gets the <see cref="Canvas"/> instance used by different behaviors to add some elements to the visual tree.
        /// Will be null if the behavior is not yet attached or the chart's template is not applied.
        /// </summary>
        protected Canvas AdornerLayer
        {
            get
            {
                if (this.chart != null)
                {
                    return this.chart.adornerLayer;
                }

                return null;
            }
        }

        internal void Attach(RadChartBase owner)
        {
            this.chart = owner;
            this.OnAttached();
        }

        internal void Detach()
        {
            this.OnDetached();
            this.chart = null;
        }

        internal void Unload()
        {
            this.OnUnloaded();
        }

        internal void Load()
        {
            this.OnLoaded();
        }

        /// <summary>
        /// Handles the <see cref="E:RadChartBase.PointerEntered"/> event of the owning <see cref="RadChartBase"/> instance.
        /// </summary>
        protected internal virtual void OnPointerEntered(PointerRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Handles the <see cref="E:RadChartBase.PointerMoved"/> event of the owning <see cref="RadChartBase"/> instance.
        /// </summary>
        protected internal virtual void OnPointerMoved(PointerRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Handles the <see cref="E:RadChartBase.PointerExited"/> event of the owning <see cref="RadChartBase"/> instance.
        /// </summary>
        protected internal virtual void OnPointerExited(PointerRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Handles the <see cref="E:RadChartBase.PointerPressed"/> event of the owning <see cref="RadChartBase"/> instance.
        /// </summary>
        protected internal virtual void OnPointerPressed(PointerRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Handles the <see cref="E:RadChartBase.HoldCompleted"/> event of the owning <see cref="RadChartBase"/> instance.
        /// </summary>
        protected internal virtual void OnHoldCompleted(HoldingRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Handles the <see cref="E:RadChartBase.HoldStarted"/> event of the owning <see cref="RadChartBase"/> instance.
        /// </summary>
        protected internal virtual void OnHoldStarted(HoldingRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Handles the <see cref="E:RadChartBase.Tapped"/> event of the owning <see cref="RadChartBase"/> instance.
        /// </summary>
        protected internal virtual void OnTapped(TappedRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Handles the <see cref="E:RadChartBase.DoubleTapped"/> event of the owning <see cref="RadChartBase"/> instance.
        /// </summary>
        protected internal virtual void OnDoubleTapped(DoubleTappedRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Handles the <see cref="E:RadChartBase.ManipulationStarted"/> event of the owning <see cref="RadChartBase"/> instance.
        /// </summary>
        protected internal virtual void OnManipulationStarted(ManipulationStartedRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Handles the <see cref="E:RadChartBase.ManipulationDelta"/> event of the owning <see cref="RadChartBase"/> instance.
        /// </summary>
        protected internal virtual void OnManipulationDelta(ManipulationDeltaRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Handles the <see cref="E:RadChartBase.ManipulationCompleted"/> event of the owning <see cref="RadChartBase"/> instance.
        /// </summary>
        protected internal virtual void OnManipulationCompleted(ManipulationCompletedRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Handles the <see cref="E:RadChartBase.PointerWheelChanged"/> event of the owning <see cref="RadChartBase"/> instance.
        /// </summary>
        protected internal virtual void OnPointerWheelChanged(PointerRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Handles the <see cref="E:RadChartBase.PointerReleased"/> event of the owning <see cref="RadChartBase"/> instance.
        /// </summary>
        protected internal virtual void OnPointerReleased(PointerRoutedEventArgs args)
        {
        }

        /// <summary>
        /// A callback from the owning <see cref="RadChartBase"/> instance that notifies for a completed UpdateUI pass.
        /// </summary>
        protected internal virtual void OnChartUIUpdated()
        {
        }

        /// <summary>
        /// Gets <see cref="ChartDataContext"/> associated with a gives physical location.
        /// </summary>
        /// <param name="physicalOrigin">The relative physical position of the requested data context.</param>
        /// <param name="findNearestPoints">True to find the nearest points, if no points are found on the requested physical location.</param>
        /// <returns>Returns <see cref="ChartDataContext"/> object holding information for the requested physical location.</returns>
        protected virtual ChartDataContext GetDataContext(Point physicalOrigin, bool findNearestPoints)
        {
            if (this.chart != null)
            {
                return this.chart.GetDataContext(physicalOrigin, ChartPointDistanceCalculationMode.Linear);
            }

            return null;
        }

        /// <summary>
        /// Called when the owning <see cref="RadChartBase"/> instance is loaded in the visual tree.
        /// </summary>
        protected virtual void OnLoaded()
        {
        }

        /// <summary>
        /// Called when the chart owner is removed from the element tree.
        /// </summary>
        protected virtual void OnUnloaded()
        {
        }

        /// <summary>
        /// Called when the behavior is added to the chart control.
        /// </summary>
        protected virtual void OnAttached()
        {
        }

        /// <summary>
        /// Called when the behavior is removed from the chart control.
        /// </summary>
        protected virtual void OnDetached()
        {
        }
    }
}
