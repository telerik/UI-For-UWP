using Telerik.Core.Data;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Represents an <see cref="IDataSourceItem"/> implementation that
    /// points to the UI virtualization mechanism
    /// that a header should be displayed in the <see cref="RadDataBoundListBox"/>.
    /// </summary>
    internal class ListHeaderIndicatorItem : DataSourceItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListHeaderIndicatorItem" /> class.
        /// </summary>
        internal ListHeaderIndicatorItem(RadListSource owner, object value) : base(owner, value)
        {
        }
    }
}
