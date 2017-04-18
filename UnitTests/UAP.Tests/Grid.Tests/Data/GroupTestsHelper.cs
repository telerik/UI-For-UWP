using System;
using System.Linq;
using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    internal static class GroupTestsHelper
    {
        public static bool AreGroupsAndItemsEqual(object item1, object item2)
        {
            IGroup group1 = item1 as IGroup;
            IGroup group2 = item2 as IGroup;

            if (group1 != null && group2 != null)
            {
                if (!Object.Equals(group1.Name, group2.Name))
                {
                    return false;
                }
                if (group1.IsBottomLevel != group2.IsBottomLevel)
                {
                    return false;
                }
                if (group1.HasItems != group2.HasItems)
                {
                    return false;
                }

                if (group1.HasItems)
                {
                    if (group1.Items.Count != group2.Items.Count)
                    {
                        return false;
                    }
                    for (int i = 0; i < group1.Items.Count; i++)
                    {
                        if (!object.Equals(group1.Items[i], group2.Items[i]))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            else if (group1 == null && group2 == null)
            {
                return Object.Equals(item1, item2);
            }

            return false;
        }

        public static bool AreGroupsEqual(IGroup group1, IGroup group2)
        {
            if (!Object.Equals(group1.Name, group2.Name))
            {
                return false;
            }
            
            if (group1.IsBottomLevel && group2.IsBottomLevel)
            {
                return true;
            }
            else if (group1.IsBottomLevel != group2.IsBottomLevel)
            {
                return false;
            }

            if (group1.HasItems != group2.HasItems)
            {
                return false;
            }

            if (group1.HasItems && !group1.IsBottomLevel)
            {
                if (group1.Items.Count != group2.Items.Count)
                {
                    return false;
                }
                for (int i = 0; i < group1.Items.Count; i++)
                {
                    if (!AreGroupsEqual(group1.Items[i] as IGroup, group2.Items[i] as IGroup))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool AreAggregatesEqual(double?[][] aggregates, int aggregateIndex, IDataResults results)
        {
            var flatRowGroups = results.Root.RowGroup.Flatten().ToList();
            var flatColumnGroups = results.Root.ColumnGroup.Flatten().ToList();

            for (int i = 0; i < flatRowGroups.Count(); i++)
            {
                for (int j = 0; j < flatColumnGroups.Count(); j++)
                {
                    var expectedValue = aggregates[i][j];
                    var rowGroup = flatRowGroups[i];
                    var columnGroup = flatColumnGroups[j];
                    var aggregateValue = results.GetAggregateResult(aggregateIndex, rowGroup, columnGroup);

                    if (aggregateValue == null)
                    {
                        if (expectedValue != null)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        var actualValue = aggregateValue.GetValue();
                        
                        if (!actualValue.Equals(expectedValue))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public static bool AreAggregatesEqual(double?[][] aggregates, IDataResults results)
        {
            return AreAggregatesEqual(aggregates, 0, results);
        }
    }
}
