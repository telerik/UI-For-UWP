using System;

namespace Telerik.Geospatial
{
    /// <summary>
    /// Defines a type that may be used to provide custom shape attribute parsing routine. Used by a Shapefile reader.
    /// </summary>
    public interface IAttributeValueConverter
    {
        /// <summary>
        /// Converts the provided attribute value to the desired value.
        /// </summary>
        /// <param name="value">The raw value read from the file.</param>
        /// <param name="fieldName">The name of the field associated with the provided value.</param>
        /// <param name="fieldType">The type of the field associated with the provided value.</param>
        object Convert(object value, string fieldName, Type fieldType);
    }
}
