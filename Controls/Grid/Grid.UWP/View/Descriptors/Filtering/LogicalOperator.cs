using System;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Defines the logical operator used by a <see cref="CompositeFilterDescriptor"/> to be applied among all composed <see cref="FilterDescriptorBase"/> instances.
    /// </summary>
    public enum LogicalOperator
    {
        /// <summary>
        /// The logical AND operation is applied for all the composed filters.
        /// </summary>
        And,

        /// <summary>
        /// The logical OR operation is applied for all the composed filters.
        /// </summary>
        Or
    }
}
