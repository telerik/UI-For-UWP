using System;
using System.Collections.Generic;
using System.Diagnostics;
using Telerik.Geospatial;
using Telerik.UI.Drawing;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Map
{
    /// <summary>
    /// Represents a <see cref="MapBehavior"/> that allows map shapes from different <see cref="MapShapeLayer"/> instances to be selected.
    /// </summary>
    public class MapShapeSelectionBehavior : MapBehavior
    {
        /// <summary>
        /// Identifies the <see cref="SelectionMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register(nameof(SelectionMode), typeof(MapShapeSelectionMode), typeof(MapShapeSelectionBehavior), new PropertyMetadata(MapShapeSelectionMode.Single, OnSelectionModePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="SelectedShape"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedShapeProperty =
            DependencyProperty.Register(nameof(SelectedShape), typeof(object), typeof(MapShapeSelectionBehavior), new PropertyMetadata(null, OnSelectedShapeChanged));

        private MapShapeSelectionMode selectionModeCache = MapShapeSelectionMode.Single;
        private List<MapShapeModel> selectedModels;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapShapeSelectionBehavior"/> class.
        /// </summary>
        public MapShapeSelectionBehavior()
        {
            this.selectedModels = new List<MapShapeModel>();
        }

        /// <summary>
        /// Raised when the current selection has changed.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event SelectionChangedEventHandler SelectionChanged;

        /// <summary>
        /// Gets the currently selected <see cref="IMapShape"/> instances.
        /// </summary>
        public IEnumerable<IMapShape> SelectedShapes
        {
            get
            {
                foreach (var model in this.selectedModels)
                {
                    yield return model;
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="MapShapeSelectionMode"/> value that defines how the user input affects the current selection.
        /// </summary>
        public MapShapeSelectionMode SelectionMode
        {
            get
            {
                return this.selectionModeCache;
            }
            set
            {
                this.SetValue(SelectionModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the currently selected <see cref="IMapShape"/> instance. When multiple selection is enabled, this value is set to the first selected shape.
        /// </summary>
        public IMapShape SelectedShape
        {
            get
            {
                if (this.selectedModels.Count > 0)
                {
                    return this.selectedModels[0];
                }

                return null;
            }
            set
            {
                this.SetValue(SelectedShapeProperty, value);
            }
        }

        internal virtual bool IsControlModifierKeyDown
        {
            get
            {
                return KeyboardHelper.IsModifierKeyDown(VirtualKey.Control);
            }
        }

        /// <summary>
        /// Clears the selected shapes for this behavior.
        /// </summary>
        public void ClearSelection()
        {
            this.ClearSelectedModelsAndUpdateUIState();
        }

        internal void PerformSelection(D2DShape shape)
        {
            MapShapeModel model = shape.Model as MapShapeModel;
            if (model == null)
            {
                Debug.Assert(false, "Must have a model associated with each UI shape");
                return;
            }

            var context = new SelectionChangeContext()
            {
                Layer = this.map.Layers.FindLayerById(shape.LayerId),
                Shape = shape,
                Model = model
            };

            if (this.selectionModeCache == MapShapeSelectionMode.Single)
            {
                this.SingleSelect(context, true);
            }
            else if (this.selectionModeCache == MapShapeSelectionMode.MultiSimple)
            {
                this.SingleSelect(context, false);
            }
            else if (this.selectionModeCache == MapShapeSelectionMode.MultiExtended)
            {
                this.MultiExtendedSelect(context);
            }

            this.NotifySelectionChanged(context);
        }

        internal void DeselectShape(MapShapeModel shapeModel)
        {
            this.selectedModels.Remove(shapeModel);
        }

        /// <summary>
        /// Method is internal for unit test purposes.
        /// </summary>
        /// <param name="position">The position.</param>
        internal void HandleTap(Point position)
        {
            if (this.selectionModeCache == MapShapeSelectionMode.None)
            {
                return;
            }

            foreach (var shape in this.HitTest(position))
            {
                if (shape == null)
                {
                    return;
                }

                var d2dShape = this.map.FindShapeForModel(shape);
                this.PerformSelection(d2dShape);
            }
        }
        
        /// <summary>
        /// Called when the respective <see cref="MapShapeLayer" /> is invalidated and its contents are cleared.
        /// </summary>
        /// <param name="layer">The shape layer.</param>
        protected internal override void OnShapeLayerCleared(MapShapeLayer layer)
        {
            if (layer == null)
            {
                return;
            }

            base.OnShapeLayerCleared(layer);

            var context = new SelectionChangeContext()
            {
                Layer = layer
            };

            for (int i = this.selectedModels.Count - 1; i >= 0; i--)
            {
                var shape = this.map == null ? null : this.map.FindShapeForModel(this.selectedModels[i]);
                if (shape == null || shape.LayerId != layer.Id)
                {
                    continue;
                }

                context.Shape = shape;
                context.Model = this.selectedModels[i];
                this.DeselectShape(context);
            }

            this.NotifySelectionChanged(context);
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
                if (layer == null || !layer.IsSelectionEnabled)
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
        /// Handles the <see cref="E:RadMap.Tapped" /> event of the owning <see cref="RadMap" /> instance.
        /// </summary>
        protected internal override void OnTapped(TappedRoutedEventArgs args)
        {
            base.OnTapped(args);

            if (args == null)
            {
                throw new ArgumentNullException();
            }

            var position = args.GetPosition(this.map.D2DSurface);

            this.HandleTap(position);
        }

        /// <summary>
        /// Raises the <see cref="CommandId.ShapeSelectionChanged" /> command and the <see cref="SelectionChanged" /> event.
        /// </summary>
        /// <param name="removedItems">The removed items.</param>
        /// <param name="addedItems">The added items.</param>
        protected virtual void OnSelectionChanged(IList<object> removedItems, IList<object> addedItems)
        {
            // The SelectionChangedEventArgs constructor requires non-null Added and Removed items
            if (removedItems == null)
            {
                removedItems = new object[] { };
            }
            if (addedItems == null)
            {
                addedItems = new object[] { };
            }

            var args = new SelectionChangedEventArgs(removedItems, addedItems);

            // execute the ShapeSelectionChanged command
            if (!this.map.CommandService.ExecuteCommand(CommandId.ShapeSelectionChanged, args))
            {
                // SelectionChange is prevented by the user, rollback previous state
                foreach (MapShapeModel model in addedItems)
                {
                    this.selectedModels.Remove(model);
                }

                foreach (MapShapeModel model in removedItems)
                {
                    this.selectedModels.Add(model);
                }
            }
            else
            {
                var eh = this.SelectionChanged;
                if (eh != null)
                {
                    eh(this, args);
                }

                if (this.selectedModels.Count == 1)
                {
                    this.ChangePropertyInternally(SelectedShapeProperty, this.selectedModels[0]);
                }
                else if (this.selectedModels.Count == 0)
                {
                    this.ChangePropertyInternally(SelectedShapeProperty, null);
                }

                this.UpdateShapeUIState(args.RemovedItems, args.AddedItems);
            }
        }

        private static void OnSelectionModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as MapShapeSelectionBehavior;
            behavior.selectionModeCache = (MapShapeSelectionMode)e.NewValue;

            behavior.OnSelectionModeChanged();
        }

        private static void OnSelectedShapeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as MapShapeSelectionBehavior;
            if (behavior.IsInternalPropertyChange)
            {
                return;
            }

            var newItem = e.NewValue as MapShapeModel;
            if (newItem != null)
            {
                var shape = behavior.map == null ? null : behavior.map.FindShapeForModel(newItem);
                if (shape != null)
                {
                    behavior.PerformSelection(shape);
                }
                else
                {
                    // model is still not visualized
                    behavior.SelectModel(newItem);
                }
            }
            else
            {
                behavior.ClearSelectedModelsAndUpdateUIState();
            }
        }

        private void UpdateShapeUIState(IList<object> removedItems, IList<object> addedItems)
        {
            if (removedItems != null)
            {
                foreach (MapShapeModel model in removedItems)
                {
                    var shape = this.map == null ? null : this.map.FindShapeForModel(model);
                    if (shape != null)
                    {
                        shape.UIState = ShapeUIState.Normal;
                    }
                }
            }

            if (addedItems != null)
            {
                foreach (MapShapeModel model in addedItems)
                {
                    var shape = this.map == null ? null : this.map.FindShapeForModel(model);
                    if (shape != null)
                    {
                        shape.UIState = ShapeUIState.Selected;
                    }
                }
            }
        }        

        private void OnSelectionModeChanged()
        {
            this.ClearSelectedModelsAndUpdateUIState();
        }

        private void SingleSelect(SelectionChangeContext context, bool clearSelection)
        {
            if (context.Shape.UIState == ShapeUIState.Selected)
            {
                this.DeselectShape(context);
            }
            else
            {
                if (clearSelection)
                {
                    this.ClearSelectedModels(context, false);
                }

                this.SelectShape(context);
            }
        }

        private void MultiExtendedSelect(SelectionChangeContext context)
        {
            bool isControlDown = this.IsControlModifierKeyDown;
            bool isContextShapeSelected = context.Shape.UIState == ShapeUIState.Selected;

            if (!isControlDown)
            {
                this.ClearSelectedModels(context, isContextShapeSelected);
            }

            if (!isContextShapeSelected)
            {
                this.SelectShape(context);
            }
            else if (isControlDown)
            {
                this.DeselectShape(context);
            }
        }

        private void SelectShape(SelectionChangeContext context)
        {
            this.selectedModels.Add(context.Model);

            if (context.AddedItems == null)
            {
                context.AddedItems = new List<object>();
            }
            context.AddedItems.Add(context.Model);
        }

        private void DeselectShape(SelectionChangeContext context)
        {
            this.selectedModels.Remove(context.Model);

            if (context.RemovedItems == null)
            {
                context.RemovedItems = new List<object>();
            }
            context.RemovedItems.Add(context.Model);
        }

        private void SelectModel(MapShapeModel model)
        {
            var context = new SelectionChangeContext()
            {
                Model = model
            };
            this.ClearSelectedModels(context, false);

            this.selectedModels.Add(context.Model);

            context.AddedItems = new List<object>();
            context.AddedItems.Add(context.Model);

            this.NotifySelectionChanged(context);
        }

        private void ClearSelectedModelsAndUpdateUIState()
        {
            var context = new SelectionChangeContext();

            this.ClearSelectedModels(context, false);
            this.UpdateShapeUIState(context.RemovedItems, context.AddedItems);
        }

        private void ClearSelectedModels(SelectionChangeContext context, bool skipContextShape)
        {
            if (this.selectedModels.Count == 0)
            {
                return;
            }

            List<object> removedItems = new List<object>();
            removedItems.AddRange(this.selectedModels);
            context.RemovedItems = removedItems;

            if (skipContextShape)
            {
                context.RemovedItems.Remove(context.Model);
            }

            this.selectedModels.Clear();

            if (skipContextShape)
            {
                this.selectedModels.Add(context.Model);
            }
        }

        private void NotifySelectionChanged(SelectionChangeContext context)
        {
            if (context.AddedItems == null && context.RemovedItems == null)
            {
                // no actual change
                return;
            }

            this.OnSelectionChanged(context.RemovedItems, context.AddedItems);
        }

        private class SelectionChangeContext
        {
            public D2DShape Shape;
            public MapShapeModel Model;
            public MapLayer Layer;
            public IList<object> AddedItems;
            public IList<object> RemovedItems;
        }
    }
}
