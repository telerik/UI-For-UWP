using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.DataVisualization
{
    /// <summary>
    /// The base class for RadBulletGraph's additional measures.
    /// </summary>
    public abstract class BulletGraphMeasureBase : FrameworkElement
    {
        /// <summary>
        /// Identifies the Value dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(double), typeof(BulletGraphMeasureBase), new PropertyMetadata(0.0, OnValuePropertyChanged));

        /// <summary>
        /// Initializes a new instance of the BulletGraphMeasureBase class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "These virtual calls do not rely on uninitialized base state.")]
        protected BulletGraphMeasureBase()
        {
            this.Visual = this.CreateVisual();
            RadLinearGauge.SetIndicatorOffset(this.Visual, -20);
        }
        
        /// <summary>
        /// Gets or sets the value of this measure.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "Value is the desired name.")]
        public double Value
        {
            get
            {
                return (double)this.GetValue(BulletGraphMeasureBase.ValueProperty);
            }

            set
            {
                this.SetValue(BulletGraphMeasureBase.ValueProperty, value);
            }
        }

        internal GaugeIndicator Visual
        {
            get;
            private set;
        }

        /// <summary>
        /// This is a factory method that is used to create specific visualizations for different bullet graph measures.
        /// </summary>
        /// <returns>Returns an object that will be used to visualize this measure.</returns>
        internal abstract GaugeIndicator CreateVisual();

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BulletGraphMeasureBase measure = d as BulletGraphMeasureBase;
            measure.Visual.Value = (double)e.NewValue;
        }
    }
}
