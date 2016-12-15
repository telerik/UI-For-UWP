using Telerik.Core;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Defines a custom type that may be used for custom value aggregation in data component. 
    /// The interface needs to be cloneable due to the parallel data processing.
    /// </summary>
    public interface IAggregateFunction : ICloneable<IAggregateFunction>
    {
        /// <summary>
        /// Gets the computed value.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        object GetValue();

        /// <summary>
        /// Applies the function logic to the provided value - that is the extracted value from the ViewModel.
        /// </summary>
        void Accumulate(object value);

        /// <summary>
        /// Merges this function with another one - this is used when Grand Totals are calculated.
        /// </summary>
        void Merge(IAggregateFunction aggregateFunction);
    }
}