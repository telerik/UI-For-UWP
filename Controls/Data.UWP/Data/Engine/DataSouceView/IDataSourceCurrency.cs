using System;

namespace Telerik.Data.Core
{
    internal interface IDataSourceCurrency
    {
        event EventHandler<object> CurrentChanged;
        void ChangeCurrentItem(object item);
    }
}
