namespace Telerik.Geospatial
{
    /// <summary>
    /// Defines a 1-Dimensional <see cref="IMapShape"/>.
    /// </summary>
    public interface IMapPointShape : IMapShape
    {
        /// <summary>
        /// The geographical location of the shape.
        /// </summary>
        Location Location
        {
            get;
        }
    }
}
