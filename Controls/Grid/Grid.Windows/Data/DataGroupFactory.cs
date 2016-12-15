using System;
using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal class DataGroupFactory : IGroupFactory
    {
        public Group CreateGroup(object key)
        {
            return new DataGroup(key);
        }
    }
}
