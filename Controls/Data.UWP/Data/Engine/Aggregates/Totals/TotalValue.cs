using System.Diagnostics.CodeAnalysis;
namespace Telerik.Data.Core.Totals
{
    /// <summary>
    /// Used to format an <see cref="AggregateValue"/> for a <see cref="Coordinate"/> in a data grouping.
    /// </summary>
    internal sealed class TotalValue
    {
        private IAggregateResultProvider results;
        private int aggregate;

        private bool unasigendValue;
        private AggregateValue value;

        internal TotalValue(IAggregateResultProvider results, Coordinate groups, int aggregate)
        {
            this.results = results;
            this.aggregate = aggregate;
            this.unasigendValue = true;
            this.Groups = groups;
        }

        /// <summary>
        /// Gets the <see cref="Coordinate"/> this <see cref="TotalValue"/> is responsible for.
        /// </summary>
        [SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = nameof(value), Justification = "Design choice.")]
        public Coordinate Groups
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the <see cref="AggregateValue"/> that should replace the <see cref="Value"/> in the final result.
        /// </summary>
        [SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = nameof(value), Justification = "Design choice.")]
        public AggregateValue FormattedValue
        {
            get;
            set;
        }

        /// <summary>
        /// Gets and caches the value for <see cref="Groups"/> from the <see cref="IAggregateResultProvider"/> this <see cref="TotalValue"/> is generated for.
        /// </summary>
        public AggregateValue Value
        {
            get
            {
                if (this.unasigendValue)
                {
                    this.value = this.results.GetAggregateResult(this.aggregate, this.Groups);
                }

                return this.value;
            }
        }
    }
}
