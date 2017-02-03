﻿using Telerik.Charting;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a chart series, which can visualize <see cref="ScatterDataPoint"/> instances by connecting them with straight line segments.
    /// </summary>
    public class ScatterLineSeries : ScatterPointSeries, IStrokedSeries
    {
        /// <summary>
        /// Identifies the <see cref="Stroke"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register(nameof(Stroke), typeof(Brush), typeof(ScatterLineSeries), new PropertyMetadata(null, OnStrokeChanged));

        /// <summary>
        /// Identifies the <see cref="StrokeThickness"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register(nameof(StrokeThickness), typeof(double), typeof(ScatterLineSeries), new PropertyMetadata(2d, OnStrokeThicknessChanged));

        /// <summary>
        /// Identifies the <see cref="StrokeDashArray"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeDashArrayProperty =
            DependencyProperty.Register(nameof(StrokeDashArray), typeof(DoubleCollection), typeof(ScatterLineSeries), new PropertyMetadata(null, OnStrokeDashArrayChanged));

        /// <summary>
        /// Identifies the <see cref="StrokeLineJoin"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeLineJoinProperty =
            DependencyProperty.Register(nameof(StrokeLineJoin), typeof(PenLineJoin), typeof(ScatterLineSeries), new PropertyMetadata(PenLineJoin.Miter, OnStrokeLineJoinChanged));

        internal LineRenderer renderer;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ScatterLineSeries"/> class.
        /// </summary>
        public ScatterLineSeries()
        {
            this.DefaultStyleKey = typeof(ScatterLineSeries);
            this.renderer = this.CreateRenderer();
            this.renderer.model = this.Model;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Stroke"/> property has been set locally.
        /// </summary>
        bool IStrokedSeries.IsStrokeSetLocally
        {
            get
            {
                return this.ReadLocalValue(StrokeProperty) is Brush;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> instance that defines the stroke of the <see cref="Windows.UI.Xaml.Shapes.Line"/> shape.
        /// </summary>
        public Brush Stroke
        {
            get
            {
                return this.GetValue(StrokeProperty) as Brush;
            }
            set
            {
                this.SetValue(StrokeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the thickness of the line used to present the series.
        /// </summary>
        public double StrokeThickness
        {
            get
            {
                return this.renderer.strokeShape.StrokeThickness;
            }
            set
            {
                this.SetValue(StrokeThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a collection of <see cref="T:System.Double" /> values that indicates the pattern of dashes and gaps that is used to outline stroked series.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public DoubleCollection StrokeDashArray
        {
            get
            {
                return (DoubleCollection)this.GetValue(StrokeDashArrayProperty);
            }
            set
            {
                this.SetValue(StrokeDashArrayProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="PenLineJoin" /> enumeration value that specifies the type of join that is used at the vertices of a stroked series.
        /// </summary>
        public PenLineJoin StrokeLineJoin
        {
            get
            {
                return (PenLineJoin)this.GetValue(StrokeLineJoinProperty);
            }
            set
            {
                this.SetValue(StrokeLineJoinProperty, value);
            }
        }

        /// <summary>
        /// Gets the human-readable family of this instance. For example Area is the family for Area, SplineArea and PolarArea series.
        /// </summary>
        internal override string Family
        {
            get
            {
                return ChartPalette.LineFamily;
            }
        }

        internal override bool SupportsDefaultVisuals
        {
            get
            {
                return false;
            }
        }

        internal override FrameworkElement SeriesVisual
        {
            get
            {
                return this.renderer.strokeShape;
            }
        }

        /// <summary>
        /// Updates of all of the chart elements presented by this instance.
        /// </summary>
        internal override void UpdateUICore(ChartLayoutContext context)
        {
            base.UpdateUICore(context);

            this.renderer.Render();
        }

        internal override void ApplyPaletteCore()
        {
            base.ApplyPaletteCore();

            this.renderer.ApplyPalette();

            this.UpdateLegendItem(null, null);
        }

        internal override void UpdateLegendItem(FrameworkElement visual, DataPoint dataPoint)
        {
            base.UpdateLegendItem(visual, dataPoint);
            this.UpdateLegendItemProperties(this.renderer.strokeShape.Fill, this.renderer.strokeShape.Stroke);
        }

        internal virtual LineRenderer CreateRenderer()
        {
            return new LineRenderer();
        }

        /// <inheritdoc/>
        protected override void UnapplyTemplateCore()
        {
            base.UnapplyTemplateCore();

            if (this.renderSurface != null)
            {
                this.renderSurface.Children.Remove(this.renderer.strokeShape);
            }
        }

        /// <summary>
        /// Adds the polyline shape to the visual tree.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            if (applied)
            {
                this.renderSurface.Children.Add(this.renderer.strokeShape);
            }

            return applied;
        }

        private static void OnStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScatterLineSeries series = d as ScatterLineSeries;
            series.renderer.strokeShape.Stroke = e.NewValue as Brush;

            if (series.isPaletteApplied)
            {
                series.UpdatePalette(true);
            }
        }

        private static void OnStrokeThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScatterLineSeries series = d as ScatterLineSeries;
            series.renderer.strokeShape.StrokeThickness = (double)e.NewValue;
        }

        private static void OnStrokeDashArrayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScatterLineSeries series = d as ScatterLineSeries;
            series.renderer.strokeShape.StrokeDashArray = (e.NewValue as DoubleCollection).Clone();
        }

        private static void OnStrokeLineJoinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScatterLineSeries series = d as ScatterLineSeries;
            series.renderer.strokeShape.StrokeLineJoin = (PenLineJoin)e.NewValue;
        }
    }
}