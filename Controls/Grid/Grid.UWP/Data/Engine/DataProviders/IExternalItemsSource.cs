using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Grid;

namespace Telerik.Data.Core
{
    internal interface IExternalItemsSource
    {
        IDataProvider GetExternalProvider();
    }
}
