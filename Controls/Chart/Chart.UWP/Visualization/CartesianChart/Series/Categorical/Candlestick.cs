using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents the financial Candlestick symbol.
    /// </summary>
    public class Candlestick : OhlcShape
    {
        /// <summary>
        /// Identifies the <see cref="UpFill"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UpFillProperty =
            DependencyProperty.Register(nameof(UpFill), typeof(Brush), typeof(Candlestick), null);

        /// <summary>
        /// Identifies the <see cref="DownFill"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DownFillProperty =
            DependencyProperty.Register(nameof(DownFill), typeof(Brush), typeof(Candlestick), null);

        /// <summary>
        /// Gets or sets the fill of the candlestick for up (raising) items.
        /// </summary>
        /// <value>The fill.</value>
        public Brush UpFill
        {
            get
            {
                return (Brush)this.GetValue(UpFillProperty);
            }
            set
            {
                this.SetValue(UpFillProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the fill of the candlestick for down (falling) items.
        /// </summary>
        /// <value>The fill.</value>
        public Brush DownFill
        {
            get
            {
                return (Brush)this.GetValue(DownFillProperty);
            }
            set
            {
                this.SetValue(DownFillProperty, value);
            }
        }

        internal override void UpdateElementAppearance()
        {
            this.UpdateOhlcElementFill();
            this.UpdateOhlcElementStroke();
        }

        internal void UpdateOhlcElementFill()
        {
            Brush fillBrush = this.DefaultBrush;

            if (this.dataPoint != null)
            {
                if (this.dataPoint.IsFalling)
                {
                    fillBrush = this.DownFill;
                }
                else
                {
                    fillBrush = this.UpFill;
                }
            }

            this.Fill = fillBrush;
        }

        internal override void UpdateGeometry()
        {
            if (this.dataPoint == null)
            {
                (this.Data as PathGeometry).Figures.Clear();
                {
                    return;
                }
            }

            PathGeometry pathGeometry = new PathGeometry();

            var numericalPlot = this.dataPoint.numericalPlot;

            // NOTE: Rendering the shape within (1,1) box and then stretching it generally works,
            // however, the "stretch" causes unsolvable layout rounding issues.
            // Therefore, we are now rendering the element within the actual physical dimensions.
            var boxHeight = this.dataPoint.layoutSlot.Height;
            var boxWidth = this.dataPoint.layoutSlot.Width;

            double upperShadowMinValue = Math.Min(numericalPlot.PhysicalOpen, numericalPlot.PhysicalClose);
            double lowerShadowMaxValue = Math.Max(numericalPlot.PhysicalOpen, numericalPlot.PhysicalClose);
            double halfBoxWidth = (int)(0.5 * boxWidth);

            double singlePixelRounding = (this.StrokeThickness % 2) == 1 ? 0.5 : 0;

            PathFigure figure = new PathFigure();
            figure.IsClosed = false;
            figure.StartPoint = new Point(halfBoxWidth + singlePixelRounding, 0);

            var candleSegment = new PolyLineSegment();
            candleSegment.Points.Add(new Point(halfBoxWidth + singlePixelRounding, upperShadowMinValue - singlePixelRounding));
            candleSegment.Points.Add(new Point(boxWidth - singlePixelRounding, upperShadowMinValue - singlePixelRounding));
            candleSegment.Points.Add(new Point(boxWidth - singlePixelRounding, lowerShadowMaxValue - singlePixelRounding));
            candleSegment.Points.Add(new Point(singlePixelRounding, lowerShadowMaxValue - singlePixelRounding));
            candleSegment.Points.Add(new Point(singlePixelRounding, upperShadowMinValue - singlePixelRounding));
            candleSegment.Points.Add(new Point(halfBoxWidth + singlePixelRounding, upperShadowMinValue - singlePixelRounding));

            figure.Segments.Add(candleSegment);

            pathGeometry.Figures.Add(figure);

            figure = new PathFigure();
            figure.IsClosed = false;
            figure.StartPoint = new Point(halfBoxWidth + singlePixelRounding, lowerShadowMaxValue - singlePixelRounding);
            figure.Segments.Add(new LineSegment() { Point = new Point(halfBoxWidth + singlePixelRounding, boxHeight - singlePixelRounding) });
            pathGeometry.Figures.Add(figure);

            this.Data = pathGeometry;
        }
    }
}
