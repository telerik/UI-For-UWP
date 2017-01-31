using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Telerik.Data.Core.Aggregates
{
    /// <summary>
    /// Represents an abstract aggregate class helping in variance estimation.
    /// </summary>
    internal abstract class VarianceAggregateBase : AggregateValue
    {
        private List<double> values;

        internal VarianceAggregateBase()
        {
            this.values = new List<double>();
        }

        internal int Count
        {
            get
            {
                return this.values.Count;
            }
        }

        internal double GetSquaredDifferencesSum()
        {
            double average = Enumerable.Sum(this.values) / this.values.Count;
            double difference = 0;

            foreach (var value in this.values)
            {
                difference += Math.Pow(value - average, 2);
            }

            return difference;
        }

        /// <inheritdoc />
        protected override void AccumulateOverride(object value)
        {
            this.values.Add(Convert.ToDouble(value, CultureInfo.InvariantCulture));
        }

        /// <inheritdoc />
        protected override void MergeOverride(AggregateValue childAggregate)
        {
            VarianceAggregateBase standardDeviationChildAggregate = (VarianceAggregateBase)childAggregate;
            foreach (double value in standardDeviationChildAggregate.values)
            {
                this.values.Add(value);
            }
        }
    }
}