namespace Telerik.Core.Data
{
    /// <summary>
    /// Defines a public method that can be used for looking-up values on an object instance.
    /// </summary>
    public abstract class ValueLookup : ViewModelBase
    {
        /// <summary>
        /// Retrieves the desired value from the specified object instance.
        /// </summary>
        /// <param name="dataItem">The object instance from which the value is retrieved.</param>
        /// <returns>The actual value.</returns>
        public abstract object GetValueForItem(object dataItem);
    }
}
