using System;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a class that allows for context-based <see cref="ChartSeriesDescriptor"/> selection within a <see cref="ChartSeriesProvider"/> instance.
    /// </summary>
    public class ChartSeriesDescriptorSelector : DependencyObject
    {
        /// <summary>
        /// Selects the desired descriptor, depending on the context specified and the owning <see cref="ChartSeriesProvider"/> instance.
        /// </summary>
        public virtual ChartSeriesDescriptor SelectDescriptor(ChartSeriesProvider provider, object context)
        {
            return null;
        }
    }
}
