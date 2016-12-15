namespace Telerik.Data.Core.Fields
{
    /// <summary>
    /// Provides member access methods.
    /// </summary>
    internal interface IMemberAccess
    {
        /// <summary>
        /// Gets a value from item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The value.</returns>
        object GetValue(object item);

        /// <summary>
        /// Sets a value to item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="fieldValue">The value.</param>
        void SetValue(object item, object fieldValue);
    }
}