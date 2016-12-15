using System.Collections.Specialized;

namespace Telerik.Core.Data
{
    internal class DataSourceItemFactory : IDataSourceItemFactory
    {
        public IDataSourceItem CreateItem(RadListSource owner, object value)
        {
            return new DataSourceItem(owner, value);
        }

        public IDataSourceGroup CreateGroup(RadListSource owner, DataGroup group)
        {
            return new DataSourceGroup(owner, group);
        }

        public virtual void OnOwningListSourceCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
        }
    }
}