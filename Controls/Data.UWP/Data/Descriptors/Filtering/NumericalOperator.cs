using System;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Defines the operator used when numerical data filtering is required.
    /// </summary>
    public enum NumericalOperator
    {
        /// <summary>
        /// The left and the right operands are equal.
        /// </summary>
        EqualsTo,

        /// <summary>
        /// The left and the right operands are not equal.
        /// </summary>
        DoesNotEqualTo,

        /// <summary>
        /// The left operand is greater than the right one.
        /// </summary>
        IsGreaterThan,

        /// <summary>
        /// The left operand is greater than or equal to the right one.
        /// </summary>
        IsGreaterThanOrEqualTo,

        /// <summary>
        /// The left operand is less than the right one.
        /// </summary>
        IsLessThan,

        /// <summary>
        /// The left operand is less than or equal to the right one.
        /// </summary>
        IsLessThanOrEqualTo
    }
}
