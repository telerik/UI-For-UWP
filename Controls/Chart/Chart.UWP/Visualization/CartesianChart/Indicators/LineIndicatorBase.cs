using System;
using Telerik.Charting;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a financial indicator, whose value depends on the values of DataPoints in financial series.
    /// </summary>
    public abstract class LineIndicatorBase : IndicatorBase
    {
        /// <summary>
        /// Identifies the <see cref="Stroke"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register(nameof(Stroke), typeof(Brush), typeof(LineIndicatorBase), new PropertyMetadata(null, OnStrokeChanged));

        /// <summary>
        /// Identifies the <see cref="StrokeThickness"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register(nameof(StrokeThickness), typeof(double), typeof(LineIndicatorBase), new PropertyMetadata(2d, OnStrokeThicknessChanged));

        /// <summary>
        /// Identifies the <see cref="StrokeDashArray"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeDashArrayProperty =
            DependencyProperty.Register(nameof(StrokeDashArray), typeof(DoubleCollection), typeof(LineIndicatorBase), new PropertyMetadata(null, OnStrokeDashArrayChanged));

        /// <summary>
        /// Identifies the <see cref="StrokeLineJoin"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeLineJoinProperty =
            DependencyProperty.Register(nameof(StrokeLineJoin), typeof(PenLineJoin), typeof(LineIndicatorBase), new PropertyMetadata(PenLineJoin.Miter, OnStrokeLineJoinChanged));

        internal LineRenderer renderer;

        internal ContainerVisual lineRendererVisual;

        /// <summary>
        /// Initializes a new instance of the <see cref="LineIndicatorBase" /> class.
        /// </summary>
        internal LineIndicatorBase()
        {
            this.DefaultStyleKey = typeof(LineIndicatorBase);

            this.renderer = this.CreateRenderer();
            this.renderer.model = this.model;
        }

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> instance that defines the stroke of the line.
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
        /// Gets or sets the thickness of the line used to present the indicator.
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

        internal override string Family
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        internal LineRenderer CreateRenderer()
        {
            LineRenderer lineRenderer = new LineRenderer();
            lineRenderer.strokeShape.Stroke = this.Stroke;

            return lineRenderer;
        }

        /// <summary>
        /// Updates of all of the chart elements presented by this instance.
        /// </summary>
        internal override void UpdateUICore(ChartLayoutContext context)
        {
            base.UpdateUICore(context);

            this.renderer.Render(this.drawWithComposition);

            if (this.drawWithComposition && this.renderer.renderPoints.Count > 2)
            {
                foreach (DataPointSegment dataSegment in ChartSeriesRenderer.GetDataSegments(this.renderer.renderPoints))
                {
                    this.chart.ContainerVisualsFactory.PrepareLineRenderVisual(this.lineRendererVisual, this.renderer.GetPoints(dataSegment), this.Stroke, this.StrokeThickness);
                }
            }
        }

        /// <summary>
        /// Removes the current control template. Occurs when a template has already been applied and a new one is applied.
        /// </summary>
        protected override void UnapplyTemplateCore()
        {
            base.UnapplyTemplateCore();

            if (this.renderSurface != null && !this.drawWithComposition)
            {
                this.renderSurface.Children.Remove(this.renderer.strokeShape);
            }
            else if (this.drawWithComposition)
            {
                this.ContainerVisualRoot.Children.Remove(this.lineRendererVisual);
            }
        }

        /// <summary>
        /// Adds the polyline shape to the visual tree.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            if (applied && !this.drawWithComposition)
            {
                this.renderSurface.Children.Add(this.renderer.strokeShape);
            }
            else if (this.drawWithComposition)
            {
                this.lineRendererVisual = this.chart.ContainerVisualsFactory.CreateContainerVisual(this.Compositor, this.GetType());
                this.ContainerVisualRoot.Children.InsertAtBottom(this.lineRendererVisual);
            }

            return applied;
        }

        /// <summary>
        /// Called when the StrokeThickness property is changed.
        /// </summary>
        protected virtual void OnStrokeThicknessChanged(double newValue)
        {
        }

        /// <summary>
        /// Called when the StrokeDashArray property is changed.
        /// </summary>
        protected virtual void OnStrokeDashArrayChanged(DoubleCollection newValue)
        {
        }

        /// <summary>
        /// Called when the StrokeLineJoin property is changed.
        /// </summary>
        protected virtual void OnStrokeLineJoinChanged(PenLineJoin newValue)
        {
        }

        private static void OnStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineIndicatorBase indicator = d as LineIndicatorBase;
            indicator.renderer.strokeShape.Stroke = e.NewValue as Brush;

            if (indicator.isPaletteApplied)
            {
                indicator.UpdatePalette(true);
            }
        }

        private static void OnStrokeThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineIndicatorBase indicator = d as LineIndicatorBase;
            double newValue = (double)e.NewValue;

            indicator.renderer.strokeShape.StrokeThickness = newValue;

            indicator.OnStrokeThicknessChanged(newValue);
        }

        private static void OnStrokeDashArrayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineIndicatorBase indicator = d as LineIndicatorBase;
            DoubleCollection newValue = e.NewValue as DoubleCollection;

            indicator.renderer.strokeShape.StrokeDashArray = newValue.Clone();

            indicator.OnStrokeDashArrayChanged(newValue);
        }

        private static void OnStrokeLineJoinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineIndicatorBase indicator = d as LineIndicatorBase;
            PenLineJoin newValue = (PenLineJoin)e.NewValue;

            indicator.renderer.strokeShape.StrokeLineJoin = newValue;

            indicator.OnStrokeLineJoinChanged(newValue);
        }
    }
}
