using Windows.Foundation;

namespace Telerik.Geospatial
{
    internal class MapPointModel : MapShapeModel, IMapPointShape
    {
        public Location Location
        {
            get;
            set;
        }
    }
}
