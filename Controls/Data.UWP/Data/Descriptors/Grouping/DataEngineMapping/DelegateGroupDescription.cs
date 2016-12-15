using System;
using System.Collections.Generic;
using Telerik.Data.Core;

namespace Telerik.Data.Core
{
    internal class DelegateGroupDescription : PropertyGroupDescriptionBase
    {
        protected override void CloneOverride(Cloneable source)
        {
        }

        protected override Cloneable CreateInstanceCore()
        {
            return new DelegateGroupDescription();
        }
    }
}
