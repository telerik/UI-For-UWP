namespace Telerik.Data.Core
{
    /// <summary>
    /// A filter abstraction.
    /// </summary>
    public interface IFilter
    {
        /// <summary>
        /// Determines if an object should be filtered.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>True if the <paramref name="item"/> should be used in the results. False if it should be ignored.</returns>
        bool PassesFilter(object item);
    }
}
