using Telerik.Charting;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents the financial Candlestick symbol.
    /// </summary>
    public abstract class OhlcShape : Path
    {
        /// <summary>
        /// Identifies the <see cref="UpStroke"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UpStrokeProperty =
            DependencyProperty.Register(nameof(UpStroke), typeof(Brush), typeof(OhlcShape), null);

        /// <summary>
        /// Identifies the <see cref="DownStroke"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DownStrokeProperty =
            DependencyProperty.Register(nameof(DownStroke), typeof(Brush), typeof(OhlcShape), null);

        internal readonly SolidColorBrush DefaultBrush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        internal OhlcDataPoint dataPoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="OhlcShape" /> class.
        /// </summary>
        protected OhlcShape()
        {
            this.Data = new PathGeometry();
        }

        /// <summary>
        /// Gets the data point this shape is representing.
        /// </summary>
        /// <value>The data point.</value>
        public OhlcDataPoint DataPoint
        {
            get
            {
                return this.dataPoint;
            }
        }

        /// <summary>
        /// Gets or sets the stroke of the candlestick for up (rising) items.
        /// </summary>
        /// <value>The stroke.</value>
        public Brush UpStroke
        {
            get
            {
                return (Brush)this.GetValue(UpStrokeProperty);
            }
            set
            {
                this.SetValue(UpStrokeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the stroke of the candlestick for down (falling) items.
        /// </summary>
        /// <value>The stroke.</value>
        public Brush DownStroke
        {
            get
            {
                return (Brush)this.GetValue(DownStrokeProperty);
            }
            set
            {
                this.SetValue(DownStrokeProperty, value);
            }
        }

        internal virtual void UpdateElementAppearance()
        {
        }

        internal void UpdateOhlcElementStroke()
        {
            Brush strokeBrush = this.DefaultBrush;

            if (this.dataPoint != null)
            {
                if (this.dataPoint.IsFalling)
                {
                    strokeBrush = this.DownStroke;
                }
                else
                {
                    strokeBrush = this.UpStroke;
                }
            }

            this.Stroke = strokeBrush;
        }

        internal abstract void UpdateGeometry();
    }
}
