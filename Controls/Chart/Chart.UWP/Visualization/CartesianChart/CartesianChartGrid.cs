using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Telerik.Charting;
using Telerik.Core;
using Telerik.UI.Automation.Peers;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a decoration over a <see cref="RadCartesianChart"/> plot area. Adds major and minor lines, connected to each Major and Minor tick of each axis.
    /// </summary>
    public class CartesianChartGrid : ChartGrid
    {
        /// <summary>
        /// Identifies the <see cref="MajorXLineStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty MajorXLineStyleProperty =
            DependencyProperty.Register(nameof(MajorXLineStyle), typeof(Style), typeof(CartesianChartGrid), new PropertyMetadata(null, OnMajorXLineStyleChanged));

        /// <summary>
        /// Identifies the <see cref="MajorYLineStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty MajorYLineStyleProperty =
            DependencyProperty.Register(nameof(MajorYLineStyle), typeof(Style), typeof(CartesianChartGrid), new PropertyMetadata(null, OnMajorYLineStyleChanged));

        /// <summary>
        /// Identifies the <see cref="MajorLinesVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MajorLinesVisibilityProperty =
            DependencyProperty.Register(nameof(MajorLinesVisibility), typeof(GridLineVisibility), typeof(CartesianChartGrid), new PropertyMetadata(GridLineVisibility.None, OnMajorLinesVisibilityChanged));

        internal CartesianChartGridModel grid;
        internal List<Rectangle> xStripes;
        internal List<Rectangle> yStripes;

        private const int StripeZIndex = -1;

        private GridLineVisibility majorLinesVisibilityCache;
        private GridLineVisibility stripLinesVisibility;

        private GridLinesInfo majorXLines;
        private GridLinesInfo majorYLines;

        private Brush defaultStripeBrushCache;

        private ObservableCollection<Brush> xStripeBrushes;
        private ObservableCollection<Brush> yStripeBrushes;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartesianChartGrid"/> class.
        /// </summary>
        public CartesianChartGrid()
        {
            this.DefaultStyleKey = typeof(CartesianChartGrid);
            this.grid = new CartesianChartGridModel();

            this.majorXLines = new GridLinesInfo(this, GridLineVisibility.X);
            this.majorYLines = new GridLinesInfo(this, GridLineVisibility.Y);

            this.xStripes = new List<Rectangle>();
            this.yStripes = new List<Rectangle>();

            this.xStripeBrushes = new ObservableCollection<Brush>();
            this.xStripeBrushes.CollectionChanged += this.OnXStripeStylesChanged;

            this.yStripeBrushes = new ObservableCollection<Brush>();
            this.yStripeBrushes.CollectionChanged += this.OnYStripeStylesChanged;
        }

        /// <summary>
        /// Gets or sets the visibility of major grid lines.
        /// </summary>
        public GridLineVisibility MajorLinesVisibility
        {
            get
            {
                return this.majorLinesVisibilityCache;
            }
            set
            {
                this.SetValue(MajorLinesVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the render mode of major X-lines.
        /// </summary>
        public GridLineRenderMode MajorXLinesRenderMode
        {
            get
            {
                return this.majorXLines.RenderMode;
            }
            set
            {
                if (value == this.majorXLines.RenderMode)
                {
                    return;
                }

                this.majorXLines.RenderMode = value;
                this.InvalidateCore();
            }
        }

        /// <summary>
        /// Gets or sets the render mode of major X-lines.
        /// </summary>
        public GridLineRenderMode MajorYLinesRenderMode
        {
            get
            {
                return this.majorYLines.RenderMode;
            }
            set
            {
                if (value == this.majorYLines.RenderMode)
                {
                    return;
                }

                this.majorYLines.RenderMode = value;
                this.InvalidateCore();
            }
        }

        /// <summary>
        /// Gets or sets the visibility of grid's Stripes.
        /// </summary>
        public GridLineVisibility StripLinesVisibility
        {
            get
            {
                return this.stripLinesVisibility;
            }
            set
            {
                this.stripLinesVisibility = value;
                this.InvalidateCore();
            }
        }

        /// <summary>
        /// Gets the collection of brushes used to display X-axis stripes.
        /// </summary>
        public ObservableCollection<Brush> XStripeBrushes
        {
            get
            {
                return this.xStripeBrushes;
            }
        }

        /// <summary>
        /// Gets the collection of styles used to display Y-axis stripes.
        /// </summary>
        public ObservableCollection<Brush> YStripeBrushes
        {
            get
            {
                return this.yStripeBrushes;
            }
        }

        /// <summary>
        /// Gets or sets the style that defines the appearance of the major lines along the X-axis.
        /// </summary>
        public Style MajorXLineStyle
        {
            get
            {
                return (Style)this.GetValue(MajorXLineStyleProperty);
            }
            set
            {
                this.SetValue(MajorXLineStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the style that defines the appearance of the major lines along the Y-axis.
        /// </summary>
        public Style MajorYLineStyle
        {
            get
            {
                return (Style)this.GetValue(MajorYLineStyleProperty);
            }
            set
            {
                this.SetValue(MajorYLineStyleProperty, value);
            }
        }

        internal override Element Element
        {
            get
            {
                return this.grid;
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

            this.UpdateXStripes();
            this.UpdateYStripes();
        }

        /// <summary>
        /// Updates of all of the chart elements presented by this instance.
        /// </summary>
        internal override void UpdateUICore(ChartLayoutContext context)
        {
            base.UpdateUICore(context);

            this.majorXLines.Stripes = this.grid.xStripes;
            this.majorYLines.Stripes = this.grid.yStripes;

            this.UpdateVisuals();
            this.UpdateClip();
        }

        internal override void OnPlotOriginChanged(ChartLayoutContext context)
        {
            base.OnPlotOriginChanged(context);

            this.UpdateVisuals();
            this.UpdateClip();
        }

        /// <summary>
        /// Occurs when the presenter has been successfully attached to its owning <see cref="RadChartBase"/> instance.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            (this.chart.chartArea as ChartAreaModelWithAxes).SetGrid(this.grid);
        }

        /// <summary>
        /// Occurs when the presenter has been successfully detached from its owning <see cref="RadChartBase"/> instance.
        /// </summary>        
        protected override void OnDetached(RadChartBase oldChart)
        {
            base.OnDetached(oldChart);

            if (oldChart != null)
            {
                var oldChartArea = oldChart.chartArea as ChartAreaModelWithAxes;
                if (oldChartArea != null)
                {
                    oldChartArea.SetGrid(null);
                }
            }

            // TODO: Consider throwing Exception if oldChart is not valid.
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CartesianChartGridAutomationPeer(this);
        }

        private static void UpdateGridLineStyle(GridLinesInfo gridLinesInfo, Style lineStyle)
        {
            if (gridLinesInfo != null)
            {
                gridLinesInfo.LineStyle = lineStyle;
            }
        }

        private static void OnMajorXLineStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CartesianChartGrid presenter = d as CartesianChartGrid;
            UpdateGridLineStyle(presenter.majorXLines, e.NewValue as Style);
            presenter.InvalidateCore();
        }

        private static void OnMajorYLineStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CartesianChartGrid presenter = d as CartesianChartGrid;
            UpdateGridLineStyle(presenter.majorYLines, e.NewValue as Style);
            presenter.InvalidateCore();
        }

        private static void OnMajorLinesVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CartesianChartGrid presenter = d as CartesianChartGrid;
            GridLineVisibility value = (GridLineVisibility)e.NewValue;

            presenter.majorLinesVisibilityCache = value;

            presenter.majorXLines.Visible = (value & GridLineVisibility.X) == GridLineVisibility.X;
            presenter.majorYLines.Visible = (value & GridLineVisibility.Y) == GridLineVisibility.Y;

            presenter.InvalidateCore();
        }

        private void UpdateClip()
        {
            RadRect clipArea = this.chart.chartArea.plotArea.layoutSlot;
            RectangleGeometry clip = new RectangleGeometry();
            clip.Rect = new Rect(clipArea.X, clipArea.Y, clipArea.Width, clipArea.Height);

            this.Clip = clip;
        }

        private void UpdateVisuals()
        {
            if (this.drawWithComposition)
            {
                this.majorXLines.UpdateVisuals();
                this.majorYLines.UpdateVisuals();
            }
            else
            {
                this.majorXLines.Update();
                this.majorYLines.Update();
            }

            this.UpdateXStripes();
            this.UpdateYStripes();
        }

        private Rectangle GetStripeVisual(List<Rectangle> rectangles, int realizedItemIndex, IList<Brush> brushes, int brushIndex)
        {
            brushIndex = brushes.Count > 0 ? brushIndex % brushes.Count : 0;
            Brush fill = brushIndex < brushes.Count ? brushes[brushIndex] : null;
            Rectangle rectangle;

            if (realizedItemIndex >= rectangles.Count)
            {
                rectangle = new Rectangle();
                rectangles.Add(rectangle);
                this.renderSurface.Children.Add(rectangle);
                Canvas.SetZIndex(rectangle, StripeZIndex);
            }
            else
            {
                rectangle = rectangles[realizedItemIndex];
                rectangle.Visibility = Visibility.Visible;
            }

            // TODO: Think of a more elegant way to create alternating stripes that might be applied through Styles
            if (fill == null && (realizedItemIndex % 2) == 0)
            {
                fill = this.defaultStripeBrushCache;
            }

            rectangle.Fill = fill;

            return rectangle;
        }

        private void UpdateXStripes()
        {
            Rectangle rectangle;
            int realizedItemIndex = 0;

            if ((this.stripLinesVisibility & GridLineVisibility.X) == GridLineVisibility.X)
            {
                foreach (GridStripe stripe in this.grid.xStripes)
                {
                    rectangle = this.GetStripeVisual(this.xStripes, realizedItemIndex, this.xStripeBrushes, stripe.AssociatedTick.virtualIndex);
                    this.ArrangeUIElement(rectangle, stripe.FillRect);
                    realizedItemIndex++;
                }
            }

            // hide not used stripes
            while (realizedItemIndex < this.xStripes.Count)
            {
                this.xStripes[realizedItemIndex++].Visibility = Visibility.Collapsed;
            }
        }

        private void UpdateYStripes()
        {
            Rectangle rectangle;
            int realizedItemIndex = 0;

            if ((this.stripLinesVisibility & GridLineVisibility.Y) == GridLineVisibility.Y)
            {
                foreach (GridStripe stripe in this.grid.yStripes)
                {
                    rectangle = this.GetStripeVisual(this.yStripes, realizedItemIndex, this.yStripeBrushes, stripe.AssociatedTick.virtualIndex);
                    this.ArrangeUIElement(rectangle, stripe.FillRect);
                    realizedItemIndex++;
                }
            }

            // hide not used stripes
            while (realizedItemIndex < this.yStripes.Count)
            {
                this.yStripes[realizedItemIndex++].Visibility = Visibility.Collapsed;
            }
        }

        private void OnXStripeStylesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.InvalidateCore();
        }

        private void OnYStripeStylesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.InvalidateCore();
        }

        private class GridLinesInfo
        {
            public List<Line> Lines = new List<Line>();
            public Style LineStyle;
            public bool Visible;
            public GridLineRenderMode RenderMode = GridLineRenderMode.InnerAndLast;
            public List<GridStripe> Stripes;
            public Orientation Orientation;
            public CartesianChartGrid Owner;

            private List<ContainerVisual> ContainerVisualLines = new List<ContainerVisual>();
            private DoubleCollection dashArray;

            public GridLinesInfo(CartesianChartGrid owner, GridLineVisibility visibility)
            {
                this.Owner = owner;

                if (visibility == GridLineVisibility.X)
                {
                    this.Orientation = Windows.UI.Xaml.Controls.Orientation.Vertical;
                }
                else
                {
                    this.Orientation = Windows.UI.Xaml.Controls.Orientation.Horizontal;
                }
            }

            public void Update()
            {
                if (this.dashArray == null && this.LineStyle != null)
                {
                    this.dashArray = this.FindDashArray();
                }

                int arrangedLines = this.ArrangeLines();

                // hide not used lines
                while (arrangedLines < this.Lines.Count)
                {
                    this.Lines[arrangedLines].Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    arrangedLines++;
                }
            }

            public void UpdateVisuals()
            {
                if (dashArray == null)
                {
                    this.dashArray = new DoubleCollection() { 4, 2};
                }

                int arrangedVisuals = this.ArrangeLines(true);

                // hide not used lines
                while (arrangedVisuals < this.ContainerVisualLines.Count)
                {
                    this.ContainerVisualLines[arrangedVisuals].IsVisible = false;
                    arrangedVisuals++;
                }
            }

            private DoubleCollection FindDashArray()
            {
                foreach (Setter setter in this.LineStyle.Setters)
                {
                    if (setter.Property == Shape.StrokeDashArrayProperty)
                    {
                        return setter.Value as DoubleCollection;
                    }
                }

                return null;
            }

            private int ArrangeLines(bool shouldDrawWithComposition = false)
            {
                if (!this.Visible)
                {
                    return 0;
                }


                object line;
                int lineCount = 0;
                GridStripe stripe;

                int count = this.Stripes.Count;
                for (int i = 0; i < count; i++)
                {
                    stripe = this.Stripes[i];
                    if (!stripe.AssociatedTick.isVisible)
                    {
                        continue;
                    }

                    if (i == 0)
                    {
                        if ((this.RenderMode & GridLineRenderMode.First) != GridLineRenderMode.First)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if ((this.RenderMode & GridLineRenderMode.Inner) != GridLineRenderMode.Inner)
                        {
                            continue;
                        }
                    }

                    if (shouldDrawWithComposition)
                    {
                        line = this.GetContainerLineVisual(lineCount);
                        this.Owner.chart.ContainerVisualsFactory.PrepareCartesianChartGridLineVisual((ContainerVisual)line, stripe.BorderRect, this.Orientation, this.dashArray);
                    }
                    else
                    {
                        line = this.GetLineVisual(lineCount);
                        this.ArrangeLine(stripe.BorderRect, (Line)line);
                    }

                    lineCount++;
                }

                //draw the last line
                if (count > 0 && (this.RenderMode & GridLineRenderMode.Last) == GridLineRenderMode.Last)
                {
                    stripe = this.Stripes[count - 1];
                    if (stripe.AssociatedTick.isVisible)
                    {
                        AxisTickModel nextTick = stripe.AssociatedTick.NextMajorTick;
                        if (nextTick.isVisible)
                        {
                            RadRect lastRect = this.Orientation == Windows.UI.Xaml.Controls.Orientation.Vertical ?
                                    new RadRect(stripe.BorderRect.Right, stripe.BorderRect.Y, 1, stripe.BorderRect.Height) :
                                    new RadRect(stripe.BorderRect.X, stripe.BorderRect.Y - 1, stripe.BorderRect.Width, 1);

                            if (shouldDrawWithComposition)
                            {
                                line = this.GetContainerLineVisual(lineCount);
                                this.Owner.chart.ContainerVisualsFactory.PrepareCartesianChartGridLineVisual((ContainerVisual)line, lastRect, this.Orientation, this.dashArray);
                            }
                            else
                            {
                                line = this.GetLineVisual(lineCount);
                                this.ArrangeLine(lastRect, (Line)line);
                            }

                            lineCount++;
                        }
                    }
                }

                return lineCount;
            }
            
            private Line GetLineVisual(int index)
            {
                Line line;
                if (index < this.Lines.Count)
                {
                    line = this.Lines[index];
                    line.Visibility = Visibility.Visible;
                }
                else
                {
                    line = new Line();
                    this.Lines.Add(line);
                    this.Owner.renderSurface.Children.Add(line);
                }

                line.Style = this.LineStyle;

                // TODO: For some reason the StrokeDashArray gets reset once applied to a Line instance through Style
                // TODO: Check next WinRT versions
                this.UpdateDashArray(line);

                return line;
            }

            private ContainerVisual GetContainerLineVisual(int index)
            {
                ContainerVisual containerVisual;
                if (index < this.ContainerVisualLines.Count)
                {
                    containerVisual = this.ContainerVisualLines[index];
                    if (!containerVisual.IsVisible)
                    {
                        containerVisual.IsVisible = true;
                    }

                    return containerVisual;
                }

                containerVisual = this.Owner.chart.ContainerVisualsFactory.CreateContainerVisual(this.Owner.Compositor, this.GetType());
                this.ContainerVisualLines.Add(containerVisual);
                this.Owner.ContainerVisualRoot.Children.InsertAtBottom(containerVisual);

                return containerVisual;
            }
            
            private void UpdateDashArray(Line line)
            {
                if (this.dashArray == null)
                {
                    return;
                }

                line.StrokeDashArray = this.dashArray.Clone();
            }

            private void ArrangeLine(RadRect rect, Line line)
            {
                double offset = line.StrokeThickness % 2 == 0 ? 0 : 0.5;
                if (this.Orientation == Windows.UI.Xaml.Controls.Orientation.Vertical)
                {
                    line.X1 = rect.X + offset;
                    line.X2 = rect.X + offset;
                    line.Y1 = rect.Y;
                    line.Y2 = rect.Bottom;
                }
                else
                {
                    line.X1 = rect.X;
                    line.X2 = rect.Right;
                    line.Y1 = rect.Bottom + offset;
                    line.Y2 = rect.Bottom + offset;
                }
            }
        }
    }
}
