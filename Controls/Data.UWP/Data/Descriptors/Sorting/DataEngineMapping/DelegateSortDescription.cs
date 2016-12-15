using System;

namespace Telerik.Data.Core
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
