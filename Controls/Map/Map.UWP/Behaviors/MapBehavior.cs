using System.Collections.Generic;
using Telerik.Geospatial;
using Telerik.UI.Drawing;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Map
{
    /// <summary>
    /// This is the base class for all map behaviors.
    /// </summary>
    public abstract class MapBehavior : FrameworkElement
    {
        internal RadMap map;

        private bool isInternalPropertyChange;

        /// <summary>
        /// Gets the <see cref="RadMap"/> instance to which this behavior is attached.
        /// </summary>
        public RadMap Map
        {
            get
            {
                return this.map;
            }
        }

        internal bool IsInternalPropertyChange
        {
            get
            {
                return this.isInternalPropertyChange;
            }
        }

        /// <summary>
        /// Gets the desired manipulation mode to be used by this <see cref="MapBehavior"/> instance.
        /// </summary>
        protected internal virtual ManipulationModes DesiredManipulationMode
        {
            get
            {
                return ManipulationModes.None;
            }
        }

        /// <summary>
        /// Gets the <see cref="Canvas"/> instance used by different behaviors to add some elements to the visual tree.
        /// Will be null if the behavior is not yet attached or the map template is not applied.
        /// </summary>
        protected Canvas AdornerLayer
        {
            get
            {
                if (this.map != null)
                {
                    return this.map.adornerLayer;
                }

                return null;
            }
        }

        internal void ChangePropertyInternally(DependencyProperty property, object value)
        {
            this.isInternalPropertyChange = true;
            this.SetValue(property, value);
            this.isInternalPropertyChange = false;
        }

        internal void OnMapTemplateApplied()
        {
            // This is needed so that the behaviors are added to the visual scene. Although this seems strange, it actually enables bindings to work at behavior level.
            this.map.RenderSurface.Children.Add(this);
        }

        internal void Attach(RadMap owner)
        {
            this.map = owner;
            if (this.map.RenderSurface != null)
            {
                this.OnMapTemplateApplied();
            }
            this.OnAttached();
        }

        internal void Detach()
        {
            this.OnDetached();
            if (this.map.RenderSurface != null)
            {
                this.map.RenderSurface.Children.Remove(this);
            }
            this.map = null;
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
        /// Called when the respective <see cref="MapShapeLayer"/> is invalidated and its contents are cleared.
        /// </summary>
        /// <param name="layer">The shape layer.</param>
        protected internal virtual void OnShapeLayerCleared(MapShapeLayer layer)
        {
        }

        /// <summary>
        /// Initiates a hit test on the specified <see cref="Windows.Foundation.Point(double, double)"/> location.
        /// </summary>
        /// <remarks>
        /// The default <see cref="MapBehavior" /> logic returns only the top-most <see cref="D2DShape"/> from the <see cref="MapShapeLayer"/> that matches the specific behavior requirements; 
        /// you can override the default logic and return multiple <see cref="D2DShape"/> instances (e.g. from layers that overlay one another) and the specific <see cref="MapBehavior"/> will
        /// manipulate all of them.
        /// </remarks>
        /// <param name="location">The location.</param>
        protected internal virtual IEnumerable<IMapShape> HitTest(Point location)
        {
            yield return this.map.HitTest(location);
        }

        /// <summary>
        /// Handles the <see cref="E:RadMap.PointerEntered"/> event of the owning <see cref="RadMap"/> instance.
        /// </summary>
        protected internal virtual void OnPointerEntered(PointerRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Handles the <see cref="E:RadMap.PointerMoved"/> event of the owning <see cref="RadMap"/> instance.
        /// </summary>
        protected internal virtual void OnPointerMoved(PointerRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Handles the <see cref="E:RadMap.PointerExited"/> event of the owning <see cref="RadMap"/> instance.
        /// </summary>
        protected internal virtual void OnPointerExited(PointerRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Handles the <see cref="E:RadMap.PointerPressed"/> event of the owning <see cref="RadMap"/> instance.
        /// </summary>
        protected internal virtual void OnPointerPressed(PointerRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Handles the <see cref="E:RadMap.HoldCompleted"/> event of the owning <see cref="RadMap"/> instance.
        /// </summary>
        protected internal virtual void OnHoldCompleted(HoldingRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Handles the <see cref="E:RadMap.HoldStarted"/> event of the owning <see cref="RadMap"/> instance.
        /// </summary>
        protected internal virtual void OnHoldStarted(HoldingRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Handles the <see cref="E:RadMap.Tapped"/> event of the owning <see cref="RadMap"/> instance.
        /// </summary>
        protected internal virtual void OnTapped(TappedRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Handles the <see cref="E:RadMap.DoubleTapped"/> event of the owning <see cref="RadMap"/> instance.
        /// </summary>
        protected internal virtual void OnDoubleTapped(DoubleTappedRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Handles the <see cref="E:RadMap.ManipulationStarted"/> event of the owning <see cref="RadMap"/> instance.
        /// </summary>
        protected internal virtual void OnManipulationStarted(ManipulationStartedRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Handles the <see cref="E:RadMap.ManipulationDelta"/> event of the owning <see cref="RadMap"/> instance.
        /// </summary>
        protected internal virtual void OnManipulationDelta(ManipulationDeltaRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Handles the <see cref="E:RadMap.ManipulationCompleted"/> event of the owning <see cref="RadMap"/> instance.
        /// </summary>
        protected internal virtual void OnManipulationCompleted(ManipulationCompletedRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Handles the <see cref="E:RadMap.PointerWheelChanged"/> event of the owning <see cref="RadMap"/> instance.
        /// </summary>
        protected internal virtual void OnPointerWheelChanged(PointerRoutedEventArgs args)
        {
        }

        /// <summary>
        /// Handles the <see cref="E:RadMap.PointerReleased"/> event of the owning <see cref="RadMap"/> instance.
        /// </summary>
        protected internal virtual void OnPointerReleased(PointerRoutedEventArgs args)
        {
        }

        /// <summary>
        /// A callback from the owning <see cref="RadMap" /> instance that notifies for a view change event.
        /// </summary>
        /// <param name="context">The context.</param>
        protected internal virtual void OnMapViewChanged(ViewChangedContext context)
        {
        }

        /// <summary>
        /// Called when the owning <see cref="RadMap"/> instance is loaded in the visual tree.
        /// </summary>
        protected virtual void OnLoaded()
        {
        }

        /// <summary>
        /// Called when the map owner is removed from the element tree.
        /// </summary>
        protected virtual void OnUnloaded()
        {
        }

        /// <summary>
        /// Called when the behavior is added to the map control.
        /// </summary>
        protected virtual void OnAttached()
        {
        }

        /// <summary>
        /// Called when the behavior is removed from the map control.
        /// </summary>
        protected virtual void OnDetached()
        {
        }
    }
}
