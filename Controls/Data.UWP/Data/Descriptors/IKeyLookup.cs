using System;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Represents an type that can be used to retrieve a certain key from an object instance. 
    /// This key may be used in various data operations like sorting, filtering and grouping within a data control instance.
    /// </summary>
    public interface IKeyLookup
    {
        /// <summary>
        /// Retrieves the key value for the provided object instance.
        /// </summary>
        object GetKey(object instance);
    }
}
