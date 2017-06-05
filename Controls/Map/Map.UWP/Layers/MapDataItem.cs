using Telerik.Core;
using Telerik.Geospatial;
using Windows.Foundation;

namespace Telerik.UI.Xaml.Controls.Map
{
    /// <summary>
    /// Represents a concrete implementation if the <see cref="IMapDataItem"/> interface.
    /// </summary>
    public class MapDataItem : ViewModelBase, IMapDataItem
    {
        private Location location;
        private double minZoom;
        private double maxZoom;
        private Point locationOrigin;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapDataItem"/> class.
        /// </summary>
        public MapDataItem()
        {
            this.location = Location.Empty;
            this.minZoom = 1;
            this.maxZoom = 20;
            this.locationOrigin = new Point(0.5, 0.5);
        }

        /// <summary>
        /// Gets or sets the minimum of the zoom range in which the item is visible.
        /// </summary>
        public double MinZoom
        {
            get
            {
                return this.minZoom;
            }
            set
            {
                if (value < 1)
                {
                    value = 1;
                }

                if (this.minZoom == value)
                {
                    return;
                }

                this.minZoom = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the maximum of the zoom range in which the item is visible.
        /// </summary>
        public double MaxZoom
        {
            get
            {
                return this.maxZoom;
            }
            set
            {
                if (this.maxZoom == value)
                {
                    return;
                }

                this.maxZoom = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the geographic location where the item needs to be displayed.
        /// </summary>
        public Location Location
        {
            get
            {
                return this.location;
            }
            set
            {
                if (this.location == value)
                {
                    return;
                }

                this.location = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Windows.Foundation.Point(double, double)"/> value that defines how the visual representation of the data item is aligned with the physical coordinates of the <see cref="Location"/> property.
        /// The default value of (0.5, 0.5) will center the visual representation over the geographic location.
        /// </summary>
        public Point LocationOrigin
        {
            get
            {
                return this.locationOrigin;
            }
            set
            {
                if (this.locationOrigin == value)
                {
                    return;
                }

                this.locationOrigin = value;
                this.OnPropertyChanged();
            }
        }
    }
}
