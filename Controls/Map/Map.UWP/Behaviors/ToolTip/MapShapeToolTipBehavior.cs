using System;
using System.Collections.Generic;
using Telerik.Geospatial;
using Telerik.UI.Drawing;
using Telerik.UI.Xaml.Controls.Map.Primitives;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Map
{
    /// <summary>
    /// Represents a <see cref="MapBehavior"/> that provides a context-sensitive information on a per map shape basis. Applicable when <see cref="MapShapeLayer"/> are added to a <see cref="RadMap"/> instance.
    /// </summary>
    public class MapShapeToolTipBehavior : MapBehavior
    {
        /// <summary>
        /// Identifies the ContentTemplate attached property.
        /// Usually this property is used by <see cref="MapShapeLayer"/> to define different toolTips on a per layer basis.
        /// </summary>
        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.RegisterAttached("ContentTemplate", typeof(DataTemplate), typeof(MapShapeToolTipBehavior), new PropertyMetadata(null));

        private Popup toolTip;
        private MapToolTip toolTipContent;
        private DispatcherTimer delayTimer;
        private Point displayPosition;
        private Size contentSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapShapeToolTipBehavior"/> class.
        /// </summary>
        public MapShapeToolTipBehavior()
        {
            this.toolTip = new Popup();

            this.toolTipContent = new MapToolTip();
            this.toolTipContent.SizeChanged += this.OnToolTipContent_SizeChanged;

            this.toolTip.Child = this.toolTipContent;

            this.delayTimer = new DispatcherTimer();
            this.delayTimer.Interval = TimeSpan.FromMilliseconds(0);
            this.delayTimer.Tick += this.OnDelayTimer_Tick;

            this.HorizontalAlignment = HorizontalAlignment.Right;
            this.VerticalAlignment = VerticalAlignment.Top;
        }

        /// <summary>
        /// Gets or sets the offset to be applied when the toolTip position is calculated.
        /// </summary>
        public Point TouchOverhang
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the delay to be applied before the toolTip is displayed.
        /// </summary>
        public TimeSpan ShowDelay
        {
            get
            {
                return this.delayTimer.Interval;
            }
            set
            {
                this.delayTimer.Interval = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the toolTip is currently displayed.
        /// </summary>
        public bool IsOpen
        {
            get
            {
                return this.toolTip.IsOpen;
            }
        }

        /// <summary>
        /// Gets the <see cref="DataTemplate"/> instance associated with the specified <see cref="DependencyObject"/> instance.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static DataTemplate GetContentTemplate(DependencyObject instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException();
            }

            return instance.GetValue(ContentTemplateProperty) as DataTemplate;
        }

        /// <summary>
        /// Sets the provided <see cref="DataTemplate"/> instance to the the specified <see cref="DependencyObject"/> instance.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="template"></param>
        public static void SetContentTemplate(DependencyObject instance, DataTemplate template)
        {
            if (instance == null)
            {
                throw new ArgumentNullException();
            }

            instance.SetValue(ContentTemplateProperty, template);
        }

        /// <summary>
        /// Exposed for testing purposes.
        /// </summary>
        /// <param name="position">The position.</param>
        internal void HandleTap(Point position)
        {
            this.InitializeToolTip();

            this.UpdateToolTip(position);
        }

        /// <summary>
        /// Exposed for testing purposes.
        /// </summary>
        internal virtual void UpdateToolTipCore()
        {
            // reset the timer
            this.delayTimer.Stop();
            this.delayTimer.Start();
        }

        /// <summary>
        /// Exposed for testing purposes.
        /// </summary>
        internal void ShowToolTip()
        {
            foreach (var shape in this.HitTest(this.displayPosition))
            {
                if (shape == null || this.map == null)
                {
                    this.toolTip.IsOpen = false;
                    return;
                }

                var d2dShape = this.map.FindShapeForModel(shape);
                if (d2dShape == null)
                {
                    continue;
                }

                object defaultToolTipValue = null;
                DataTemplate template = null;
                var layer = this.map.FindLayerById(d2dShape.LayerId) as MapShapeLayer;

                if (layer != null)
                {
                    var toolTipAttributeName = layer.ShapeToolTipAttributeName;
                    if (!string.IsNullOrEmpty(toolTipAttributeName))
                    {
                        defaultToolTipValue = shape.GetAttribute(toolTipAttributeName);
                    }

                    template = GetContentTemplate(layer);
                }

                if (template != null)
                {
                    this.toolTipContent.ContentTemplate = template;
                }
                else
                {
                    this.toolTipContent.ClearValue(ContentControl.ContentTemplateProperty);

                    // NOTE: Do not show toolTip if there is no default content, and the user has not specified custom toolTip template; otherwise we will just visualize empty toolTip.
                    if (defaultToolTipValue == null)
                    {
                        this.toolTipContent.Content = null;
                        this.toolTip.IsOpen = false;

                        return;
                    }
                }

                this.toolTipContent.Content = new MapShapeToolTipContext() { Shape = shape, Layer = layer, DisplayContent = defaultToolTipValue };
                this.toolTip.IsOpen = true;

                this.UpdateToolTipPosition(this.displayPosition);
            }
        }

        /// <summary>
        /// Called when the respective <see cref="MapShapeLayer" /> is invalidated and its contents are cleared.
        /// </summary>
        /// <param name="layer">The shape layer.</param>
        protected internal override void OnShapeLayerCleared(MapShapeLayer layer)
        {
            base.OnShapeLayerCleared(layer);

            this.HideToolTip();
        }

        /// <summary>
        /// Initiates a hit test on the specified <see cref="Point" /> location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns></returns>
        /// <remarks>
        /// The default <see cref="MapBehavior" /> logic returns only the top-most <see cref="D2DShape" /> from the <see cref="MapShapeLayer" /> that matches the specific behavior requirements;
        /// you can override the default logic and return multiple <see cref="D2DShape" /> instances (e.g. from layers that overlay one another) and the specific <see cref="MapBehavior" /> will
        /// manipulate all of them.
        /// </remarks>
        protected internal override IEnumerable<IMapShape> HitTest(Point location)
        {
            if (this.map == null)
            {
                yield break;
            }

            IMapShape shape = null;

            int layerCount = this.map.Layers.Count;
            for (int i = layerCount - 1; i >= 0; i--)
            {
                var layer = this.map.Layers[i] as MapShapeLayer;
                if (layer == null || !layer.IsToolTipEnabled)
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
        /// A callback from the owning <see cref="RadMap" /> instance that notifies for a view change event.
        /// </summary>
        /// <param name="context">The context.</param>
        protected internal override void OnMapViewChanged(ViewChangedContext context)
        {
            base.OnMapViewChanged(context);

            this.HideToolTip();
        }

        /// <summary>
        /// Handles the <see cref="E:RadMap.Tapped" /> event of the owning <see cref="RadMap" /> instance.
        /// </summary>
        /// <param name="args"></param>
        protected internal override void OnTapped(TappedRoutedEventArgs args)
        {
            base.OnTapped(args);

            if (args == null)
            {
                return;
            }

            var position = args.GetPosition(this.map);

            this.HandleTap(position);
        }

        /// <summary>
        /// Called when the map owner is removed from the element tree.
        /// </summary>
        protected override void OnUnloaded()
        {
            base.OnUnloaded();

            if (this.delayTimer.IsEnabled)
            {
                this.delayTimer.Stop();
            }

            this.toolTip.IsOpen = false;

            if (this.map.adornerLayer != null)
            {
                this.map.adornerLayer.Children.Remove(this.toolTip);
            }
        }

        /// <summary>
        /// Initializes the toolTip visual and adds it to the <see cref="RadMap"/> visual tree.
        /// </summary>
        protected void InitializeToolTip()
        {
            if (!this.map.adornerLayer.Children.Contains(this.toolTip))
            {
                this.map.adornerLayer.Children.Add(this.toolTip);
            }
        }

        /// <summary>
        /// Updates the tooltip display position (and opens it if it was previously hidden).
        /// </summary>
        /// <param name="position">The position.</param>
        protected void UpdateToolTip(Point position)
        {
            this.displayPosition = position;

            this.toolTip.IsOpen = false;
            
            this.UpdateToolTipCore();
        }

        /// <summary>
        /// Hides the toolTip visual.
        /// </summary>
        protected void HideToolTip()
        {
            this.delayTimer.Stop();
            this.toolTip.IsOpen = false;
            this.displayPosition = new Point(-1, -1);
        }

        private void UpdateToolTipPosition(Point position)
        {
            var offsetX = position.X + this.TouchOverhang.X;
            var offsetY = position.Y - this.contentSize.Height - this.TouchOverhang.Y;

            bool horizontalFlip = offsetX + this.contentSize.Width > this.map.CurrentSize.Width;
            bool verticalFlip = offsetY < 0;

            if (horizontalFlip)
            {
                offsetX -= this.contentSize.Width + this.TouchOverhang.X;
            }
            if (verticalFlip)
            {
                offsetY += this.contentSize.Height + this.TouchOverhang.Y;
            }

            this.toolTipContent.UpdateVisualState(horizontalFlip, verticalFlip);

            this.toolTip.HorizontalOffset = offsetX;
            this.toolTip.VerticalOffset = offsetY;
        }

        private void OnDelayTimer_Tick(object sender, object e)
        {
            this.delayTimer.Stop();

            this.ShowToolTip();
        }

        private void OnToolTipContent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.contentSize = e.NewSize;

            if (this.toolTip.IsOpen)
            {
                this.UpdateToolTipPosition(this.displayPosition);
            }
        }
    }
}
