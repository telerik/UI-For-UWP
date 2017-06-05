using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Telerik.Core;
using Telerik.Geospatial;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Drawing;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;

namespace Telerik.UI.Xaml.Controls.Map
{
    /// <summary>
    /// Represents a concrete <see cref="MapLayer"/> implementation that is used to visualize vector information coming from various <see cref="IShapeDataSource"/> instances.
    /// </summary>
    public class MapShapeLayer : MapLayer, IWeakEventListener
    {
        /// <summary>
        /// Identifies the <see cref="ShapeStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShapeStyleProperty =
            DependencyProperty.Register(nameof(ShapeStyle), typeof(D2DShapeStyle), typeof(MapShapeLayer), new PropertyMetadata(null, OnShapeStylePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="ShapePointerOverStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShapePointerOverStyleProperty =
            DependencyProperty.Register(nameof(ShapePointerOverStyle), typeof(D2DShapeStyle), typeof(MapShapeLayer), new PropertyMetadata(null, OnShapePointerOverStylePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="ShapeSelectedStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShapeSelectedStyleProperty =
            DependencyProperty.Register(nameof(ShapeSelectedStyle), typeof(D2DShapeStyle), typeof(MapShapeLayer), new PropertyMetadata(null, OnShapeSelectedStylePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="ShapeLabelStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShapeLabelStyleProperty =
            DependencyProperty.Register(nameof(ShapeLabelStyle), typeof(D2DTextStyle), typeof(MapShapeLayer), new PropertyMetadata(null, OnShapeLabelStylePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="ShapeStyleSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShapeStyleSelectorProperty =
            DependencyProperty.Register(nameof(ShapeStyleSelector), typeof(MapShapeStyleSelector), typeof(MapShapeLayer), new PropertyMetadata(null, OnShapeStyleSelectorPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="ShapeLabelLayoutStrategy"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShapeLabelLayoutStrategyProperty =
            DependencyProperty.Register(nameof(ShapeLabelLayoutStrategy), typeof(MapShapeLabelLayoutStrategy), typeof(MapShapeLayer), new PropertyMetadata(null, OnShapeLabelLayoutStrategyPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="Source"/> dependency property.
        /// </summary>
        ////TODO: The Property type is object because WinRT bindings will fails to work with a .NET interface type.
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source), typeof(object), typeof(MapShapeLayer), new PropertyMetadata(null, OnSourcePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="ShapeLabelAttributeName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShapeLabelAttributeNameProperty =
            DependencyProperty.Register(nameof(ShapeLabelAttributeName), typeof(string), typeof(MapShapeLayer), new PropertyMetadata(null, OnShapeLabelAttributeNamePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="ShapeToolTipAttributeName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShapeToolTipAttributeNameProperty =
            DependencyProperty.Register(nameof(ShapeToolTipAttributeName), typeof(string), typeof(MapShapeLayer), new PropertyMetadata(null, OnShapeToolTipAttributeNamePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="IsSelectionEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectionEnabledProperty =
            DependencyProperty.Register(nameof(IsSelectionEnabled), typeof(bool), typeof(MapShapeLayer), new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="IsPointerOverEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPointerOverEnabledProperty =
            DependencyProperty.Register(nameof(IsPointerOverEnabled), typeof(bool), typeof(MapShapeLayer), new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="IsToolTipEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsToolTipEnabledProperty =
            DependencyProperty.Register(nameof(IsToolTipEnabled), typeof(bool), typeof(MapShapeLayer), new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="ShapeColorizer"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShapeColorizerProperty =
            DependencyProperty.Register(nameof(ShapeColorizer), typeof(MapShapeColorizer), typeof(MapShapeLayer), new PropertyMetadata(null, OnShapeColorizerPropertyChanged));

        internal static int layerIDCounter = 0;

        private List<D2DShape> shapes;
        private MapShapeModelCollection shapeModels;
        private D2DShapeStyle normalStyleCache;
        private D2DShapeStyle pointerOverStyleCache;
        private D2DShapeStyle selectedStyleCache;
        private D2DTextStyle labelStyleCache;

        private Dictionary<IMapShape, D2DShape> modelToVisualTable;

        private string labelAttributeNameCache;
        private string toolTipAttributeNameCache;

        private bool areModelsProcessed;
        private MapShapeColorizer colorizerCache;
        private MapShapeStyleSelector shapeStyleSelectorCache;
        private MapShapeLabelLayoutStrategy shapeLabelLayoutStrategyCache;
        private WeakEventHandler<PropertyChangedEventArgs> sourceShapesPropertyChangedHandler;

        // used by the D2DCanvas instance to associate shapes with a particular layer
        private int id;

        // used to determine the order of layer rendering within the single D2DCanvas instance
        private int zIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapShapeLayer"/> class.
        /// </summary>
        public MapShapeLayer()
        {
            this.DefaultStyleKey = typeof(MapShapeLayer);

            this.id = layerIDCounter++;
            this.areModelsProcessed = false;

            this.shapes = new List<D2DShape>();
            this.modelToVisualTable = new Dictionary<IMapShape, D2DShape>();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="MapShapeLayer"/> class.
        /// </summary>
        ~MapShapeLayer()
        {
            this.DetachSourceEvent();
        }

        /// <summary>
        /// Gets or sets the <see cref="MapShapeColorizer"/> instance that is used to provide context-based colorization of the visualized shapes, based on their attribute data.
        /// </summary>
        public MapShapeColorizer ShapeColorizer
        {
            get
            {
                return this.colorizerCache;
            }
            set
            {
                this.SetValue(ShapeColorizerProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the shapes within the layer may be manipulated by a <see cref="MapShapeSelectionBehavior"/> instance.
        /// </summary>
        public bool IsSelectionEnabled
        {
            get
            {
                return (bool)this.GetValue(IsSelectionEnabledProperty);
            }
            set
            {
                this.SetValue(IsSelectionEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the shapes within the layer may be manipulated by a <see cref="MapShapePointerOverBehavior"/> instance.
        /// </summary>
        public bool IsPointerOverEnabled
        {
            get
            {
                return (bool)this.GetValue(IsPointerOverEnabledProperty);
            }
            set
            {
                this.SetValue(IsPointerOverEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the shapes within the layer may be manipulated by a <see cref="MapShapeToolTipBehavior"/> instance.
        /// </summary>
        public bool IsToolTipEnabled
        {
            get
            {
                return (bool)this.GetValue(IsToolTipEnabledProperty);
            }
            set
            {
                this.SetValue(IsToolTipEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the name of the attribute, as specified by the *.dbf file, that points to the value set to each Shape as its label.
        /// </summary>
        public string ShapeLabelAttributeName
        {
            get
            {
                return this.labelAttributeNameCache;
            }
            set
            {
                this.SetValue(ShapeLabelAttributeNameProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the name of the attribute, as specified by the *.dbf file, that points to the value of each Shape that is used to display a toolTip for the shape.
        /// </summary>
        public string ShapeToolTipAttributeName
        {
            get
            {
                return this.toolTipAttributeNameCache;
            }
            set
            {
                this.SetValue(ShapeToolTipAttributeNameProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="MapShapeStyleSelector"/> instance that may be used to dynamically style a shape based on its attributes.
        /// </summary>
        public MapShapeStyleSelector ShapeStyleSelector
        {
            get
            {
                return this.shapeStyleSelectorCache;
            }
            set
            {
                this.SetValue(ShapeStyleSelectorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="MapShapeLabelLayoutStrategy"/> instance that may be used to customize the layout of each shape's label.
        /// </summary>
        public MapShapeLabelLayoutStrategy ShapeLabelLayoutStrategy
        {
            get
            {
                return this.shapeLabelLayoutStrategyCache;
            }
            set
            {
                this.SetValue(ShapeLabelLayoutStrategyProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="D2DShapeStyle"/> instance that defines the appearance of all the shapes that are currently in <see cref="ShapeUIState.Normal"/>  state.
        /// </summary>
        public D2DShapeStyle ShapeStyle
        {
            get
            {
                return this.normalStyleCache;
            }
            set
            {
                this.SetValue(ShapeStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="D2DShapeStyle"/> instance that defines the appearance of all the shapes that are currently in <see cref="ShapeUIState.PointerOver"/>  state.
        /// </summary>
        public D2DShapeStyle ShapePointerOverStyle
        {
            get
            {
                return this.pointerOverStyleCache;
            }
            set
            {
                this.SetValue(ShapePointerOverStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="D2DShapeStyle"/> instance that defines the appearance of all the shapes that are currently in <see cref="ShapeUIState.Selected"/>  state.
        /// </summary>
        public D2DShapeStyle ShapeSelectedStyle
        {
            get
            {
                return this.selectedStyleCache;
            }
            set
            {
                this.SetValue(ShapeSelectedStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="D2DTextStyle"/> instance that defines the appearance of each label, displayed by the visualized shapes.
        /// </summary>
        public D2DTextStyle ShapeLabelStyle
        {
            get
            {
                return this.labelStyleCache;
            }
            set
            {
                this.SetValue(ShapeLabelStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IShapeDataSource"/> implementation that provides the shapes definition.
        /// </summary>
        public IShapeDataSource Source
        {
            get
            {
                return (IShapeDataSource)this.GetValue(SourceProperty);
            }
            set
            {
                this.SetValue(SourceProperty, value);
            }
        }

        internal MapShapeModelCollection ShapeModels
        {
            get
            {
                return this.shapeModels;
            }
            set
            {
                if (this.shapeModels != value)
                {
                    this.shapeModels = value;

                    this.OnShapeModelsPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets a list of the <see cref="D2DShape"/> shape visuals - exposed for testing purposes, do not use outside the test project.
        /// </summary>
        internal List<D2DShape> ShapeVisuals
        {
            get
            {
                return this.shapes;
            }
        }

        internal override int Id
        {
            get
            {
                return this.id;
            }
        }

        internal int ZIndex
        {
            get
            {
                return this.zIndex;
            }
        }

        void IWeakEventListener.ReceiveEvent(object sender, object args)
        {
            var propertyChangedArgs = args as PropertyChangedEventArgs;
            if (propertyChangedArgs == null || propertyChangedArgs.PropertyName != ShapefileDataSource.ShapesPropertyName)
            {
                return;
            }

            this.ShapeModels = this.Source.Shapes;
        }

        /// <summary>
        /// Forces the <see cref="MapShapeLayer"/> to reevaluate and apply the styling logic for its associated shapes (e.g. <see cref="ShapeStyleSelector"/> and/or 
        /// <see cref="ShapeColorizer"/> will be re-applied in response to calling this method). 
        /// </summary>
        public void EvaluateShapeStyles()
        {
            this.PerformShapesUpdate(() =>
            {
                if (this.colorizerCache != null)
                {
                     this.colorizerCache.Initialize(this.ShapeModels);
                }             

                foreach (var shape in this.shapes)
                {
                    this.ApplyShapeStyle(shape);
                }
            });
        }

        internal D2DShape GetShapeForModel(IMapShape model)
        {
            D2DShape shape;
            if (this.modelToVisualTable.TryGetValue(model, out shape))
            {
                return shape;
            }

            return null;
        }

        internal bool ContainsShape(IMapShape model)
        {
            return this.modelToVisualTable.ContainsKey(model);
        }

        internal override void OnAttached()
        {
            base.OnAttached();

            this.zIndex = this.Owner.Layers.IndexOf(this);

            // NOTE: Update UI if necessary both on OnTemplateApplied() and OnAttached() to cover different scenarios (i.e. assign datasource prior to/after attaching layer to map, etc.).
            this.UpdateUIForUnprocessedModels();
        }

        internal override void UpdateUI()
        {
            base.UpdateUI();

            if (this.ShapeModels == null)
            {
                return;
            }

            if (this.colorizerCache != null && !this.areModelsProcessed)
            {
                this.colorizerCache.Initialize(this.ShapeModels);
            }

            EsriShapeType shapeType = this.ShapeModels.Type;
            switch (shapeType)
            {
                case EsriShapeType.NullShape:
                    break;
                case EsriShapeType.Point:
                case EsriShapeType.PointM:
                case EsriShapeType.PointZ:
                case EsriShapeType.Multipoint:
                case EsriShapeType.MultipointM:
                case EsriShapeType.MultipointZ:
                    this.RenderFilledShape();
                    break;

                case EsriShapeType.Polyline:
                case EsriShapeType.PolylineM:
                case EsriShapeType.PolylineZ:
                    this.RenderPolylines(false);
                    break;

                case EsriShapeType.Polygon:
                case EsriShapeType.PolygonM:
                case EsriShapeType.PolygonZ:
                    this.RenderPolylines(true);
                    break;

                default:
                    throw new NotSupportedException();
            }

            this.areModelsProcessed = true;
        }

        internal override void OnViewChanged(ViewChangedContext context)
        {
            base.OnViewChanged(context);

            if (this.shapeLabelLayoutStrategyCache != null && context.NewZoomLevel != context.PreviousZoomLevel)
            {
                // we need to re-evaluate the labels upon each zoom since some labels may depend on the zoom level
                this.ReevaluateShapeLabelStrategy();
            }
        }

        internal override void OnZoomChanged()
        {
            base.OnZoomChanged();

            if (this.shapeLabelLayoutStrategyCache != null)
            {
                // we need to re-evaluate the labels upon each zoom since some labels may depend on the zoom level
                this.ReevaluateShapeLabelStrategy();
            }
        }

        internal override void OnDetached(RadMap oldMap)
        {
            base.OnDetached(oldMap);

            this.InvalidateUI(oldMap);
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate" /> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            if (this.labelAttributeNameCache != null)
            {
                this.OnLabelAttributeChanged();
            }

            // NOTE: Update UI if necessary both on OnTemplateApplied() and OnAttached() to cover different scenarios (i.e. assign datasource prior to/after attaching layer to map, etc.).
            this.UpdateUIForUnprocessedModels();
        }

        /// <summary>
        /// Called within the handler of the <see cref="E:Unloaded" /> event. Allows inheritors to provide their specific logic.
        /// </summary>
        protected override void UnloadCore()
        {
            base.UnloadCore();

            this.InvalidateUI();
        }

        /// <summary>
        /// Called within the handler of the <see cref="E:Loaded" /> event. Allows inheritors to provide their specific logic.
        /// </summary>
        protected override void LoadCore()
        {
            base.LoadCore();

            if (this.WasUnloaded)
            {
                this.ScheduleUpdate();
            }
        }

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new MapShapeLayerAutomationPeer(this);
        }

        private static void OnSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var layer = (MapShapeLayer)sender;

            layer.DetachSourceEvent();

            if (layer.Source == null)
            {
                layer.ShapeModels = null;
                return;
            }

            layer.ShapeModels = layer.Source.Shapes;

            layer.AttachSourceEvent();
        }

        private static void OnShapeLabelAttributeNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var layer = d as MapShapeLayer;
            layer.labelAttributeNameCache = (string)e.NewValue;

            layer.OnLabelAttributeChanged();
        }

        private static void OnShapeLabelStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var layer = d as MapShapeLayer;
            layer.labelStyleCache = e.NewValue as D2DTextStyle;

            if (layer.labelStyleCache != null)
            {
                // we do a Clone here since this value may come from a Style and shared among all MapStreamLayer instances in all RadMaps.
                // If this happens it will generate a D2D exception since each D2DCanvas creates its own ID2D1Factory and a D2DBrush is associated 1-to-1 with a Factory.
                layer.labelStyleCache = layer.labelStyleCache.Clone();
            }

            layer.EvaluateShapeStyles();
        }

        private static void OnShapeToolTipAttributeNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var layer = d as MapShapeLayer;
            layer.toolTipAttributeNameCache = (string)e.NewValue;
        }

        private static void OnShapeStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var layer = d as MapShapeLayer;
            layer.UpdateStyleField(ref layer.normalStyleCache, e.NewValue as D2DShapeStyle);
        }

        private static void OnShapePointerOverStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var layer = d as MapShapeLayer;
            layer.UpdateStyleField(ref layer.pointerOverStyleCache, e.NewValue as D2DShapeStyle);
        }

        private static void OnShapeSelectedStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var layer = d as MapShapeLayer;
            layer.UpdateStyleField(ref layer.selectedStyleCache, e.NewValue as D2DShapeStyle);
        }

        private static void OnShapeStyleSelectorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var layer = d as MapShapeLayer;
            layer.shapeStyleSelectorCache = e.NewValue as MapShapeStyleSelector;
            layer.EvaluateShapeStyles();
        }

        private static void OnShapeLabelLayoutStrategyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var layer = d as MapShapeLayer;
            layer.shapeLabelLayoutStrategyCache = e.NewValue as MapShapeLabelLayoutStrategy;

            if (layer.shapeLabelLayoutStrategyCache != null)
            {
                layer.ReevaluateShapeLabelStrategy();
            }
            else
            {
                layer.ResetShapeLabelStrategy();
            }
        }

        private static void OnShapeColorizerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var layer = d as MapShapeLayer;

            if (layer.colorizerCache != null)
            {
                layer.colorizerCache.Reset();
            }

            layer.colorizerCache = e.NewValue as MapShapeColorizer;

            if (layer.colorizerCache != null)
            {
                layer.colorizerCache.Initialize(layer.ShapeModels);
            }

            layer.EvaluateShapeStyles();
        }

        private void MapShapeLayer_AttributeChanged(object sender, AttributeChangedEventArgs e)
        {
            var col = this.colorizerCache as ChoroplethColorizer;
            if (col != null && col.AttributeName == e.AttributeName)
            {
                this.EvaluateShapeStyles();
            }

            if (e.AttributeName == this.labelAttributeNameCache)
            {
                this.OnLabelAttributeChanged(sender as MapShapeModel);
            }
        }

        private void OnShapeModelsPropertyChanged()
        {
            if (this.ShapeModels != null)
            {
                this.Bounds = this.ShapeModels.BoundingRect;
            }
            else
            {
                this.Bounds = LocationRect.Empty;
            }

            if (!this.IsTemplateApplied)
            {
                this.areModelsProcessed = false;
                return;
            }

            this.InvalidateUI();

            if (this.Owner != null)
            {
                this.Owner.CommandService.ExecuteCommand(CommandId.ShapeLayerSourceChanged, this);
            }

            this.ScheduleUpdate();
        }

        private void UpdateStyleField(ref D2DShapeStyle field, D2DShapeStyle newValue)
        {
            field = newValue;
            if (field != null)
            {
                // TODO: This is not optimal. Instead of cloning, keep all D2DFactory-dependent resources on a per D2DCanvas instance basis.
                field = field.Clone();
            }

            this.EvaluateShapeStyles();
        }

        private void ReevaluateShapeLabelStrategy()
        {
            this.PerformShapesUpdate(() =>
            {
                foreach (var shape in this.shapes)
                {
                    if (shape.Label != null)
                    {
                        this.ApplyShapeLabelStrategy(shape);
                    }
                }
            });
        }

        private void ResetShapeLabelStrategy()
        {
            this.PerformShapesUpdate(() =>
            {
                foreach (var shape in this.shapes)
                {
                    shape.LabelVisibility = ShapeLabelVisibility.Auto;

                    if (shape.Label != null)
                    {
                        shape.LabelRenderPosition = new DoublePoint() { X = -1, Y = -1 };
                        shape.LabelRenderPositionOrigin = new Point(0.5, 0.5);
                    }
                }
            });
        }

        private void RenderFilledShape()
        {
            // The D2DCanvas will scale the position of the Rectangles
            if (this.Owner.D2DSurface.HasShapesForLayer(this.id))
            {
                return;
            }

            foreach (IMapShape shapeModel in this.ShapeModels)
            {
                var pointModel = shapeModel as MapPointModel;
                if (pointModel == null)
                {
                    continue;
                }

                D2DRectangle shape = new D2DRectangle()
                {
                    Size = new Size(5, 5), // TODO: Size?
                };

                var pixelPoint = this.Owner.ConvertGeographicToPixelCoordinate(pointModel.Location);
                shape.Location = pixelPoint;

                this.AddShape(shape, pointModel);
            }

            this.Owner.D2DSurface.SetShapesForLayer(this.shapes, new ShapeLayerParameters() { Id = this.id, ZIndex = this.zIndex, RenderPrecision = ShapeRenderPrecision.Double });
        }

        private void RenderPolylines(bool closed)
        {
            // Shape geometry models already defined - the D2DCanvas will scale them.
            if (this.Owner.D2DSurface.HasShapesForLayer(this.id))
            {
                return;
            }

            int minPoints = closed ? 3 : 2;

            foreach (IMapShape shapeModel in this.ShapeModels)
            {
                var shape2DModel = shapeModel as MapShape2DModel;
                if (shape2DModel == null)
                {
                    continue;
                }

                if (shape2DModel.Locations.Count == 1)
                {
                    if (shape2DModel.Locations[0].Count < minPoints)
                    {
                        continue;
                    }

                    var shape = new D2DPolyline();
                    shape.IsClosed = closed;
                    this.SetPolylinePoints(shape, shape2DModel.Locations[0]);
                    this.AddShape(shape, shape2DModel);
                }
                else
                {
                    // Use ShapeContainer for more than 1 polyline per model
                    var container = new D2DShapeContainer();
                    var childShapes = new List<D2DShape>();

                    foreach (LocationCollection locations in shape2DModel.Locations)
                    {
                        if (locations.Count < minPoints)
                        {
                            continue;
                        }

                        var shape = new D2DPolyline();
                        shape.IsClosed = closed;
                        this.SetPolylinePoints(shape, locations);

                        childShapes.Add(shape);
                    }

                    container.SetShapes(childShapes);

                    this.AddShape(container, shape2DModel);
                }
            }

            this.Owner.D2DSurface.SetShapesForLayer(this.shapes, new ShapeLayerParameters() { Id = this.id, ZIndex = this.zIndex, RenderPrecision = ShapeRenderPrecision.Double });
        }

        private void SetPolylinePoints(D2DPolyline polyline, LocationCollection locations)
        {
            // materialize the list to skip managed-to-unmanaged marshaling on every iteration
            var list = this.Owner.ConvertGeographicToPixelCoordinates(locations).ToList();
            polyline.SetPoints(list);
        }

        private void OnLabelAttributeChanged(object labelModel = null)
        {
            if (!this.IsTemplateApplied)
            {
                return;
            }

            string label;

            foreach (var shape in this.shapes)
            {
                if (labelModel != null && labelModel != shape.Model)
                {
                    continue;
                }

                label = this.GetShapeLabel(shape.Model as MapShapeModel);
                if (label == null)
                {
                    continue;
                }

                if (shape.Label == null)
                {
                    shape.Label = new D2DTextBlock() { Text = label };
                }
                else
                {
                    shape.Label.Text = label;
                }
            }
        }

        private D2DTextBlock GetShapeTextBlock(MapShapeModel model)
        {
            var text = this.GetShapeLabel(model);
            if (text == null)
            {
                return null;
            }

            return new D2DTextBlock()
            {
                Text = text
            };
        }

        private string GetShapeLabel(MapShapeModel model)
        {
            if (string.IsNullOrEmpty(this.labelAttributeNameCache))
            {
                return null;
            }

            object labelValue;
            if (!model.Attributes.TryGetValue(this.labelAttributeNameCache, out labelValue))
            {
                return null;
            }

            if (labelValue == null)
            {
                return null;
            }

            return labelValue.ToString();
        }

        private void ApplyShapeStyle(D2DShape shape)
        {
            (shape.Model as IMapShape).AttributeChanged -= this.MapShapeLayer_AttributeChanged;
            (shape.Model as IMapShape).AttributeChanged += this.MapShapeLayer_AttributeChanged;
            
            D2DShapeStyle normalStyle = this.normalStyleCache;
            D2DShapeStyle pointerOverStyle = this.pointerOverStyleCache;
            D2DShapeStyle selectedStyle = this.selectedStyleCache;

            if (this.colorizerCache != null && this.colorizerCache.IsProperlyInitialized)
            {
                var style = this.colorizerCache.GetShapeStyle(shape.Model as IMapShape);
                if (style != null)
                {
                    if (normalStyle != null)
                    {
                        if (style.Stroke == null)
                        {
                            style.Stroke = normalStyle.Stroke;
                        }
                        if (style.Foreground == null)
                        {
                            style.Foreground = normalStyle.Foreground;
                        }
                        if (style.StrokeThickness == 1)
                        {
                            style.StrokeThickness = normalStyle.StrokeThickness;
                        }
                    }

                    normalStyle = style;
                }
            }

            MapShapeStyleContext selectorContext = null;

            if (this.shapeStyleSelectorCache != null)
            {
                selectorContext = new MapShapeStyleContext()
                {
                    Shape = shape.Model as IMapShape,
                    UIState = shape.UIState
                };
                this.shapeStyleSelectorCache.SelectStyle(selectorContext, this);
                if (selectorContext.NormalStyle != null)
                {
                    normalStyle = selectorContext.NormalStyle;
                }
                if (selectorContext.PointerOverStyle != null)
                {
                    pointerOverStyle = selectorContext.PointerOverStyle;
                }
                if (selectorContext.SelectedStyle != null)
                {
                    selectedStyle = selectorContext.SelectedStyle;
                }
            }

            // TODO: We may run into a D2DFactory exception if the resources provided by the user are shared across different Map instances
            // TODO: This requires some refactoring, which will be handled after the Q3 release
            shape.NormalStyle = normalStyle;
            shape.PointerOverStyle = pointerOverStyle;
            shape.SelectedStyle = selectedStyle;

            this.ApplyShapeLabel(shape, selectorContext);
        }

        private void ApplyShapeLabel(D2DShape shape, MapShapeStyleContext selectorContext)
        {
            if (shape.Label == null)
            {
                return;
            }

            D2DTextStyle labelStyle = this.labelStyleCache;
            if (selectorContext != null && selectorContext.LabelStyle != null)
            {
                labelStyle = selectorContext.LabelStyle;
            }

            if (labelStyle != null)
            {
                shape.Label.Style = labelStyle;
            }

            this.ApplyShapeLabelStrategy(shape);
        }

        private void ApplyShapeLabelStrategy(D2DShape shape)
        {
            if (this.shapeLabelLayoutStrategyCache == null)
            {
                return;
            }

            var context = new MapShapeLabelLayoutContext()
            {
                Shape = shape.Model as IMapShape,
                ZoomLevel = this.Owner.ZoomLevel,
                Label = shape.Label.Text,
                Center = this.Owner.Center,
                RenderLocationOrigin = new Point(0.5, 0.5)
            };

            this.shapeLabelLayoutStrategyCache.Process(context, this);

            shape.LabelVisibility = context.Visibility;
            if (context.RenderLocation.HasValue)
            {
                var position = this.Owner.ConvertGeographicToPixelCoordinate(context.RenderLocation.Value);
                shape.LabelRenderPosition = position;
            }
            if (context.RenderLocationOrigin.HasValue)
            {
                shape.LabelRenderPositionOrigin = context.RenderLocationOrigin.Value;
            }
        }

        private void AddShape(D2DShape shape, MapShapeModel model)
        {
            // cross-references since we need to look-up a model from a shape and a shape from a model
            this.modelToVisualTable.Add(model, shape);
            shape.Model = model;

            // TODO: Extend label logic
            shape.Label = this.GetShapeTextBlock(model);
            this.ApplyShapeStyle(shape);

            this.shapes.Add(shape);
        }

        private void InvalidateUI(RadMap owner = null)
        {
            if (owner == null)
            {
                owner = this.Owner;
            }

            this.areModelsProcessed = false;

            if (owner != null)
            {
                foreach (var behavior in owner.Behaviors)
                {
                    behavior.OnShapeLayerCleared(this);
                }
            }

            // clear the cross-references
            foreach (var shape in this.shapes)
            {
                shape.Model = null;
            }
            this.modelToVisualTable.Clear();

            if (owner != null)
            {
                owner.D2DSurface.SetShapesForLayer(null, new ShapeLayerParameters() { Id = this.id, ZIndex = this.zIndex, RenderPrecision = ShapeRenderPrecision.Double });
            }
            this.shapes.Clear();

            if (this.colorizerCache != null)
            {
                this.colorizerCache.Reset();
            }
        }

        private void UpdateUIForUnprocessedModels()
        {
            if (this.ShapeModels != null && !this.areModelsProcessed)
            {
                this.UpdateUI();
            }
        }

        private void AttachSourceEvent()
        {
            if (this.Source != null)
            {
                this.sourceShapesPropertyChangedHandler = new WeakEventHandler<PropertyChangedEventArgs>(this.Source, this, KnownEvents.PropertyChanged);
            }
        }

        private void DetachSourceEvent()
        {
            if (this.sourceShapesPropertyChangedHandler != null)
            {
                this.sourceShapesPropertyChangedHandler.Unsubscribe();
                this.sourceShapesPropertyChangedHandler = null;
            }
        }

        private void PerformShapesUpdate(Action update)
        {
            if (!this.IsTemplateApplied || this.shapes.Count == 0 || this.Owner == null || !this.Owner.IsTemplateApplied)
            {
                return;
            }

            this.Owner.D2DSurface.BeginShapeUpdate();

            update();

            this.Owner.D2DSurface.EndShapeUpdate();
        }
    }
}