using System;
using System.Collections;
using System.Collections.Generic;

namespace Telerik.Data.Core
{
    internal interface IValueProvider
    {
        IEnumerable GetRowGroupNames(object item);

        IEnumerable GetColumnGroupNames(object item);

        AggregateValue CreateAggregateValue(int aggregateDescriptionIndex);

        string GetAggregateStringFormat(int aggregateDescriptionIndex);

        object GetAggregateValue(int aggregateDescriptionIndex, object item);

        IReadOnlyList<IAggregateDescription> GetAggregateDescriptions();

        int GetFiltersCount();

        bool PassesFilter(object[] items);

        object[] GetFilterItems(object fact);

        IComparer<object> GetSortComparer();

        Tuple<GroupComparer, SortOrder> GetRowGroupNameComparerAndSortOrder(int level);

        Tuple<GroupComparer, SortOrder> GetColumnGroupNameComparerAndSortOrder(int level);

        IGroupFactory GetGroupFactory();
    }
}