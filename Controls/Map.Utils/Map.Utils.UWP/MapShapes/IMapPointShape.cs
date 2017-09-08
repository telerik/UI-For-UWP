namespace Telerik.Geospatial
{
    /// <summary>
    /// Defines a 1-Dimensional <see cref="IMapShape"/>.
    /// </summary>
    public interface IMapPointShape : IMapShape
    {
        /// <summary>
        /// Gets the geographical location of the shape.
        /// </summary>
        Location Location
        {
            get;
        }
    }
}
