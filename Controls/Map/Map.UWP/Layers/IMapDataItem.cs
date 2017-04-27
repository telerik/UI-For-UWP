using Telerik.Geospatial;
using Windows.Foundation;

namespace Telerik.UI.Xaml.Controls.Map
{
    /// <summary>
    /// Defines a contract that allows arbitrary data items to be visualized within a <see cref="RadMap"/> instance.
    /// </summary>
    public interface IMapDataItem
    {
        /// <summary>
        /// Gets the geographic location where the item needs to be displayed.
        /// </summary>
        Location Location
        {
            get;
        }

        /// <summary>
        /// Gets the minimum of the zoom range in which the item is visible.
        /// </summary>
        double MinZoom
        {
            get;
        }

        /// <summary>
        /// Gets the maximum of the zoom range in which the item is visible.
        /// </summary>
        double MaxZoom
        {
            get;
        }

        /// <summary>
        /// Gets how the visual representation of the data item is aligned with the physical coordinates of the <see cref="Location"/> property.
        /// A value of (0.5, 0.5) will center the visual representation over the geographic location.
        /// </summary>
        Point LocationOrigin
        {
            get;
        }
    }
}
