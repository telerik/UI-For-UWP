using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Telerik.Data.Core
{
    internal interface IDataSourceCurrency
    {
        event EventHandler<Object> CurrentChanged;
        void ChangeCurrentItem(object item);
    }
}
