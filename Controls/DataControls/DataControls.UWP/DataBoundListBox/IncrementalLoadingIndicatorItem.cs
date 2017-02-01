using Telerik.Core.Data;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Represents a special <see cref="DataSourceItem"/> used to mark that
    /// the visual item for requesting data should be realized.
    /// </summary>
    internal class IncrementalLoadingIndicatorItem : DataSourceItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IncrementalLoadingIndicatorItem" /> class.
        /// </summary>
        internal IncrementalLoadingIndicatorItem(RadListSource owner, object value) : base(owner, value)
        {
        }
    }
}
