using System;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Defines the operator used when text filtering is required and two <see cref="System.String"/> instances are compared.
    /// </summary>
    public enum TextOperator
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
        /// The left operand starts with the right one.
        /// </summary>
        StartsWith,

        /// <summary>
        /// The left operand ends with the right one.
        /// </summary>
        EndsWith,

        /// <summary>
        /// The left operand contains the right one.
        /// </summary>
        Contains,

        /// <summary>
        /// The left operand does not contain the right one.
        /// </summary>
        DoesNotContain
    }
}
