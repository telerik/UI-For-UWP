using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Input
{
    /// <summary>
    /// Represents the list control that holds the <see cref="Segment"/>.
    /// </summary>
    [TemplatePart(Name = "DecorationLayerTemplatePartName", Type = typeof(Canvas))]
    public class SegmentedItemsControl : ItemsControl
    {
        /// <summary>
        /// Identifies the <see cref="CornerRadius" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(SegmentedItemsControl), new PropertyMetadata(new CornerRadius(0d), OnCornerRadiusChanged));

        /// <summary>
        /// Identifies the <see cref="SegmentWidthMode" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty SegmentWidthModeProperty =
            DependencyProperty.Register(nameof(SegmentWidthMode), typeof(SegmentWidthMode), typeof(SegmentedItemsControl), new PropertyMetadata(SegmentWidthMode.Equal, OnSegmentWidthModeChanged));

        /// <summary>
        /// Identifies the <see cref="SeparatorWidth" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty SeparatorWidthProperty =
            DependencyProperty.Register(nameof(SeparatorWidth), typeof(double), typeof(SegmentedItemsControl), new PropertyMetadata(0d, OnSeparatorStyleChanged));

        /// <summary>
        /// Identifies the <see cref="SeparatorBrush" /> dependency property. 
        /// </summary>
        public static readonly DependencyProperty SeparatorBrushProperty =
            DependencyProperty.Register(nameof(SeparatorBrush), typeof(Brush), typeof(SegmentedItemsControl), new PropertyMetadata(null, OnSeparatorStyleChanged));

        private const string DecorationLayerTemplatePartName = "PART_DecorationLayer";

        private Canvas decorationLayer;
        private bool isTemplateApplied;
        private Segment selectedContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SegmentedItemsControl"/> class.
        /// </summary>
        public SegmentedItemsControl()
        {
            this.DefaultStyleKey = typeof(SegmentedItemsControl);
        }

        /// <summary>
        /// Gets or sets the corner radius of the control outer border. 
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { this.SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// Gets or sets the width mode of the segments. 
        /// </summary>
        public SegmentWidthMode SegmentWidthMode
        {
            get { return (SegmentWidthMode)GetValue(SegmentWidthModeProperty); }
            set { this.SetValue(SegmentWidthModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the width of the separators between the segments. 
        /// </summary>
        public double SeparatorWidth
        {
            get { return (double)GetValue(SeparatorWidthProperty); }
            set { this.SetValue(SeparatorWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the brush that defines the background of the separators between the segments. 
        /// </summary>
        public Brush SeparatorBrush
        {
            get { return (Brush)GetValue(SeparatorBrushProperty); }
            set { this.SetValue(SeparatorBrushProperty, value); }
        }

        internal RadSegmentedControl Owner { get; set; }

        /// <summary>
        /// Gets the layer where the separators are arranged. 
        /// </summary>
        protected Canvas DecorationLayer
        {
            get
            {
                return this.decorationLayer;
            }
        }

        /// <summary>
        /// Arranges the separators in the <see cref="DecorationLayer" />. 
        /// </summary>
        /// <param name="decoratorsContext">
        /// The canvas position of each separator. 
        /// </param>
        /// <param name="finalContainerSize">
        /// The final size of the container in which separators are arranged. 
        /// </param>
        public virtual void ArrangeSeparators(List<Point> decoratorsContext, Size finalContainerSize)
        {
            for (var i = 0; i < decoratorsContext.Count; i++)
            {
                var separator = this.DecorationLayer.Children[i] as Rectangle;
                if (decoratorsContext[i].X <= finalContainerSize.Width)
                {
                    separator.Height = finalContainerSize.Height;
                    Canvas.SetLeft(separator, decoratorsContext[i].X);
                    Canvas.SetTop(separator, decoratorsContext[i].Y);
                }
                else
                {
                    separator.Height = 0;
                    Canvas.SetLeft(separator, 0);
                    Canvas.SetTop(separator, 0);
                }
            }
        }

        internal void ClearSelectedContainer()
        {
            if (this.selectedContainer != null)
            {
                this.selectedContainer.IsSelected = false;
                this.selectedContainer = null;
            }
        }

        internal void PrepareContainers()
        {
            foreach (var segment in this.ItemsPanelRoot.Children)
            {
                this.ItemContainerGenerator.PrepareItemContainer(segment);
            }
        }

        internal void UnapplyTemplate()
        {
            // TODO
            var segmentedPanel = this.ItemsPanelRoot as SegmentedPanel;
            if (segmentedPanel != null)
            {
                segmentedPanel.Owner = null;
            }

            this.selectedContainer = null;
        }

        /// <inheritdoc />
        ///
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            var segment = element as Segment;
            if (segment.IsAutoGenerated)
            {
                segment.Group = string.Empty;

                segment.ClearValue(Segment.DataContextProperty);
                segment.ClearValue(Segment.ContentProperty);
                segment.ClearValue(Segment.ContentTemplateProperty);
                segment.ClearValue(Segment.StyleProperty);
                segment.ClearValue(Segment.CommandParameterProperty);
                segment.ClearValue(Segment.CommandProperty);
            }

            segment.Selected -= this.SegmentChecked;
            segment.AnimationContextChanged -= this.SegmentAnimationContextChanged;

            if (segment.IsSelected)
            {
                segment.IsSelected = false;
            }
        }

        /// <inheritdoc />
        ///
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new Segment(true);
        }

        /// <inheritdoc />
        ///
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is Segment;
        }

        /// <inheritdoc />
        ///
        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.ItemsPanelRoot == null)
            {
                // this code is required because the ItemsPanelRoot property is set during the first measure
                base.MeasureOverride(availableSize);

                var segmentedPanel = this.ItemsPanelRoot as SegmentedPanel;
                if (segmentedPanel != null)
                {
                    segmentedPanel.Owner = this;
                    segmentedPanel.InvalidateMeasure();
                }
            }

            return base.MeasureOverride(availableSize);
        }

        /// <inheritdoc />
        ///
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.decorationLayer = this.GetTemplateChild(DecorationLayerTemplatePartName) as Canvas;
            if (this.decorationLayer == null)
            {
                throw new MissingTemplatePartException(DecorationLayerTemplatePartName, typeof(Canvas));
            }

            this.isTemplateApplied = true;

            this.SynchronizeSeparators();
        }

        /// <inheritdoc />
        ///
        protected override void OnItemsChanged(object e)
        {
            base.OnItemsChanged(e);
            var source = this.ItemsSource as IEnumerable;
            var found = false;
            if (source != null && this.Owner.SelectedItem != null)
            {
                foreach (var item in source)
                {
                    if (this.Owner.SelectedItem.Equals(item))
                    {
                        found = true;
                        break;
                    }
                }
            }

            if (!found)
            {
                this.Owner.SelectedItem = null;
            }

            this.SynchronizeSeparators();
            this.InvalidateMeasure();
        }

        /// <inheritdoc />
        ///
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            var segment = element as Segment;

            if (segment.IsAutoGenerated)
            {
                segment.DataContext = item;
                segment.Content = this.GetSegmentContent(item);

                var segmentTemplate = this.ItemTemplate ?? (this.ItemTemplateSelector != null ? this.ItemTemplateSelector.SelectTemplate(item, segment) : null);
                if (segmentTemplate != null)
                {
                    segment.ContentTemplate = segmentTemplate;
                }

                var segmentStyle = this.ItemContainerStyle ?? (this.ItemContainerStyleSelector != null ? this.ItemContainerStyleSelector.SelectStyle(item, segment) : null);
                if (segmentStyle != null)
                {
                    segment.Style = segmentStyle;
                }
                segment.CommandParameter = item;
                segment.SetBinding(Segment.CommandProperty, new Binding() { Source = this.Owner, Path = new PropertyPath("ItemCommand") });
            }

            if (item != null && item.Equals(this.Owner.SelectedItem))
            {
                segment.IsSelected = true;
            }

            segment.Group = this.Owner.GetHashCode().ToString();
            segment.AnimationContextChanged += this.SegmentAnimationContextChanged;
            segment.Selected += this.SegmentChecked;
        }

        /// <summary>
        /// Synchronizes the separators count in the <see cref="DecorationLayer" /> with the items count. 
        /// </summary>
        protected virtual void SynchronizeSeparators()
        {
            if (this.isTemplateApplied)
            {
                while (this.DecorationLayer.Children.Count > 0 && this.DecorationLayer.Children.Count > this.Items.Count - 1)
                {
                    this.DecorationLayer.Children.RemoveAt(0);
                }

                while (this.DecorationLayer.Children.Count < this.Items.Count - 1)
                {
                    var separator = new Rectangle();
                    separator.SetBinding(FrameworkElement.WidthProperty, new Binding { Source = this, Path = new PropertyPath("SeparatorWidth") });
                    separator.SetBinding(Shape.FillProperty, new Binding { Source = this, Path = new PropertyPath("SeparatorBrush") });
                    this.DecorationLayer.Children.Add(separator);
                }
            }
        }

        private static void OnCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SegmentedItemsControl;
            if (control.isTemplateApplied)
            {
                var root = control.ItemsPanelRoot as SegmentedPanel;
                if (root != null)
                {
                    root.UpdateSegmentsCornerRadius();
                }
            }
        }

        private static void OnSegmentWidthModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SegmentedItemsControl;
            if (control.ItemsPanelRoot != null)
            {
                control.ItemsPanelRoot.InvalidateMeasure();
            }
        }

        private static void OnSeparatorStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SegmentedItemsControl;
            if (control.ItemsPanelRoot != null)
            {
                control.ItemsPanelRoot.InvalidateMeasure();
            }
        }

        private object GetSegmentContent(object item)
        {
            if (!(item is FrameworkElement) && !string.IsNullOrEmpty(this.Owner.DisplayMemberPath))
            {
                var property = item.GetType().GetRuntimeProperty(this.Owner.DisplayMemberPath);
                return property != null ? property.GetValue(item) : null;
            }

            return item;
        }

        private void SegmentAnimationContextChanged(object sender, EventArgs e)
        {
            this.Owner.OnSegmentAnimationContextChanged(sender as Segment);
        }

        private void SegmentChecked(object sender, EventArgs e)
        {
            var segment = sender as Segment;

            if (segment.IsSelected)
            {
                this.selectedContainer = segment;
            }

            this.Owner.OnSegmentChecked(sender as Segment);
        }
    }
}
