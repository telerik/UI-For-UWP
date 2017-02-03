using System;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents the base class for descriptors that defines categorical data series.
    /// </summary>
    public abstract class CategoricalSeriesDescriptorBase : ChartSeriesDescriptor
    {
        /// <summary>
        /// Identifies the <see cref="CategoryPath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CategoryPathProperty =
            DependencyProperty.Register(nameof(CategoryPath), typeof(string), typeof(CategoricalSeriesDescriptorBase), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the name of the property that points to the Category value of the data point view model.
        /// </summary>
        public string CategoryPath
        {
            get
            {
                return this.GetValue(CategoryPathProperty) as string;
            }
            set
            {
                this.SetValue(CategoryPathProperty, value);
            }
        }
    }
}
