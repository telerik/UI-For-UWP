using System;
using Telerik.Core;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal interface IGridNode
    {
        object ContainerType
        {
            get;
            set;
        }

        object Container
        {
            get;
            set;
        }

        RadSize DesiredSize
        {
            get;
            set;
        }
    }
}
