using System.Collections.Generic;

namespace Telerik.Data.Core
{
    internal class GroupComparerDecorator : IComparer<object>
    {
        private GroupComparer groupComparer;
        private IAggregateResultProvider results;
        private DataAxis axis;

        private int sortOrderMultiplier;

        public GroupComparerDecorator(GroupComparer groupComparer, SortOrder sortOrder, IAggregateResultProvider results, DataAxis axis)
        {
            this.groupComparer = groupComparer;
            this.results = results;
            this.axis = axis;

            this.sortOrderMultiplier = sortOrder == SortOrder.Descending ? -1 : 1;
        }

        public int Compare(object x, object y)
        {
            return this.groupComparer.CompareGroups(this.results, (IGroup)x, (IGroup)y, this.axis) * this.sortOrderMultiplier;
        }
    }
}