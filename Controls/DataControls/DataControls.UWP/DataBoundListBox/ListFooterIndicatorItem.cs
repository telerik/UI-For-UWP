using Telerik.Core.Data;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Represents an <see cref="IDataSourceItem"/> implementation that
    /// points to the UI virtualization mechanism
    /// that a footer should be displayed in the <see cref="RadDataBoundListBox"/>.
    /// </summary>
    internal class ListFooterIndicatorItem : DataSourceItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListFooterIndicatorItem" /> class.
        /// </summary>
        internal ListFooterIndicatorItem(RadListSource owner, object value) : base(owner, value)
        {
        }
    }
}
