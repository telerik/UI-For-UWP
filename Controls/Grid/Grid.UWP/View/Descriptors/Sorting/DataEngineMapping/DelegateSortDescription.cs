using System;
using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal class DelegateSortDescription : SortDescription
    {
        public DelegateSortDescription()
        {
        }

        protected override void CloneOverride(Cloneable source)
        {
        }

        protected override Cloneable CreateInstanceCore()
        {
            return new DelegateSortDescription();
        }
    }
}
