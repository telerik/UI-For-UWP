using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    internal static class IReadOnlyListOfIGroupExtensions
    {
        public static void Print(this IReadOnlyList<IGroup> source, StringBuilder stringBuilder)
        {
            source.Print(stringBuilder, String.Empty);
        }

        public static void Print(this IReadOnlyList<IGroup> source, StringBuilder stringBuilder, string Ident)
        {
            for (int i = 0; i < source.Count; i++ )
            {
                bool isLast = i == source.Count - 1;
                IGroup group = source[i];
                stringBuilder.AppendFormat("{0}{1}{2} ({3}){4}", Ident, isLast ? "└─● " : "├─● ", Convert.ToString(group.Name), Convert.ToString(group.Type), System.Environment.NewLine);
                if (group.HasItems && !group.IsBottomLevel)
                {
                    group.Items.OfType<IGroup>().ToList().Print(stringBuilder, Ident + (isLast ? "  " : "│ "));
                }
            }
        }
    }
}
