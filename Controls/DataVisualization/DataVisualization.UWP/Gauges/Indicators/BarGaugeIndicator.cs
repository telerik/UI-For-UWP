using Telerik.UI.Automation.Peers;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.DataVisualization
{
    /// <summary>
    /// A base class for bar indicators.
    /// A bar has a thickness and a brush that defines its color.
    /// </summary>
    public abstract class BarGaugeIndicator : GaugeIndicator
    {
        /// <summary>
        /// Identifies the BarThickness dependency property.
        /// </summary>
        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register(nameof(Thickness), typeof(double), typeof(BarGaugeIndicator), new PropertyMetadata(1d, OnThicknessPropertyChanged));

        /// <summary>
        /// Identifies the BarBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty BrushProperty =
            DependencyProperty.Register(nameof(Brush), typeof(Brush), typeof(BarGaugeIndicator), new PropertyMetadata(new SolidColorBrush(Colors.Gray), OnBrushPropertyChanged));

        /// <summary>
        /// Gets or sets the thickness of this bar indicator.
        /// </summary>
        public double Thickness
        {
            get
            {
                return (double)this.GetValue(BarGaugeIndicator.ThicknessProperty);
            }

            set
            {
                this.SetValue(BarGaugeIndicator.ThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the color of this bar indicator.
        /// </summary>
        public Brush Brush
        {
            get
            {
                return (Brush)this.GetValue(BarGaugeIndicator.BrushProperty);
            }

            set
            {
                this.SetValue(BarGaugeIndicator.BrushProperty, value);
            }
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new BarGaugeIndicatorAutomationPeer(this);
        }

        /// <summary>
        /// A virtual method that is called when the thickness of this indicator changes.
        /// </summary>
        /// <param name="newThickness">The new thickness.</param>
        internal virtual void OnThicknessChanged(double newThickness)
        {
        }

        /// <summary>
        /// A virtual method that is called when the color of this indicator changes.
        /// </summary>
        /// <param name="newColor">The new color.</param>
        internal virtual void OnBrushChanged(Brush newColor)
        {
        }

        private static void OnThicknessPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            BarGaugeIndicator indicator = sender as BarGaugeIndicator;
            if (!indicator.IsTemplateApplied)
            {
                return;
            }

            indicator.OnThicknessChanged((double)args.NewValue);
        }

        private static void OnBrushPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            BarGaugeIndicator indicator = sender as BarGaugeIndicator;
            if (!indicator.IsTemplateApplied)
            {
                return;
            }

            indicator.OnBrushChanged((Brush)args.NewValue);
        }
    }
}
