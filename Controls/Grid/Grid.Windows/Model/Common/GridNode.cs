using System;
using Telerik.Core;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal abstract class GridNode : Node, IGridNode
    {
        public object ContainerType
        {
            get;
            set;
        }

        public object Container
        {
            get;
            set;
        }

        public RadSize DesiredSize
        {
            get;
            set;
        }

        public virtual object GetDecorationContainer()
        {
            return null;
        }
    }
}
