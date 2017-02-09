namespace Telerik.Core
{
    /// <summary>
    /// Interface which allows to select a specific string format based on the provided value.
    /// </summary>
    public interface IFormatSelector
    {
        /// <summary>
        /// Gets a string format.
        /// </summary>
        /// <param name="value">The value which will be formatted.</param>
        string GetFormat(object value);
    }
}
