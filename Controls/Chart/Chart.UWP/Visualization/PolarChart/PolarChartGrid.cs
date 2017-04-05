using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Telerik.Charting;
using Telerik.Core;
using Telerik.UI.Automation.Peers;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a decoration layer over a <see cref="RadPolarChart"/>. Adds visual representation of Polar and Radial lines and stripes.
    /// </summary>
    public class PolarChartGrid : ChartGrid
    {
        /// <summary>
        /// Identifies the <see cref="PolarLineStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty PolarLineStyleProperty =
            DependencyProperty.Register(nameof(PolarLineStyle), typeof(Style), typeof(PolarChartGrid), new PropertyMetadata(null, OnPolarLineStyleChanged));

        /// <summary>
        /// Identifies the <see cref="RadialLineStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty RadialLineStyleProperty =
            DependencyProperty.Register(nameof(RadialLineStyle), typeof(Style), typeof(PolarChartGrid), new PropertyMetadata(null, OnRadialLineStyleChanged));

        internal List<Path> polarStripes;
        internal List<Path> radialStripes;
        internal PolarChartGridModel model;

        private Style polarLinesStyleCache;
        private Style radialLinesStyleCache;

        private PolarGridLineVisibility linesVisibility;
        private PolarGridLineVisibility stripesVisibility;

        private ObservableCollection<Brush> polarStripeBrushes;
        private ObservableCollection<Brush> radialStripeBrushes;

        private List<Ellipse> radialLines;
        private List<Line> polarLines;

        private Brush defaultStripeBrushCache;

        private DoubleCollection radialDashArray;
        private DoubleCollection polarDashArray;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolarChartGrid"/> class.
        /// </summary>
        public PolarChartGrid()
        {
            this.DefaultStyleKey = typeof(PolarChartGrid);

            this.model = new PolarChartGridModel();
            this.radialLines = new List<Ellipse>();
            this.polarLines = new List<Line>();

            this.polarStripes = new List<Path>();
            this.radialStripes = new List<Path>();

            this.polarStripeBrushes = new ObservableCollection<Brush>();
            this.polarStripeBrushes.CollectionChanged += this.OnPolarStripeBrushesChanged;

            this.radialStripeBrushes = new ObservableCollection<Brush>();
            this.radialStripeBrushes.CollectionChanged += this.OnRadialStripeBrushesChanged;

            this.linesVisibility = PolarGridLineVisibility.Both;
            this.stripesVisibility = PolarGridLineVisibility.None;
        }

        /// <summary>
        /// Gets the collection of brushes used to display polar stripes.
        /// </summary>
        public ObservableCollection<Brush> PolarStripeBrushes
        {
            get
            {
                return this.polarStripeBrushes;
            }
        }

        /// <summary>
        /// Gets the collection of brushes used to display radial stripes.
        /// </summary>
        public ObservableCollection<Brush> RadialStripeBrushes
        {
            get
            {
                return this.radialStripeBrushes;
            }
        }

        /// <summary>
        /// Gets or sets which lines are displayed by this instance.
        /// </summary>
        public PolarGridLineVisibility GridLineVisibility
        {
            get
            {
                return this.linesVisibility;
            }
            set
            {
                if (this.linesVisibility == value)
                {
                    return;
                }

                this.linesVisibility = value;
                this.InvalidateCore();
            }
        }

        /// <summary>
        /// Gets or sets which stripes area displayed by this instance.
        /// </summary>
        public PolarGridLineVisibility StripesVisibility
        {
            get
            {
                return this.stripesVisibility;
            }
            set
            {
                if (this.stripesVisibility == value)
                {
                    return;
                }

                this.stripesVisibility = value;
                this.InvalidateCore();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Style"/> instance that defines the appearance of the polar (radius) lines within the grid.
        /// The style should target the <see cref="Line"/> type.
        /// </summary>
        public Style PolarLineStyle
        {
            get
            {
                return this.GetValue(PolarLineStyleProperty) as Style;
            }
            set
            {
                this.SetValue(PolarLineStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Style"/> instance that defines the appearance of the radial (angle) lines within the grid.
        /// The style should target the <see cref="Ellipse"/> type.
        /// </summary>
        public Style RadialLineStyle
        {
            get
            {
                return this.GetValue(RadialLineStyleProperty) as Style;
            }
            set
            {
                this.SetValue(RadialLineStyleProperty, value);
            }
        }

        internal override Element Element
        {
            get
            {
                return this.model;
            }
        }

        internal override int DefaultZIndex
        {
            get
            {
                return RadChartBase.BackgroundZIndex;
            }
        }

        internal override void OnDefaultStripeBrushChanged(Brush oldDefaultStripeBrush, Brush newDefaultStripeBrush)
        {
            base.OnDefaultStripeBrushChanged(oldDefaultStripeBrush, newDefaultStripeBrush);

            this.defaultStripeBrushCache = newDefaultStripeBrush;

            if (!this.IsTemplateApplied)
            {
                return;
            }

            this.ArrangePolarStripes();
            this.ArrangeRadialStripes();
        }

        /// <summary>
        /// Updates of all of the chart elements presented by this instance.
        /// </summary>
        internal override void UpdateUICore(ChartLayoutContext context)
        {
            base.UpdateUICore(context);

            this.ArrangeVisuals();
        }

        /// <summary>
        /// Occurs when the presenter has been successfully attached to its owning <see cref="RadChartBase"/> instance.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            (this.chart.chartArea as ChartAreaModelWithAxes).SetGrid(this.model);
        }

        /// <summary>
        /// Occurs when the presenter has been successfully detached from its owning <see cref="RadChartBase"/> instance.
        /// </summary>
        protected override void OnDetached(RadChartBase oldChart)
        {
            base.OnDetached(oldChart);

            if (oldChart != null)
            {
                var oldCharArea = oldChart.chartArea as ChartAreaModelWithAxes;
                if (oldCharArea != null)
                {
                    oldCharArea.SetGrid(null);
                }
            }

            // TODO: Consider rasing exception.
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new PolarChartGridAutomationPeer(this);
        }

        private static void OnPolarLineStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PolarChartGrid presenter = d as PolarChartGrid;
            presenter.polarLinesStyleCache = e.NewValue as Style;
            UpdateStyles(presenter.polarLines, presenter.polarLinesStyleCache);
            presenter.InvalidateCore();
        }

        private static void OnRadialLineStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PolarChartGrid presenter = d as PolarChartGrid;
            presenter.radialLinesStyleCache = e.NewValue as Style;
            UpdateStyles(presenter.radialLines, presenter.radialLinesStyleCache);
            presenter.InvalidateCore();
        }

        private static void UpdateStyles(IEnumerable shapes, Style style)
        {
            foreach (Shape shape in shapes)
            {
                shape.Style = style;
            }
        }

        private static Geometry BuildRadialStripe(RadCircle circle, RadCircle previousCircle)
        {
            DoughnutSegmentData segment = new DoughnutSegmentData()
            {
                Center = new RadPoint(circle.Center.X + 0.5, circle.Center.Y + 0.5),
                Radius1 = circle.Radius + 0.5,
                Radius2 = previousCircle.Radius - 0.5,
                StartAngle = 0,
                SweepAngle = 359.99 // 360 does not render properly
            };

            if (segment.Radius1 < 0)
            {
                segment.Radius1 = 0;
            }
            if (segment.Radius2 < 0)
            {
                segment.Radius2 = 0;
            }

            return DoughnutSegmentRenderer.RenderArc(segment);
        }

        private static DoubleCollection FindDashArray(Style style)
        {
            foreach (Setter setter in style.Setters)
            {
                if (setter.Property == Shape.StrokeDashArrayProperty)
                {
                    return setter.Value as DoubleCollection;
                }
            }

            return null;
        }

        private static void UpdateDashArray(Shape shape, DoubleCollection dashArray)
        {
            // TODO: This HACK is needed because for some reason the DashArray, applied through Style, is not working properly for more than one shape(s)
            if (dashArray == null)
            {
                return;
            }

            DoubleCollection collection = new DoubleCollection();
            foreach (double value in dashArray)
            {
                collection.Add(value);
            }

            shape.StrokeDashArray = collection;
        }

        private void OnPolarStripeBrushesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.InvalidateCore();
        }

        private void OnRadialStripeBrushesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.InvalidateCore();
        }

        private void ArrangeVisuals()
        {
            this.ArrangeRadialStripes();
            this.ArrangePolarStripes();
            this.ArrangePolarLines();
            this.ArrangeRadialLines();
        }

        private void ArrangePolarLines()
        {
            // update polar (radius) lines
            int polarLinesIndex = 0;

            if ((this.linesVisibility & PolarGridLineVisibility.Polar) == PolarGridLineVisibility.Polar)
            {
                double antiAliasOffset = -1;

                foreach (RadPolarVector vector in this.model.polarLines)
                {
                    Line line = this.GetLineVisual(polarLinesIndex);
                    line.X1 = vector.Center.X;
                    line.Y1 = vector.Center.Y;
                    line.X2 = vector.Point.X;
                    line.Y2 = vector.Point.Y;

                    if (antiAliasOffset == -1)
                    {
                        antiAliasOffset = line.StrokeThickness % 2 == 1 ? 0.5 : 0;
                    }

                    if (RadMath.AreClose(line.Y1, line.Y2))
                    {
                        line.Y1 -= antiAliasOffset;
                        line.Y2 -= antiAliasOffset;
                    }
                    else if (RadMath.AreClose(line.X1, line.X2))
                    {
                        line.X1 += antiAliasOffset;
                        line.X2 += antiAliasOffset;
                    }

                    polarLinesIndex++;
                }
            }

            // hide not used lines
            while (polarLinesIndex < this.polarLines.Count)
            {
                this.polarLines[polarLinesIndex].Visibility = Visibility.Collapsed;
                polarLinesIndex++;
            }
        }

        private void ArrangeRadialLines()
        {
            // update radial lines (ellipses)
            int radialLinesIndex = 0;

            if ((this.linesVisibility & PolarGridLineVisibility.Radial) == PolarGridLineVisibility.Radial)
            {
                int count = this.model.radialLines.Count - 1;
                for (int i = 1; i < count; i++, radialLinesIndex++)
                {
                    RadCircle circle = this.model.radialLines[i];
                    Ellipse ellipse = this.GetEllipse(radialLinesIndex);
                    RadRect rect = circle.Bounds;

                    Canvas.SetLeft(ellipse, rect.X);
                    Canvas.SetTop(ellipse, rect.Y);
                    ellipse.Width = rect.Width;
                    ellipse.Height = rect.Height;
                }
            }

            // hide not used ellipses
            while (radialLinesIndex < this.radialLines.Count)
            {
                this.radialLines[radialLinesIndex].Visibility = Visibility.Collapsed;
                radialLinesIndex++;
            }
        }

        private void ArrangePolarStripes()
        {
            int polarStripeIndex = 0;

            if ((this.stripesVisibility & PolarGridLineVisibility.Polar) == PolarGridLineVisibility.Polar)
            {
                RadPolarVector vector;
                RadPolarVector nextVector;
                PolarChartAreaModel polarChartAreaModel = this.chart.chartArea as PolarChartAreaModel;
                bool largeArc = (polarChartAreaModel.AngleAxis as IRadialAxis).IsLargeArc;

                int stripeCount = this.model.polarLines.Count;

                for (; polarStripeIndex < stripeCount; polarStripeIndex++)
                {
                    vector = this.model.polarLines[polarStripeIndex];
                    if (polarStripeIndex < stripeCount - 1)
                    {
                        nextVector = this.model.polarLines[polarStripeIndex + 1];
                    }
                    else
                    {
                        nextVector = this.model.polarLines[0];
                    }

                    Path path = this.GetPolarStripe(polarStripeIndex);
                    path.Data = this.BuildPolarStripe(vector, nextVector, largeArc);
                }
            }

            // hide not used paths
            while (polarStripeIndex < this.polarStripes.Count)
            {
                this.polarStripes[polarStripeIndex].Visibility = Visibility.Collapsed;
                polarStripeIndex++;
            }
        }

        private void ArrangeRadialStripes()
        {
            int radialStripeIndex = 0;

            if ((this.stripesVisibility & PolarGridLineVisibility.Radial) == PolarGridLineVisibility.Radial)
            {
                RadCircle circle;
                RadCircle previousCircle;

                int stripeCount = this.model.radialLines.Count;
                for (int i = 1; i < stripeCount; i++, radialStripeIndex++)
                {
                    circle = this.model.radialLines[i];
                    previousCircle = this.model.radialLines[i - 1];

                    Path path = this.GetRadialStripe(radialStripeIndex);
                    path.Data = BuildRadialStripe(circle, previousCircle);
                }
            }

            // hide not used ellipses
            while (radialStripeIndex < this.radialStripes.Count)
            {
                this.radialStripes[radialStripeIndex].Visibility = Visibility.Collapsed;
                radialStripeIndex++;
            }
        }

        private PathGeometry BuildPolarStripe(RadPolarVector vector, RadPolarVector nextVector, bool isLargeArc)
        {
            RadPoint center = this.model.layoutSlot.Center;
            double radius = this.model.layoutSlot.Width / 2;

            double endAngle = nextVector.Angle;
            if (endAngle == 360)
            {
                endAngle = 0;
            }
            RadPoint arcEndPoint = RadMath.GetArcPoint(endAngle, center, radius);

            PathGeometry geometry = new PathGeometry();
            PathFigure figure = new PathFigure();
            figure.IsClosed = true;
            figure.IsFilled = true;

            figure.StartPoint = new Point(vector.Center.X, vector.Center.Y);

            // first line
            figure.Segments.Add(new LineSegment() { Point = new Point(vector.Point.X, vector.Point.Y) });

            // arc
            ArcSegment arc = new ArcSegment();
            arc.SweepDirection = (this.Chart as RadPolarChart).RadialAxis.SweepDirection;
            arc.Size = new Size(radius, radius);
            arc.IsLargeArc = isLargeArc;
            arc.Point = new Point(arcEndPoint.X, arcEndPoint.Y);
            figure.Segments.Add(arc);

            // second line
            figure.Segments.Add(new LineSegment() { Point = new Point(nextVector.Point.X, nextVector.Point.Y) });

            geometry.Figures.Add(figure);

            return geometry;
        }

        private Path GetPolarStripe(int index)
        {
            Path path;

            if (index < this.polarStripes.Count)
            {
                path = this.polarStripes[index];
                path.Visibility = Visibility.Visible;
            }
            else
            {
                path = new Path();
                this.polarStripes.Add(path);
                this.renderSurface.Children.Add(path);
            }

            int brushIndex = this.polarStripeBrushes.Count > 0 ? index % this.polarStripeBrushes.Count : 0;

            var fill = brushIndex < this.polarStripeBrushes.Count ? this.polarStripeBrushes[brushIndex] : null;

            // TODO: Think of a more elegant way to create alternating stripes that might be applied through Styles
            fill = this.GetDefaultStripeBrush(index, fill);

            path.Fill = fill;

            return path;
        }

        private Ellipse GetEllipse(int index)
        {
            if (index < this.radialLines.Count)
            {
                this.radialLines[index].Visibility = Visibility.Visible;
                return this.radialLines[index];
            }

            Ellipse ellipse = new Ellipse();
            ellipse.Style = this.radialLinesStyleCache;

            this.UpdateRadialDashArray(ellipse);

            this.radialLines.Add(ellipse);
            this.renderSurface.Children.Add(ellipse);

            return ellipse;
        }

        private Path GetRadialStripe(int index)
        {
            Path path;

            if (index < this.radialStripes.Count)
            {
                path = this.radialStripes[index];
                path.Visibility = Visibility.Visible;
            }
            else
            {
                path = new Path();
                this.radialStripes.Add(path);
                this.renderSurface.Children.Add(path);
            }

            int brushIndex = this.radialStripeBrushes.Count > 0 ? index % this.radialStripeBrushes.Count : 0;

            var fill = brushIndex < this.radialStripeBrushes.Count ? this.radialStripeBrushes[brushIndex] : null;

            fill = this.GetDefaultStripeBrush(index, fill);

            path.Fill = fill;

            return path;
        }

        private Line GetLineVisual(int index)
        {
            if (index < this.polarLines.Count)
            {
                this.polarLines[index].Visibility = Visibility.Visible;
                return this.polarLines[index];
            }

            Line line = new Line();
            line.Style = this.polarLinesStyleCache;

            // TODO: For some reason the StrokeDashArray gets reset once applied to a Line instance through Style
            // TODO: Check next WinRT versions
            this.UpdatePolarDashArray(line);

            this.polarLines.Add(line);
            this.renderSurface.Children.Add(line);

            return line;
        }

        private Brush GetDefaultStripeBrush(int index, Brush fill)
        {
            // TODO: Think of a more elegant way to apply a default stripe brush
            if (fill == null && (index % 2) == 0)
            {
                fill = this.defaultStripeBrushCache;
            }

            return fill;
        }

        private void UpdatePolarDashArray(Line line)
        {
            if (this.polarDashArray == null && this.PolarLineStyle != null)
            {
                this.polarDashArray = FindDashArray(this.PolarLineStyle);
            }

            UpdateDashArray(line, this.polarDashArray);
        }

        private void UpdateRadialDashArray(Ellipse ellipse)
        {
            if (this.radialDashArray == null && this.RadialLineStyle != null)
            {
                this.radialDashArray = FindDashArray(this.RadialLineStyle);
            }

            UpdateDashArray(ellipse, this.radialDashArray);
        }
    }
}