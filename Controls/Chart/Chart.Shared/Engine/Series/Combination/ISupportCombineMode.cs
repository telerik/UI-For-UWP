using System;

namespace Telerik.Charting
{
    /// <summary>
    /// Identifies chart series that can be combined with other <see cref="ISupportCombineMode"/> instances of same type.
    /// </summary>
    public interface ISupportCombineMode
    {
        /// <summary>
        /// Gets or sets the <see cref="ChartSeriesCombineMode"/> value for this instance.
        /// </summary>
        ChartSeriesCombineMode CombineMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the key of the stack where this instance is plotted.
        /// </summary>
        object StackGroupKey
        {
            get;
            set;
        }
    }
}
