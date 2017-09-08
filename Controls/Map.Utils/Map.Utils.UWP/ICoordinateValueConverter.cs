namespace Telerik.Geospatial
{
    /// <summary>
    /// Defines a type that may be used by a Shapefile reader to provide custom conversion of each <see cref="Location"/> value processed.
    /// </summary>
    public interface ICoordinateValueConverter
    {
        /// <summary>
        /// Converts the processed <see cref="Location"/> value.
        /// </summary>
        Location Convert(Location location);
    }
}
