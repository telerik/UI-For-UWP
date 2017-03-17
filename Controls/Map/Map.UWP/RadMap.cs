using System;
using System.Collections.Generic;
using Telerik.Geospatial;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Drawing;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Map
{
    /// <summary>
    /// Represents a <see cref="RadControl"/> implementation that enables rich GeoSpatial data visualization.
    /// </summary>
    [ContentProperty(Name = "Layers")]
    public partial class RadMap : RadControl
    {
        /// <summary>
        /// Identifies the <see cref="Center"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register(nameof(Center), typeof(Location), typeof(RadMap), new PropertyMetadata(Location.Empty, OnCenterPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="ZoomLevel"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ZoomLevelProperty =
            DependencyProperty.Register(nameof(ZoomLevel), typeof(double), typeof(RadMap), new PropertyMetadata(1d, OnZoomLevelPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="MaxZoomLevel"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxZoomLevelProperty =
            DependencyProperty.Register(nameof(MaxZoomLevel), typeof(double), typeof(RadMap), new PropertyMetadata(20d, OnMaxZoomLevelPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="MinZoomLevel"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinZoomLevelProperty =
            DependencyProperty.Register(nameof(MinZoomLevel), typeof(double), typeof(RadMap), new PropertyMetadata(1d, OnMinZoomLevelPropertyChanged));

        internal Canvas adornerLayer;

        private const string LayoutRootPartName = "PART_LayoutRoot";
        private const string RenderSurfacePartName = "PART_RenderSurface";
        private const string D2DSurfacePartName = "PART_D2DSurface";
        private const string AdornerLayerPartName = "PART_AdornerLayer";

        private static readonly Size TileSize = new Size(256, 256);
        private static readonly double BaseViewportPixelWidth = TileSize.Width * 2;

        private MapLayerCollection layers;
        private List<MapLayer> unattachedPresenters;
        private Grid layoutRoot;
        private Canvas renderSurface;
        private D2DCanvas d2dSurface;
        private DoublePoint scrollOffset;

        private ISpatialReference spatialReference;
        private double viewportPixelWidth;
        private Size currentSize;
        private double zoomLevelCache = 1d;

        private LocationRect deferredBoundingRect = LocationRect.Empty;
        private LocationRect bounds;
        private bool isBoundsDirty = true;
        private bool arrangePassed;
        private bool updatingView;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadMap"/> class.
        /// </summary>
        public RadMap()
        {
            this.DefaultStyleKey = typeof(RadMap);

            this.layers = new MapLayerCollection(this);
            this.behaviors = new MapBehaviorCollection(this);
            this.unattachedPresenters = new List<MapLayer>();

            this.commandService = new MapCommandService(this);

            this.CanvasZoomFactor = 1;
            this.ViewportPixelWidth = BaseViewportPixelWidth;

            if (!DesignMode.DesignModeEnabled)
            {
                Application.Current.Suspending += this.OnApplicationSuspending;
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="RadMap"/> class.
        /// </summary>
        ~RadMap()
        {
            if (!DesignMode.DesignModeEnabled)
            {
                Application.Current.Suspending -= this.OnApplicationSuspending;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Location"/> value that represents the logical center of the map.
        /// </summary>
        public Location Center
        {
            get
            {
                return (Location)this.GetValue(CenterProperty);
            }
            set
            {
                this.SetValue(CenterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Double"/> value that represents the current zoom level of the map.
        /// </summary>
        public double ZoomLevel
        {
            get
            {
                return this.zoomLevelCache;
            }
            set
            {
                this.SetValue(ZoomLevelProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Double"/> value that defines the maximum value the <see cref="ZoomLevel"/> property can reach.
        /// </summary>
        public double MaxZoomLevel
        {
            get
            {
                return (double)this.GetValue(MaxZoomLevelProperty);
            }
            set
            {
                this.SetValue(MaxZoomLevelProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Double"/> value that defines the minimum value the <see cref="ZoomLevel"/> property can reach.
        /// </summary>
        public double MinZoomLevel
        {
            get
            {
                return (double)this.GetValue(MinZoomLevelProperty);
            }
            set
            {
                this.SetValue(MinZoomLevelProperty, value);
            }
        }

        /// <summary>
        /// Gets the collection with all the <see cref="MapLayer"/> instances currently available within the map.
        /// </summary>
        public MapLayerCollection Layers
        {
            get
            {
                return this.layers;
            }
        }

        /// <summary>
        /// Gets the current <see cref="ISpatialReference"/> implementation that is used by the map to visualize the geographical coordinates in the 2-D space.
        /// </summary>
        public ISpatialReference SpatialReference
        {
            get
            {
                if (this.spatialReference == null)
                {
                    this.spatialReference = new MercatorProjection();
                }

                return this.spatialReference;
            }
            ////TODO: Do we need a public setter?
            ////set
            ////{
            ////    this.spatialReference = value;
            ////}
        }

        /// <summary>
        /// Gets a <see cref="LocationRect"/> instance that defines the geographical boundaries of the currently visible area of the <see cref="RadMap"/> control.
        /// </summary>
        public LocationRect Bounds
        {
            get
            {
                if (!this.IsTemplateApplied)
                {
                    ISpatialReference projection = this.SpatialReference;
                    return new LocationRect(new Location(projection.MaxLatitude, projection.MinLongitude), new Location(projection.MinLatitude, projection.MaxLongitude));
                }

                if (this.isBoundsDirty)
                {
                    this.bounds = this.CalculateGeoBounds();

                    this.isBoundsDirty = false;
                }

                return this.bounds;
            }
        }

        internal Canvas RenderSurface
        {
            get
            {
                return this.renderSurface;
            }
        }

        internal double CanvasZoomFactor
        {
            get;
            set;
        }

        internal double ViewportPixelWidth
        {
            get
            {
                return this.viewportPixelWidth;
            }
            set
            {
                if (this.viewportPixelWidth != value)
                {
                    this.viewportPixelWidth = value;

                    this.UpdateViewportWidth();
                }
            }
        }

        internal double ViewportWidth
        {
            get;
            set;
        }

        internal DoublePoint ScrollOffset
        {
            get
            {
                return this.scrollOffset;
            }
        }

        internal Size CurrentSize
        {
            get
            {
                return this.currentSize;
            }
        }

        internal double AspectRatio
        {
            get
            {
                if (this.currentSize.Height == 0 || this.currentSize.Width == 0)
                {
                    return 1d;
                }

                return this.currentSize.Height / this.currentSize.Width;
            }
        }

        internal D2DCanvas D2DSurface
        {
            get
            {
                return this.d2dSurface;
            }
        }

        /// <summary>
        /// Initiates a hit test on the specified <see cref="Point"/> location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="shapeLayer">The <see cref="MapShapeLayer"/> instance whose shapes to hit test. 
        /// Default value is null i.e. hit testing is performed on all layers (starting from the top-most one) and the first positive match is returned.</param>
        /// <returns></returns>
        public IMapShape HitTest(Point location, MapShapeLayer shapeLayer = null)
        {
            int layerZIndex = -1;
            if (shapeLayer != null)
            {
                layerZIndex = shapeLayer.ZIndex;
            }

            var result = this.D2DSurface.HitTest(location, layerZIndex);
            if (result != null)
            {
                return result.Model as IMapShape;
            }

            return null;
        }

        /// <summary>
        /// Sets the provided <see cref="LocationRect"/> value as the current view of the map.
        /// </summary>
        /// <param name="boundingRect"></param>
        public void SetView(LocationRect boundingRect)
        {
            boundingRect = this.CoerceLocationRect(boundingRect);
            if (boundingRect.IsEmpty)
            {
                return;
            }

            if (!this.arrangePassed)
            {
                this.deferredBoundingRect = boundingRect;
                return;
            }

            var topLeft = this.SpatialReference.ConvertGeographicToLogicalCoordinate(boundingRect.Northwest);
            var bottomRight = this.SpatialReference.ConvertGeographicToLogicalCoordinate(boundingRect.Southeast);
            var logicalCenter = new DoublePoint() { X = (topLeft.X + bottomRight.X) / 2d, Y = (topLeft.Y + bottomRight.Y) / 2d };

            Location center = this.SpatialReference.ConvertLogicalToGeographicCoordinate(logicalCenter);

            double viewportWidth = bottomRight.X - topLeft.X;
            double viewportHeight = bottomRight.Y - topLeft.Y;

            double zoomWidth = Math.Log(this.currentSize.Width / TileSize.Width / viewportWidth, 2d);
            double zoomHeight = Math.Log(this.currentSize.Height / TileSize.Height / viewportHeight, 2d);
            double zoomLevel = Math.Min(zoomWidth, zoomHeight);

            var context = new ViewChangedContext()
            {
                NewCenter = center,
                NewZoomLevel = zoomLevel,
                PreviousCenter = this.Center,
                PreviousZoomLevel = this.ZoomLevel
            };

            this.OnViewChanged(context);
        }

        /// <summary>
        /// Gets the <see cref="MapShapeLayer"/> that the <see cref="IMapShape"/> instance specified as parameter belongs to.
        /// </summary>
        /// <param name="shape">The <see cref="IMapShape"/> instance.</param>
        public MapShapeLayer GetLayerForShape(IMapShape shape)
        {
            if (!this.IsTemplateApplied)
            {
                return null;
            }

            foreach (var layer in this.layers)
            {
                var shapeLayer = layer as MapShapeLayer;
                if (shapeLayer != null && shapeLayer.ContainsShape(shape))
                {
                    return shapeLayer;
                }
            }

            return null;
        }

        /// <summary>
        /// Converts the specified physical point to its geographic equivalent. The provided physical point is relative to the map's TopLeft position.
        /// </summary>
        /// <param name="point">The physical point (typically associated with a Pointer contact).</param>
        /// <returns></returns>
        public Location ConvertPhysicalToGeographicCoordinate(DoublePoint point)
        {
            if (!this.IsTemplateApplied)
            {
                return Location.Empty;
            }

            var logicalCoordinate = this.ConvertPixelToLogicalCoordinate(point);

            return this.SpatialReference.ConvertLogicalToGeographicCoordinate(logicalCoordinate);
        }

        /// <summary>
        /// Converts the specified geographic location to its physical equivalent.
        /// </summary>
        /// <param name="location">The geographic location.</param>
        /// <returns></returns>
        public DoublePoint ConvertGeographicToPhysicalCoordinate(Location location)
        {
            var logicalPoint = this.SpatialReference.ConvertGeographicToLogicalCoordinate(location);
            var logicalOrigin = this.GetLogicalOrigin();

            double pixelFactorX = this.ViewportWidth / this.currentSize.Width;
            double pixelFactorY = this.ViewportWidth * this.AspectRatio / this.currentSize.Height;

            return new DoublePoint() { X = (logicalPoint.X - logicalOrigin.X) / pixelFactorX, Y = (logicalPoint.Y - logicalOrigin.Y) / pixelFactorY };
        }

        internal D2DShape FindShapeForModel(IMapShape model)
        {
            var layer = this.GetLayerForShape(model);
            if (layer != null)
            {
                return layer.GetShapeForModel(model);
            }

            return null;
        }

        internal void OnPresenterAdded(MapLayer presenter)
        {
            if (this.renderSurface == null)
            {
                this.unattachedPresenters.Add(presenter);
                return;
            }

            this.renderSurface.Children.Add(presenter);
            presenter.Attach(this);
        }

        internal void OnPresenterRemoved(MapLayer presenter)
        {
            if (this.renderSurface == null)
            {
                this.unattachedPresenters.Remove(presenter);
                return;
            }

            this.renderSurface.Children.Remove(presenter);
            presenter.Detach();
        }

        internal DoublePoint ConvertGeographicToPixelCoordinate(Location location)
        {
            var logicalPoint = this.SpatialReference.ConvertGeographicToLogicalCoordinate(location);

            return new DoublePoint() { X = logicalPoint.X * BaseViewportPixelWidth, Y = logicalPoint.Y * BaseViewportPixelWidth };
        }

        internal IEnumerable<DoublePoint> ConvertGeographicToPixelCoordinates(LocationCollection locations)
        {
            foreach (var location in locations)
            {
                yield return this.ConvertGeographicToPixelCoordinate(location);
            }
        }

        internal DoublePoint ConvertPixelToLogicalCoordinate(DoublePoint point)
        {
            DoublePoint logicalOrigin = this.GetLogicalOrigin();

            double pixelFactorX = this.ViewportWidth / this.currentSize.Width;
            double pixelFactorY = this.ViewportWidth * this.AspectRatio / this.currentSize.Height;

            return new DoublePoint() { X = (point.X * pixelFactorX) + logicalOrigin.X, Y = (point.Y * pixelFactorY) + logicalOrigin.Y };
        }

        internal void OnViewChanged(ViewChangedContext context)
        {
            this.SetView(context.NewZoomLevel, context.NewCenter);

            this.NotifyViewChanged(context);
        }

        internal MapLayer FindLayerById(int layerId)
        {
            foreach (var layer in this.layers)
            {
                if (layer.Id == layerId)
                {
                    return layer;
                }
            }

            return null;
        }

        /// <summary>
        /// Provides the behavior for the Arrange pass of layout. Classes can override this method to define their own Arrange pass behavior.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        /// <returns>
        /// The actual size that is used after the element is arranged in layout.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Size resultSize = base.ArrangeOverride(finalSize);

            this.arrangePassed = true;

            return resultSize;
        }

        /// <summary>
        /// Called when the Framework <see cref="M:OnApplyTemplate" /> is called. Inheritors should override this method should they have some custom template-related logic.
        /// This is done to ensure that the <see cref="P:IsTemplateApplied" /> property is properly initialized.
        /// </summary>
        /// <returns></returns>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.layoutRoot = this.GetTemplatePartField<Grid>(LayoutRootPartName);
            applied = applied && this.layoutRoot != null;

            this.renderSurface = this.GetTemplatePartField<Canvas>(RenderSurfacePartName);
            applied = applied && this.renderSurface != null;

            this.d2dSurface = this.GetTemplatePartField<D2DCanvas>(D2DSurfacePartName);
            applied = applied && this.d2dSurface != null;

            this.adornerLayer = this.GetTemplatePartField<Canvas>(AdornerLayerPartName);
            applied = applied && this.adornerLayer != null;

            if (applied)
            {
                foreach (MapLayer presenter in this.unattachedPresenters)
                {
                    this.renderSurface.Children.Add(presenter);
                    presenter.Attach(this);
                }
                this.unattachedPresenters.Clear();
            }

            return applied;
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate" /> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            this.EnsureZoomLevelInRange();
            this.UpdateZoomLevel();

            this.d2dSurface.ZoomFactor = this.CanvasZoomFactor;
            this.d2dSurface.ViewportOrigin = this.scrollOffset;

            foreach (var behavior in this.behaviors)
            {
                behavior.OnMapTemplateApplied();
            }

            this.layoutRoot.SizeChanged += this.OnLayoutRootSizeChanged;
        }

        /// <summary>
        /// Unapplies the current control template. Occurs when a template has already been applied and a new one is applied.
        /// </summary>
        protected override void UnapplyTemplateCore()
        {
            this.layoutRoot.SizeChanged -= this.OnLayoutRootSizeChanged;

            base.UnapplyTemplateCore();
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadMapAutomationPeer(this);
        }

        private static void OnCenterPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var map = (RadMap)sender;

            Location coercedCenter = map.CoerceLocation(map.Center);
            if (map.Center != coercedCenter)
            {
                map.Center = coercedCenter;
                return;
            }

            if (map.IsTemplateApplied)
            {
                map.isBoundsDirty = true;

                map.UpdateScrollOffset();
            }
        }

        private static void OnZoomLevelPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var map = (RadMap)sender;
            map.zoomLevelCache = (double)args.NewValue;
            map.isBoundsDirty = true;

            if (map.IsInternalPropertyChange || !map.IsTemplateApplied)
            {
                return;
            }

            map.UpdateZoomLevel();

            if (!map.updatingView)
            {
                map.UpdateScrollOffset();
            }
        }

        private static void OnMinZoomLevelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var map = d as RadMap;
            if (map.IsInternalPropertyChange)
            {
                return;
            }

            var newValue = (double)e.NewValue;
            if (newValue < 1d)
            {
                map.MinZoomLevel = 1;
                return;
            }

            if (!map.IsTemplateApplied)
            {
                // zoom range will be handled upon TemplateApplied to allow 
                return;
            }

            map.EnsureZoomLevelInRange();
        }

        private static void OnMaxZoomLevelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var map = d as RadMap;
            if (map.IsInternalPropertyChange)
            {
                return;
            }

            if (!map.IsTemplateApplied)
            {
                // zoom range will be handled upon TemplateApplied to allow 
                return;
            }

            map.EnsureZoomLevelInRange();
        }

        private Location CoerceLocation(Location locationToCoerce)
        {
            Location coercedLocation = locationToCoerce;
            if (locationToCoerce.Latitude < this.SpatialReference.MinLatitude)
            {
                coercedLocation.Latitude = this.SpatialReference.MinLatitude;
            }
            else if (locationToCoerce.Latitude > this.SpatialReference.MaxLatitude)
            {
                coercedLocation.Latitude = this.SpatialReference.MaxLatitude;
            }

            if (locationToCoerce.Longitude < this.SpatialReference.MinLongitude)
            {
                coercedLocation.Longitude = this.SpatialReference.MinLongitude;
            }
            else if (locationToCoerce.Longitude > this.SpatialReference.MaxLongitude)
            {
                coercedLocation.Longitude = this.SpatialReference.MaxLongitude;
            }

            return coercedLocation;
        }

        private LocationRect CoerceLocationRect(LocationRect boundingRect)
        {
            Location coercedNorthwest = boundingRect.Northwest;
            Location coercedSoutheast = boundingRect.Southeast;

            coercedNorthwest = this.CoerceLocation(coercedNorthwest);
            coercedSoutheast = this.CoerceLocation(coercedSoutheast);

            if (coercedNorthwest.Latitude <= coercedSoutheast.Latitude ||
                coercedNorthwest.Longitude >= coercedSoutheast.Longitude)
            {
                return LocationRect.Empty;
            }

            if (coercedNorthwest != boundingRect.Northwest ||
                coercedSoutheast != boundingRect.Southeast)
            {
                return new LocationRect(coercedNorthwest, coercedSoutheast);
            }

            return boundingRect;
        }

        private void OnLayoutRootSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.currentSize = e.NewSize;

            if (this.IsTemplateApplied)
            {
                this.layoutRoot.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, this.currentSize.Width, this.currentSize.Height) };
            }

            this.isBoundsDirty = true;

            this.UpdateViewportWidth();
            this.UpdateScrollOffset();

            if (!this.deferredBoundingRect.IsEmpty)
            {
                this.SetView(this.deferredBoundingRect);
                this.deferredBoundingRect = LocationRect.Empty;
            }
        }

        private void UpdateScrollOffset()
        {
            var center = this.SpatialReference.ConvertGeographicToLogicalCoordinate(this.Center);

            this.scrollOffset = new DoublePoint() { X = this.currentSize.Width / 2 - this.ViewportPixelWidth * center.X, Y = this.currentSize.Height / 2 - this.ViewportPixelWidth * center.Y };
            if (this.d2dSurface != null)
            {
                this.d2dSurface.ViewportOrigin = this.scrollOffset;
            }

            if (this.IsTemplateApplied && !this.updatingView)
            {
                foreach (var layer in this.layers)
                {
                    layer.OnScrollOffsetChanged();
                }
            }
        }

        private void UpdateViewportPixelWidth()
        {
            this.ViewportPixelWidth = TileSize.Width * Math.Pow(2d, this.ZoomLevel);
        }

        private void UpdateViewportWidth()
        {
            this.ViewportWidth = this.currentSize.Width / this.ViewportPixelWidth;
        }

        private void UpdateCanvasZoomFactor()
        {
            this.CanvasZoomFactor = this.ViewportPixelWidth / BaseViewportPixelWidth;
            if (this.d2dSurface != null)
            {
                this.d2dSurface.ZoomFactor = (float)this.CanvasZoomFactor;
            }
        }

        private DoublePoint GetLogicalOrigin()
        {
            var logicalOrigin = this.SpatialReference.ConvertGeographicToLogicalCoordinate(this.Center);

            double targetWidth = this.ViewportWidth;
            double targetHeight = this.ViewportWidth * this.AspectRatio;

            logicalOrigin.X -= targetWidth / 2;
            logicalOrigin.Y -= targetHeight / 2;

            return logicalOrigin;
        }

        private void UpdateZoomLevel()
        {
            double oldZoom = this.ZoomLevel;
            if (!this.EnsureZoomLevelInRange())
            {
                if (oldZoom == this.ZoomLevel)
                {
                    // the zoom is clamped back to its previous value - e.g. MaxZoomLevel
                    return;
                }
            }

            this.UpdateViewportPixelWidth();
            this.UpdateCanvasZoomFactor();

            if (!this.updatingView)
            {
                foreach (MapLayer layer in this.Layers)
                {
                    layer.OnZoomChanged();
                }
            }
        }

        private bool EnsureZoomLevelInRange()
        {
            double maxZoom = this.MaxZoomLevel;
            double minZoom = this.MinZoomLevel;

            if (maxZoom < minZoom)
            {
                this.ChangePropertyInternally(MaxZoomLevelProperty, minZoom);
                maxZoom = minZoom;
            }

            bool isInRange = true;

            if (this.ZoomLevel < minZoom)
            {
                this.ChangePropertyInternally(ZoomLevelProperty, minZoom);
                isInRange = false;
            }
            else if (this.ZoomLevel > maxZoom)
            {
                this.ChangePropertyInternally(ZoomLevelProperty, maxZoom);
                isInRange = false;
            }

            return isInRange;
        }

        private void SetView(double zoom, Location center)
        {
            this.updatingView = true;

            this.ZoomLevel = zoom;
            this.Center = center;

            this.updatingView = false;
        }

        /// <summary>
        /// Calculates the geo bounds.
        /// </summary>
        /// <returns></returns>
        private LocationRect CalculateGeoBounds()
        {
            DoublePoint topLeft = this.ConvertPixelToLogicalCoordinate(new DoublePoint() { X = 0, Y = 0 });
            DoublePoint bottomRight = this.ConvertPixelToLogicalCoordinate(new DoublePoint() { X = this.currentSize.Width, Y = this.currentSize.Height });

            Location northwest = this.SpatialReference.ConvertLogicalToGeographicCoordinate(topLeft);
            Location southeast = this.SpatialReference.ConvertLogicalToGeographicCoordinate(bottomRight);

            northwest = this.CoerceLocation(northwest);
            southeast = this.CoerceLocation(southeast);

            return new LocationRect(northwest, southeast);
        }

        private void OnApplicationSuspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            if (this.D2DSurface != null)
            {
                this.D2DSurface.CleanUpOnSuspend();
            }
        }
    }
}
