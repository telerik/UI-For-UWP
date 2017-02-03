using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.DataVisualization
{
    /// <summary>
    /// Represents a comparative measure in a RadBulletGraph.
    /// </summary>
    public class BulletGraphComparativeMeasure : BulletGraphMeasureBase
    {
        /// <summary>
        /// Identifies the Template property.
        /// </summary>
        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.Register(nameof(Template), typeof(DataTemplate), typeof(BulletGraphComparativeMeasure), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a data template that defines the look of this comparative measure instance.
        /// </summary>
        public DataTemplate Template
        {
            get
            {
                return (DataTemplate)this.GetValue(BulletGraphComparativeMeasure.TemplateProperty);
            }

            set
            {
                this.SetValue(BulletGraphComparativeMeasure.TemplateProperty, value);
            }
        }

        /// <summary>
        /// This is a factory method that is used to create specific visualizations for different bullet graph measures.
        /// </summary>
        /// <returns>Returns an object that will be used to visualize this measure.</returns>
        internal override GaugeIndicator CreateVisual()
        {
            MarkerGaugeIndicator result = new MarkerGaugeIndicator();
            result.SetBinding(MarkerGaugeIndicator.ContentTemplateProperty, new Binding() { Path = new PropertyPath("Template"), Source = this });
            Canvas.SetZIndex(result, RadBulletGraph.ComparativeMeasuresZIndex);
            return result;
        }
    }
}
