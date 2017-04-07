using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Telerik.Data.Core.Aggregates;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Holds a value presentation of an aggregate function accumulated during data grouping.
    /// </summary>
    internal abstract class AggregateValue
    {
        internal static readonly AggregateError Error = new AggregateError();

        private AggregateError error;
        private string formattedValue;

        /// <summary>
        /// Gets a presentation friendly value of the results in the current <see cref="AggregateValue"/>.
        /// </summary>
        /// <returns>Returns an object containing a formatted value or error object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Design choice."), SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Design choice.")]
        public object GetValue()
        {
            if (this.error != null)
            {
                return this.error;
            }
            else
            {
                try
                {
                    return this.GetValueOverride();
                }
                catch
                {
                    return AggregateValue.Error;
                }
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.formattedValue ?? Convert.ToString(this.GetValue(), CultureInfo.InvariantCulture);
        }

        internal void SetFormattedValue(string value)
        {
            this.formattedValue = value;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Design choice.")]
        internal void AccumulateCore(object item)
        {
            if (this.error == null)
            {
                try
                {
                    this.AccumulateOverride(item);
                }
                catch
                {
                    this.error = AggregateValue.Error;
                }
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Design choice.")]
        internal void MergeCore(AggregateValue childAggregate)
        {
            try
            {
                if (childAggregate.error != null)
                {
                    this.error = childAggregate.error;
                }
                else
                {
                    this.MergeOverride(childAggregate);
                }
            }
            catch
            {
                this.error = AggregateValue.Error;
            }
        }

        internal void RaiseError()
        {
            this.error = AggregateValue.Error;
        }

        /// <summary>
        /// Gets a presentation friendly value of the results in the current <see cref="AggregateValue"/> instance to be returned in <see cref="GetValue"/>.
        /// If an error occurred during calculations the <see cref="GetValue"/> will not call <see cref="GetValueOverride"/> but return the error instead.
        /// </summary>
        /// <returns>A result object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Design choice.")]
        protected abstract object GetValueOverride();

        /// <summary>
        /// Add the <paramref name="value"/> to the results in the current <see cref="AggregateValue"/> instance.
        /// </summary>
        /// <param name="value">The value to accumulate.</param>
        protected abstract void AccumulateOverride(object value);

        /// <summary>
        /// Merge the results of an <see cref="AggregateValue"/> with the results in the current <see cref="AggregateValue"/> instance.
        /// </summary>
        /// <param name="childAggregate">The <see cref="AggregateValue"/> to merge.</param>
        protected abstract void MergeOverride(AggregateValue childAggregate);
    }
}
