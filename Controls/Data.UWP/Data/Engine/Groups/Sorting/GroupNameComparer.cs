using System;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Used for <see cref="IGroup"/> comparison based on their <see cref="IGroup.Name"/>s.
    /// </summary>
    internal sealed class GroupNameComparer : GroupComparer
    {
        // TODO: Currently we are using the IGroup.Names as IComparables. Should we add IComparer?

        /// <summary>
        /// Compares two <see cref="IGroup"/>s based on their <see cref="IGroup.Name"/>s.
        /// </summary>
        /// <param name="results">The current aggregate results.</param>
        /// <param name="left">The first <see cref="IGroup"/> to compare.</param>
        /// <param name="right">The second <see cref="IGroup"/> to compare.</param>
        /// <param name="axis">Identifies if the groups are in <see cref="DataAxis.Rows"/> or <see cref="DataAxis.Columns"/>.</param>
        /// <returns>
        /// A signed integer that indicates the relative values of x and y, as shown in the following table.
        /// <para>Value Meaning Less than zero x is less than y.</para>
        /// <para>Zero x equals y.</para>
        /// <para>Greater than zero x is greater than y.</para>
        /// </returns>
        public override int CompareGroups(IAggregateResultProvider results, IGroup left, IGroup right, DataAxis axis)
        {
            if (left.Name == NullValue.Instance && right.Name == NullValue.Instance)
            {
                return 0;
            }
            else if (left.Name == NullValue.Instance)
            {
                return 1;
            }
            else if (right.Name == NullValue.Instance)
            {
                return -1;
            }

            IComparable leftComparable = left.Name as IComparable;
            if (leftComparable != null)
            {
                return leftComparable.CompareTo(right.Name);
            }

            IComparable rightComparable = right.Name as IComparable;
            if (rightComparable != null)
            {
                return -rightComparable.CompareTo(left.Name);
            }

            return 0;
        }

        /// <inheritdoc />
        protected override Cloneable CreateInstanceCore()
        {
            return new GroupNameComparer();
        }

        /// <inheritdoc />
        protected override void CloneCore(Cloneable source)
        {
        }
    }
}