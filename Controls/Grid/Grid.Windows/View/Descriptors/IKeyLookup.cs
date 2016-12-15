using System;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Represents an type that can be used to retrieve a certain key from an object instance. 
    /// This key may be used in various data operations like sorting, filtering and grouping within a <see cref="RadDataGrid"/> instance.
    /// </summary>
    public interface IKeyLookup
    {
        /// <summary>
        /// Retrieves the key value for the provided object instance.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        object GetKey(object instance);
    }
}
