using System;
using System.Collections.Generic;
using Telerik.Geospatial;
using Telerik.UI.Drawing;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Map
{
    /// <summary>
    /// Represents a <see cref="MapBehavior"/> implementation that provides a visual indication when the primary input pointer is over a <see cref="Telerik.Geospatial.IMapShape"/> instance. Applicable for <see cref="MapShapeLayer"/> instances.
    /// </summary>
    public class MapShapePointerOverBehavior : MapBehavior
    {
        private D2DShape hoveredShape;

        internal void UpdateHoveredShape(Point location)
        {
            foreach (var shape in this.HitTest(location))
            {
                this.ClearHoveredShape();

                if (shape == null || this.map == null)
                {
                    continue;
                }

                var d2dShape = this.map.FindShapeForModel(shape);
                if (d2dShape == null)
                {
                    continue;
                }

                if (d2dShape.UIState != ShapeUIState.Selected)
                {
                    d2dShape.UIState = ShapeUIState.PointerOver;
                    this.hoveredShape = d2dShape;
                }
            }
        }

        /// <summary>
        /// Initiates a hit test on the specified <see cref="Windows.Foundation.Point(double, double)" /> location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <remarks>
        /// The default <see cref="MapBehavior" /> logic returns only the top-most <see cref="D2DShape" /> from the <see cref="MapShapeLayer" /> that matches the specific behavior requirements;
        /// you can override the default logic and return multiple <see cref="D2DShape" /> instances (e.g. from layers that overlay one another) and the specific <see cref="MapBehavior" /> will
        /// manipulate all of them.
        /// </remarks>
        protected internal override IEnumerable<IMapShape> HitTest(Point location)
        {
            IMapShape shape = null;

            int layerCount = this.map.Layers.Count;
            for (int i = layerCount - 1; i >= 0; i--)
            {
                var layer = this.map.Layers[i] as MapShapeLayer;
                if (layer == null || !layer.IsPointerOverEnabled)
                {
                    continue;
                }

                shape = this.map.HitTest(location, layer);
                if (shape != null)
                {
                    break;
                }
            }

            yield return shape;
        }

        /// <summary>
        /// Raises the <see cref="E:PointerEntered" /> event.
        /// </summary>
        /// <param name="args">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
        protected internal override void OnPointerEntered(PointerRoutedEventArgs args)
        {
            base.OnPointerEntered(args);

            if (args == null)
            {
                throw new ArgumentNullException();
            }

            if (args.Pointer.IsInContact)
            {
                return;
            }

            var location = args.GetCurrentPoint(this.map.D2DSurface).Position;
            this.UpdateHoveredShape(location);
        }

        /// <summary>
        /// Raises the <see cref="E:PointerMoved" /> event.
        /// </summary>
        /// <param name="args">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
        protected internal override void OnPointerMoved(PointerRoutedEventArgs args)
        {
            base.OnPointerMoved(args);

            if (args == null)
            {
                throw new ArgumentNullException();
            }

            if (args.Pointer.IsInContact)
            {
                return;
            }

            var location = args.GetCurrentPoint(this.map.D2DSurface).Position;
            this.UpdateHoveredShape(location);
        }

        /// <summary>
        /// Raises the <see cref="E:PointerReleased" /> event.
        /// </summary>
        /// <param name="args">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
        protected internal override void OnPointerReleased(PointerRoutedEventArgs args)
        {
            base.OnPointerReleased(args);

            if (args == null)
            {
                throw new ArgumentNullException();
            }

            if (args.Pointer.PointerDeviceType == PointerDeviceType.Touch)
            {
                return;
            }

            var location = args.GetCurrentPoint(this.map).Position;
            this.UpdateHoveredShape(location);
        }

        /// <summary>
        /// Raises the <see cref="E:PointerExited" /> event.
        /// </summary>
        /// <param name="args">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
        protected internal override void OnPointerExited(PointerRoutedEventArgs args)
        {
            base.OnPointerExited(args);

            this.ClearHoveredShape();
        }

        /// <summary>
        /// Called when the map owner is removed from the element tree.
        /// </summary>
        protected override void OnUnloaded()
        {
            base.OnUnloaded();

            this.ClearHoveredShape();
        }

        private void ClearHoveredShape()
        {
            if (this.hoveredShape != null)
            {
                if (this.hoveredShape.UIState == ShapeUIState.PointerOver)
                {
                    this.hoveredShape.UIState = ShapeUIState.Normal;
                }
                this.hoveredShape = null;
            }
        }
    }
}
