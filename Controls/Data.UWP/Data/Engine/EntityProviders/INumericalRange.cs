namespace Telerik.Data.Core
{
    /// <summary>
    /// Represents a numerical range.
    /// </summary>
    public interface INumericalRange
    {
        /// <summary>
        /// Gets the lowest possible value of the range element. 
        /// </summary>
        double Min { get; }

        /// <summary>
        /// Gets the highest possible value of the range element. 
        /// </summary>
        double Max { get; }

        /// <summary>
        /// Gets a value to be added to or subtracted from the value.
        /// </summary>
        double Step { get; }
    }
}
