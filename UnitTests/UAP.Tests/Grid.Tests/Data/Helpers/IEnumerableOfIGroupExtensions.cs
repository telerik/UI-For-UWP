using System.Collections.Generic;
using System.Linq;
using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    internal static class IEnumerableOfIGroupExtensions
    {
        public static IEnumerable<IGroup> Flatten(this IEnumerable<IGroup> source)
        {
            foreach (var group in source)
            {
                yield return group;
                if (group.HasItems && !group.IsBottomLevel)
                {
                    foreach (var descendant in group.Items.OfType<IGroup>().Flatten())
                    {
                        yield return descendant;
                    }
                }
            }
        }
    }
}
