using System.Collections.ObjectModel;

namespace Telerik.Geospatial
{
    /// <summary>
    /// Represents a collection of <see cref="IMapShape"/> instances. This collection is the final result of a Shapefile processing.
    /// </summary>
    public class MapShapeModelCollection : Collection<IMapShape>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapShapeModelCollection"/> class.
        /// </summary>
        public MapShapeModelCollection()
        {
            this.BoundingRect = LocationRect.Empty;
        }

        /// <summary>
        /// Gets the <see cref="LocationRect"/> that represents the best view for all the shapes within the collection.
        /// </summary>
        public LocationRect BoundingRect
        {
            get;
            internal set;
        }

        // TODO: This should not be Shapefile specific type (i.e. the shape models can be constructed based on other sources as well)
        internal EsriShapeType Type
        {
            get;
            set;
        }
    }
}
