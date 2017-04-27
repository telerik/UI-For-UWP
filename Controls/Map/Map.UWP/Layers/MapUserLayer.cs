using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Telerik.Core;
using Telerik.Core.Data;

using Telerik.Geospatial;
using Telerik.UI.Drawing;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Map
{
    /// <summary>
    /// Represents a specific <see cref="MapLayer"/> that allows for user-defined locations to be visualized through data templates.
    /// </summary>
    public class MapUserLayer : MapLayer, IWeakEventListener
    {
        /// <summary>
        /// Identifies the <see cref="ItemTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(MapUserLayer), new PropertyMetadata(null, OnItemTemplatePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="ItemTemplateSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateSelectorProperty =
            DependencyProperty.Register(nameof(ItemTemplateSelector), typeof(DataTemplateSelector), typeof(MapUserLayer), new PropertyMetadata(null, OnItemTemplateSelectorPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="ItemsSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(MapUserLayer), new PropertyMetadata(null, OnItemsSourcePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="LocationPropertyName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LocationPropertyNameProperty =
            DependencyProperty.Register(nameof(LocationPropertyName), typeof(string), typeof(MapUserLayer), new PropertyMetadata(null, OnLocationPropertyNamePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="LocationOriginPropertyName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LocationOriginPropertyNameProperty =
            DependencyProperty.Register(nameof(LocationOriginPropertyName), typeof(string), typeof(MapUserLayer), new PropertyMetadata(null, OnLocationOriginPropertyNamePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="MinZoomPropertyName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinZoomPropertyNameProperty =
            DependencyProperty.Register(nameof(MinZoomPropertyName), typeof(string), typeof(MapUserLayer), new PropertyMetadata(null, OnMinZoomPropertyNamePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="MaxZoomPropertyName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxZoomPropertyNameProperty =
            DependencyProperty.Register(nameof(MaxZoomPropertyName), typeof(string), typeof(MapUserLayer), new PropertyMetadata(null, OnMaxZoomPropertyNamePropertyChanged));

        private static readonly Size InfinitySize = new Size(double.PositiveInfinity, double.PositiveInfinity);

        private Canvas renderSurface;
        private Queue<ContentPresenter> recycledPresenters;
        private Dictionary<object, ContentPresenter> presenterByItemTable;
        private IEnumerable itemsSourceCache;
        private DataTemplate itemTemplateCache;
        private DataTemplateSelector itemTemplateSelectorCache;
        private WeakEventHandler<NotifyCollectionChangedEventArgs> collectionChangedEventHandler;

        private PropertyValueLookup locationLookupCache;
        private PropertyValueLookup minZoomLookupCache;
        private PropertyValueLookup maxZoomLookupCache;
        private PropertyValueLookup locationOriginLookupCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapUserLayer"/> class.
        /// </summary>
        public MapUserLayer()
        {
            this.DefaultStyleKey = typeof(MapUserLayer);

            this.recycledPresenters = new Queue<ContentPresenter>();
            this.presenterByItemTable = new Dictionary<object, ContentPresenter>();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="MapUserLayer"/> class.
        /// </summary>
        ~MapUserLayer()
        {
            if (this.collectionChangedEventHandler != null)
            {
                this.collectionChangedEventHandler.Unsubscribe();
                this.collectionChangedEventHandler = null;
            }
        }

        /// <summary>
        /// Gets or sets the collection of data items to be visualized by the layer.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get
            {
                return this.itemsSourceCache;
            }
            set
            {
                this.SetValue(ItemsSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> that defines the visual representation of each data item present within the <see cref="ItemsSource"/>.
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get
            {
                return this.itemTemplateCache;
            }
            set
            {
                this.SetValue(ItemTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplateSelector"/> that may be used to provide context-based appearance for each data item within the <see cref="ItemsSource"/>.
        /// </summary>
        public DataTemplateSelector ItemTemplateSelector
        {
            get
            {
                return this.itemTemplateSelectorCache;
            }
            set
            {
                this.SetValue(ItemTemplateSelectorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the name of the property that contains the GeoSpatial Location value for each item in the ItemsSource.
        /// </summary>
        /// <remarks>
        /// The expected return value of the property is of type <see cref="Telerik.Geospatial.Location"/>.
        /// </remarks>
        public string LocationPropertyName
        {
            get
            {
                if (this.locationLookupCache != null)
                {
                    return this.locationLookupCache.PropertyName;
                }

                return null;
            }
            set
            {
                this.SetValue(LocationPropertyNameProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the name of the property that defines the origin of the item relative to the Geo Location of the item.
        /// A value of (0.5, 0.5) will center the visual representation of each item over its geographical location.
        /// </summary>
        /// <remarks>
        /// The expected return value of the property is of type <see cref="Windows.Foundation.Point(double, double)"/>.
        /// </remarks>
        public string LocationOriginPropertyName
        {
            get
            {
                if (this.locationOriginLookupCache != null)
                {
                    return this.locationOriginLookupCache.PropertyName;
                }

                return null;
            }
            set
            {
                this.SetValue(LocationOriginPropertyNameProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the name of the property that contains the minimum zoom level needed for the item to be displayed.
        /// </summary>
        /// <remarks>
        /// The expected return value of the property is of type <see cref="System.Double"/>.
        /// </remarks>
        public string MinZoomPropertyName
        {
            get
            {
                if (this.minZoomLookupCache != null)
                {
                    return this.minZoomLookupCache.PropertyName;
                }

                return null;
            }
            set
            {
                this.SetValue(MinZoomPropertyNameProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the name of the property that contains the maximum zoom level needed for the item to be displayed.
        /// </summary>
        /// <remarks>
        /// The expected return value of the property is of type <see cref="System.Double"/>.
        /// </remarks>
        public string MaxZoomPropertyName
        {
            get
            {
                if (this.maxZoomLookupCache != null)
                {
                    return this.maxZoomLookupCache.PropertyName;
                }

                return null;
            }
            set
            {
                this.SetValue(MaxZoomPropertyNameProperty, value);
            }
        }

        void IWeakEventListener.ReceiveEvent(object sender, object args)
        {
            var changeArgs = args as NotifyCollectionChangedEventArgs;
            switch (changeArgs.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    foreach (var item in changeArgs.OldItems)
                    {
                        if (this.presenterByItemTable.ContainsKey(item))
                        {
                            this.RecycleItem(item);
                        }
                    }
                    this.ScheduleUpdate();
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.InvalidateUI();
                    break;
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Move:
                    this.ScheduleUpdate();
                    break;
            }
        }

        internal override void UpdateUI()
        {
            base.UpdateUI();

            if (this.itemsSourceCache == null)
            {
                return;
            }

            DoublePoint pixelPosition;
            IMapDataItem mapItem;
            
            // TODO: This is not optimal, currently we are iterating the entire source
            // TODO: Instead, we may sort the set by X & Y position and track offset changes
            foreach (var item in this.itemsSourceCache)
            {
                if (item == null)
                {
                    continue;
                }

                mapItem = item as IMapDataItem;
                if (mapItem != null)
                {
                    pixelPosition = this.GetMapItemPixelPosition(mapItem);
                    if (!this.IsMapItemVisible(mapItem, pixelPosition))
                    {
                        this.RecycleItem(mapItem);
                    }
                    else
                    {
                        ArrangeItem(this.RealizeItem(mapItem), pixelPosition, mapItem.LocationOrigin);
                    }
                }
                else if (this.locationLookupCache != null)
                {
                    pixelPosition = this.GetItemPixelPosition(item);
                    if (!this.IsItemVisible(item, pixelPosition))
                    {
                        this.RecycleItem(item);
                    }
                    else
                    {
                        this.ArrangeItem(item, this.RealizeItem(item), pixelPosition);
                    }
                }
            }
        }

        internal override void OnScrollOffsetChanged()
        {
            base.OnScrollOffsetChanged();

            this.UpdateUI();
        }

        internal override void OnViewChanged(ViewChangedContext context)
        {
            base.OnViewChanged(context);

            this.UpdateUI();
        }

        internal DoublePoint GetMapItemPixelPosition(IMapDataItem mapItem)
        {
            var pixelPosition = this.Owner.ConvertGeographicToPixelCoordinate(mapItem.Location);
            return this.ApplyMapTransformToPixelPosition(pixelPosition);
        }

        /// <summary>
        /// Called when the Framework <see cref="M:OnApplyTemplate" /> is called. Inheritors should override this method should they have some custom template-related logic.
        /// This is done to ensure that the <see cref="P:IsTemplateApplied" /> property is properly initialized.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.renderSurface = this.GetTemplatePartField<Canvas>("PART_RenderSurface");
            applied = applied && this.renderSurface != null;

            return applied;
        }

        /// <summary>
        /// Called within the handler of the <see cref="E:Unloaded" /> event. Allows inheritors to provide their specific logic.
        /// </summary>
        protected override void UnloadCore()
        {
            base.UnloadCore();

            // TODO: This might not be optimal in case Page Caching
            this.InvalidateUI();
        }

        private static void OnItemTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var layer = d as MapUserLayer;
            layer.itemTemplateCache = e.NewValue as DataTemplate;
            layer.UpdateItemTemplate();
        }

        private static void OnItemTemplateSelectorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var layer = d as MapUserLayer;
            layer.itemTemplateSelectorCache = e.NewValue as DataTemplateSelector;
            layer.UpdateItemTemplate();
        }

        private static void OnItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var layer = d as MapUserLayer;
            layer.ChangeItemsSource(e.NewValue);
        }

        private static void OnLocationPropertyNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var layer = d as MapUserLayer;
            layer.UpdateLookupField(ref layer.locationLookupCache, (string)e.NewValue);
        }

        private static void OnLocationOriginPropertyNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var layer = d as MapUserLayer;
            layer.UpdateLookupField(ref layer.locationOriginLookupCache, (string)e.NewValue);
        }

        private static void OnMinZoomPropertyNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var layer = d as MapUserLayer;
            layer.UpdateLookupField(ref layer.minZoomLookupCache, (string)e.NewValue);
        }

        private static void OnMaxZoomPropertyNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var layer = d as MapUserLayer;
            layer.UpdateLookupField(ref layer.maxZoomLookupCache, (string)e.NewValue);
        }

        private static void ArrangeItem(ContentPresenter presenter, DoublePoint pixelPosition, Point locationOrigin)
        {
            var desiredSize = presenter.DesiredSize;
            Canvas.SetLeft(presenter, pixelPosition.X - (desiredSize.Width * locationOrigin.X));
            Canvas.SetTop(presenter, pixelPosition.Y - (desiredSize.Height * locationOrigin.Y));
        }

        private void RecycleItem(object item)
        {
            ContentPresenter presenter;
            if (!this.presenterByItemTable.TryGetValue(item, out presenter))
            {
                return;
            }

            this.presenterByItemTable.Remove(item);
            this.recycledPresenters.Enqueue(presenter);
            presenter.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private ContentPresenter RealizeItem(object item)
        {
            ContentPresenter presenter;
            if (this.presenterByItemTable.TryGetValue(item, out presenter))
            {
                // item is already realized
                return presenter;
            }

            if (this.recycledPresenters.Count > 0)
            {
                presenter = this.recycledPresenters.Dequeue();
                presenter.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                presenter = new ContentPresenter();
                presenter.ContentTemplate = this.GetItemTemplate(item);
                this.renderSurface.Children.Add(presenter);
            }

            presenter.Content = item;
            presenter.Measure(InfinitySize);
            this.presenterByItemTable.Add(item, presenter);

            return presenter;
        }

        private void ChangeItemsSource(object newValue)
        {
            if (this.collectionChangedEventHandler != null)
            {
                this.collectionChangedEventHandler.Unsubscribe();
                this.collectionChangedEventHandler = null;
            }

            this.itemsSourceCache = newValue as IEnumerable;
            if (this.itemsSourceCache is INotifyCollectionChanged)
            {
                this.collectionChangedEventHandler = new WeakEventHandler<NotifyCollectionChangedEventArgs>(this.itemsSourceCache, this, KnownEvents.CollectionChanged);
            }

            // TODO: Hook property changed for each item
            if (this.IsTemplateApplied)
            {
                this.InvalidateUI();
                this.ScheduleUpdate();
            }
        }

        private void UpdateItemTemplate()
        {
            foreach (var pair in this.presenterByItemTable)
            {
                pair.Value.ContentTemplate = this.GetItemTemplate(pair.Key);
            }
        }

        private void InvalidateUI()
        {
            this.recycledPresenters.Clear();
            this.presenterByItemTable.Clear();

            if (this.renderSurface != null)
            {
                this.renderSurface.Children.Clear();
            }

            this.ScheduleUpdate();
        }

        private DataTemplate GetItemTemplate(object item)
        {
            if (this.itemTemplateCache != null)
            {
                return this.itemTemplateCache;
            }

            if (this.itemTemplateSelectorCache != null)
            {
                return this.itemTemplateSelectorCache.SelectTemplate(item, this);
            }

            return null;
        }

        private DoublePoint GetItemPixelPosition(object item)
        {
            object value = this.locationLookupCache.GetValueForItem(item);
            if (!(value is Location))
            {
                throw new ArgumentException("Expected value of type Telerik.Geospatial.Location");
            }

            var pixelPosition = this.Owner.ConvertGeographicToPixelCoordinate((Location)value);
            return this.ApplyMapTransformToPixelPosition(pixelPosition);
        }

        private DoublePoint ApplyMapTransformToPixelPosition(DoublePoint pixelPosition)
        {
            pixelPosition.X *= this.Owner.CanvasZoomFactor;
            pixelPosition.Y *= this.Owner.CanvasZoomFactor;
            pixelPosition.X += this.Owner.ScrollOffset.X;
            pixelPosition.Y += this.Owner.ScrollOffset.Y;

            return pixelPosition;
        }

        private bool IsItemVisible(object item, DoublePoint pixelPosition)
        {
            if (pixelPosition.X < 0 || pixelPosition.Y < 0 ||
                pixelPosition.X > this.Owner.CurrentSize.Width ||
                pixelPosition.Y > this.Owner.CurrentSize.Height)
            {
                return false;
            }

            object value;

            if (this.minZoomLookupCache != null)
            {
                value = this.minZoomLookupCache.GetValueForItem(item);
                if (!(value is double))
                {
                    throw new ArgumentException("Expected value of type System.Double");
                }

                if (this.Owner.ZoomLevel < (double)value)
                {
                    return false;
                }
            }

            if (this.maxZoomLookupCache != null)
            {
                value = this.maxZoomLookupCache.GetValueForItem(item);
                if (!(value is double))
                {
                    throw new ArgumentException("Expected value of type System.Double");
                }

                if (this.Owner.ZoomLevel > (double)value)
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsMapItemVisible(IMapDataItem mapItem, DoublePoint pixelPosition)
        {
            if (pixelPosition.X < 0 || pixelPosition.Y < 0 ||
                pixelPosition.X > this.Owner.CurrentSize.Width ||
                pixelPosition.Y > this.Owner.CurrentSize.Height)
            {
                return false;
            }

            if (this.Owner.ZoomLevel < mapItem.MinZoom)
            {
                return false;
            }

            if (this.Owner.ZoomLevel > mapItem.MaxZoom)
            {
                return false;
            }

            return true;
        }

        private void ArrangeItem(object item, ContentPresenter presenter, DoublePoint pixelPosition)
        {
            // arrange the item, having in mind the PositionOrigin property of the data item
            Point locationOrigin = new Point(0, 0);
            if (this.locationOriginLookupCache != null)
            {
                object value = this.locationOriginLookupCache.GetValueForItem(item);
                if (!(value is Point))
                {
                    throw new ArgumentException("Expected value of type Windows.Foundation.Point");
                }

                locationOrigin = (Point)value;
            }

            ArrangeItem(presenter, pixelPosition, locationOrigin);
        }

        private void UpdateLookupField(ref PropertyValueLookup field, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Expected a valid System.String value.");
            }
            field = new PropertyValueLookup(path);
            this.InvalidateUI();
        }
    }
}
