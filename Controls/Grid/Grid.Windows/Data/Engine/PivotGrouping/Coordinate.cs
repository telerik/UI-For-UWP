using System;

namespace Telerik.Data.Core
{
    /// <summary>
    /// An unique point determined by two <see cref="IGroup"/>s - the <see cref="RowGroup"/> and the <see cref="ColumnGroup"/>.
    /// </summary>
    internal struct Coordinate : IEquatable<Coordinate>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Coordinate" /> struct.
        /// </summary>
        /// <param name="rowGroup">The row group.</param>
        /// <param name="columnGroup">The column group.</param>
        public Coordinate(IGroup rowGroup, IGroup columnGroup)
            : this()
        {
            this.RowGroup = rowGroup;
            this.ColumnGroup = columnGroup;
        }

        /// <summary>
        /// Get the RowGroup.
        /// </summary>
        public IGroup RowGroup
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the ColumnGroup.
        /// </summary>
        public IGroup ColumnGroup
        {
            get;
            set;
        }

        /// <summary>
        /// Compares two instances of <see cref="Coordinate"/> for equality. 
        /// </summary>
        /// <param name="left">The first instance of <see cref="Coordinate"/> to compare.</param>
        /// <param name="right">The second instance of <see cref="Coordinate"/> to compare.</param>
        /// <returns>true if the instances of <see cref="Coordinate"/> are equal; otherwise, false.</returns>
        public static bool operator ==(Coordinate left, Coordinate right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Evaluates two instances of <see cref="Coordinate"/> to determine inequality. 
        /// </summary>
        /// <param name="left">The first instance of <see cref="Coordinate"/> to compare.</param>
        /// <param name="right">The second instance of <see cref="Coordinate"/> to compare.</param>
        /// <returns>false if <paramref name="left"/> is equal to <paramref name="right"/>; otherwise, true.</returns>
        public static bool operator !=(Coordinate left, Coordinate right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc />
        public bool Equals(Coordinate other)
        {
            return (this.RowGroup == other.RowGroup || this.RowGroup.Equals(other.RowGroup))
                && (this.ColumnGroup == other.ColumnGroup || this.ColumnGroup.Equals(other.ColumnGroup));
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is Coordinate)
            {
                Coordinate other = (Coordinate)obj;
                return this.Equals(other);
            }

            return false;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (this.RowGroup.GetHashCode() * 8821) + (this.ColumnGroup.GetHashCode() * 8741);
        }
    }
}