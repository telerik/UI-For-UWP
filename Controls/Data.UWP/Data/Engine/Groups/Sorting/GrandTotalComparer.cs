using System;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Used for <see cref="IGroup"/> comparison based on their grand totals.
    /// </summary>
    internal sealed class GrandTotalComparer : GroupComparer
    {
        private int aggregateIndex;

        /// <summary>
        /// Gets or sets value that specifies the aggregate's grand total to be used in the comparison.
        /// </summary>
        public int AggregateIndex
        {
            get
            {
                return this.aggregateIndex;
            }

            set
            {
                if (this.aggregateIndex != value)
                {
                    this.aggregateIndex = value;
                    this.OnPropertyChanged(nameof(this.AggregateIndex));
                }
            }
        }

        /// <summary>
        /// Compares two <see cref="IGroup"/>s based on their grand totals.
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
            Coordinate grandTotalCoordinateX;
            Coordinate grandTotalCoordinateY;

            if (axis == DataAxis.Rows)
            {
                grandTotalCoordinateX = new Coordinate(left, results.Root.ColumnGroup);
                grandTotalCoordinateY = new Coordinate(right, results.Root.ColumnGroup);
            }
            else
            {
                grandTotalCoordinateX = new Coordinate(results.Root.RowGroup, left);
                grandTotalCoordinateY = new Coordinate(results.Root.RowGroup, right);
            }

            AggregateValue aggregateValueX = results.GetAggregateResult(this.AggregateIndex, grandTotalCoordinateX);
            AggregateValue aggregateValueY = results.GetAggregateResult(this.AggregateIndex, grandTotalCoordinateY);

            // TODO: Exception handling, Provide AggregateResults Comparer, Proper order on value-vs-null and null-vs-value cases
            if (aggregateValueX != null && aggregateValueY != null)
            {
                object valueX = aggregateValueX.GetValue();
                object valueY = aggregateValueY.GetValue();

                IComparable comparableX = valueX as IComparable;
                if (comparableX != null)
                {
                    return comparableX.CompareTo(valueY);
                }

                IComparable comparableY = valueY as IComparable;
                if (comparableY != null)
                {
                    return -comparableY.CompareTo(valueX);
                }
            }

            return 0;
        }

        /// <inheritdoc />
        protected override Cloneable CreateInstanceCore()
        {
            return new GrandTotalComparer();
        }

        /// <inheritdoc />
        protected override void CloneCore(Cloneable source)
        {
            GrandTotalComparer grandTotalComparer = source as GrandTotalComparer;
            if (grandTotalComparer != null)
            {
                this.AggregateIndex = grandTotalComparer.AggregateIndex;
            }
        }
    }
}
