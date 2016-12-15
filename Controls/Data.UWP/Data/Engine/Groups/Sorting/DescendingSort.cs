using System.Collections.Generic;

namespace Telerik.Data.Core
{
    internal class DescendingSort<T> : IComparer<T>
    {
        private IComparer<T> sort;

        public DescendingSort(IComparer<T> sort)
        {
            this.sort = sort;
        }

        public int Compare(T x, T y)
        {
            return -this.sort.Compare(x, y);
        }
    }
}