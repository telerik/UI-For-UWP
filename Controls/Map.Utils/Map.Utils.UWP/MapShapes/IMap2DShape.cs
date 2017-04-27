using System.Collections.Generic;

namespace Telerik.Geospatial
{
    /// <summary>
    /// Represents a 2-Dimensional <see cref="IMapShape"/>. Such a shape is represented by one or more polygons.
    /// </summary>
    public interface IMap2DShape : IMapShape
    {
        /// <summary>
        /// Gets the collection of collections of <see cref="Location"/> values that represent the shape.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        IEnumerable<IEnumerable<Location>> Locations
        {
            get;
        }
    }
}
