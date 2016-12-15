using System;
using Telerik.Data.Core.Layouts;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal interface IItemInfoNode : IGridNode
    {
        ItemInfo ItemInfo
        {
            get;
            set;
        }

        bool IsFrozen
        {
            get;
            set;
        }
    }
}
