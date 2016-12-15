using System.Collections.Specialized;
namespace Telerik.Core.Data
{
    /// <summary>
    /// Creates instances of the <see cref="IDataSourceItem"/> type which are used by a <see cref="RadListSource"/> instance.
    /// </summary>
    internal interface IDataSourceItemFactory
    {
        /// <summary>
        /// Creates a <see cref="IDataSourceItem"/> instance.
        /// </summary>
        IDataSourceItem CreateItem(RadListSource owner, object value);

        /// <summary>
        /// Creates a group item for the specified data group.
        /// </summary>
        IDataSourceGroup CreateGroup(RadListSource owner, DataGroup dataGroup);

        /// <summary>
        /// Called when the items in the owning <see cref="RadListSource"/> instance change.
        /// </summary>
        void OnOwningListSourceCollectionChanged(NotifyCollectionChangedEventArgs args);
    }
}
