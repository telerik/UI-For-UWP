using System;

namespace Telerik.Data.Core.Fields
{
    /// <summary>
    /// Represents an abstraction of a property info.
    /// </summary>
    internal interface IDataFieldInfo
    {
        /// <summary>
        /// Gets name of the property.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the data type of the property.
        /// </summary>
        /// <value>
        /// The <see cref="Type"/> of the data.
        /// </value>
        Type DataType { get; }

        /// <summary>
        /// Gets the expected role of this property.
        /// </summary>
        FieldRole Role { get; }

        /// <summary>
        /// Gets the display-friendly name of the property.
        /// </summary>
        string DisplayName { get; }

        Type RootClassType { get; }

        bool Equals(IDataFieldInfo info);
    }
}