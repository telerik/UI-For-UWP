using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Telerik.Data.Core.Aggregates;

namespace Telerik.Data.Core.Totals
{
    /// <summary>
    /// A <see cref="SiblingTotalsFormat"/> that computes running totals.
    /// <example>For example if you group by <see cref="System.DateTime"/> and use <see cref="RunningTotalsIn"/> to on that groups the results will show how the values sum up in time.</example>
    /// </summary>
    internal sealed class RunningTotalsIn : SiblingTotalsFormat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RunningTotalsIn"/> class.
        /// </summary>
        public RunningTotalsIn()
        {
        }

        /// <inheritdoc />
        protected internal override void FormatTotals(IReadOnlyList<TotalValue> valueFormatters, IAggregateResultProvider results)
        {
            double accumulation = 0;

            foreach (var valueFormatter in valueFormatters)
            {
                AggregateValue value = valueFormatter.Value;
                if (value != null)
                {
                    accumulation += Convert.ToDouble(value.GetValue(), CultureInfo.InvariantCulture);
                }

                valueFormatter.FormattedValue = new ConstantValueAggregate(accumulation);
            }
        }

        /// <inheritdoc />
        protected override Cloneable CreateInstanceCore()
        {
            return new RunningTotalsIn();
        }
    }
}
