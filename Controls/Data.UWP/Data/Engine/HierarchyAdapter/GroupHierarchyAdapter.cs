using System.Collections.Generic;
using System.Linq;

namespace Telerik.Data.Core.Layouts
{
    internal class GroupHierarchyAdapter : IHierarchyAdapter
    {
        IEnumerable<object> IHierarchyAdapter.GetItems(object item)
        {
            IGroup group = item as IGroup;
            if (group != null)
            {
                if (group.HasItems)
                {
                    return group.Items;
                }
            }

            return Enumerable.Empty<object>();
        }

        object IHierarchyAdapter.GetItemAt(object item, int index)
        {
            IGroup group = item as IGroup;
            if (group != null)
            {
                if (group.HasItems && index < group.Items.Count)
                {
                    return group.Items[index];
                }
            }

            return null;
        }
    }
}