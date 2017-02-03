using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Defines a basic chart grid control properties.
    /// </summary>
    public abstract class ChartGrid : ChartElementPresenter
    {
        /// <summary>
        /// Identifies the <see cref="DefaultStripeBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DefaultStripeBrushProperty =
            DependencyProperty.Register(nameof(DefaultStripeBrush), typeof(Brush), typeof(ChartGrid), new PropertyMetadata(null, OnDefaultStripeBrushChanged));

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> value that defines the appearance of the chart stripes of this instance.
        /// </summary>
        public Brush DefaultStripeBrush
        {
            get
            {
                return (Brush)this.GetValue(DefaultStripeBrushProperty);
            }
            set
            {
                this.SetValue(DefaultStripeBrushProperty, value);
            }
        }

        internal virtual void OnDefaultStripeBrushChanged(Brush oldDefaultStripeBrush, Brush newDefaultStripeBrush)
        {
        }

        private static void OnDefaultStripeBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as ChartGrid;
            if (sender != null)
            {
                sender.OnDefaultStripeBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
            }
        }
    }
}
