using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Telerik.Geospatial
{
    internal abstract class MapShape2DModel : MapShapeModel, IMap2DShape
    {
        public Collection<LocationCollection> Locations
        {
            get;
            set;
        }

        IEnumerable<IEnumerable<Location>> IMap2DShape.Locations
        {
            get
            {
                return this.Locations;
            }
        }
    }
}
