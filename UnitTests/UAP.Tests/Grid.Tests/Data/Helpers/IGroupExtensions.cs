using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    internal static class IGroupExtensions
    {
        public static void Print(this IGroup group, StringBuilder stringBuilder)
        {
            group.Print(stringBuilder, String.Empty);
        }

        public static void Print(this IGroup group, StringBuilder stringBuilder, string Ident)
        {
            stringBuilder.AppendFormat("{0}● {1}", Ident, Convert.ToString(group.Name));
            stringBuilder.AppendLine();
            if (group.HasItems && !group.IsBottomLevel)
            {
                group.Items.OfType<Group>().ToList().Print(stringBuilder, Ident + "│ ");
            }
        }

        public static IEnumerable<IGroup> Flatten(this IGroup group)
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
