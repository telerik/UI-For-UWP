using System.Collections.Specialized;

namespace Telerik.Core.Data
{
    internal interface ICollectionChangedListener
    {
        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e);
    }
}