using System.Collections.Generic;

namespace Telerik.Data.Core
{
    internal class NestedPropertyInfo
    {
        internal readonly HashSet<object> rootItems;
        internal readonly string nestedPropertyPath;

        internal NestedPropertyInfo(HashSet<object> rootItems, string nestedPropertyPath)
        {
            this.rootItems = rootItems;
            this.nestedPropertyPath = nestedPropertyPath;
        }
    }
}