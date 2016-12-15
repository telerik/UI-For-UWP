using Telerik.Geospatial;

namespace Telerik.UI.Xaml.Controls.Map
{
    /// <summary>
    /// Encapsulates the information associated with the <see cref="CommandId.ViewChanged"/> command within a <see cref="RadMap"/> instance.
    /// </summary>
    public class ViewChangedContext
    {
        private Location previousCenter;
        private double previousZoomLevel;
        private Location newCenter;
        private double newZoomLevel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewChangedContext"/> class.
        /// </summary>
        public ViewChangedContext()
        {
        }

        /// <summary>
        /// Gets or sets the <see cref="Location"/> value that will be set as the new <see cref="RadMap.Center"/>.
        /// </summary>
        public Location NewCenter
        {
            get
            {
                return this.newCenter;
            }
            set
            {
                this.newCenter = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Double"/> value that will be set as the new <see cref="RadMap.ZoomLevel"/>.
        /// </summary>
        public double NewZoomLevel
        {
            get
            {
                return this.newZoomLevel;
            }
            set
            {
                this.newZoomLevel = value;
            }
        }

        /// <summary>
        /// Gets the previous <see cref="RadMap.Center"/> value.
        /// </summary>
        public Location PreviousCenter
        {
            get
            {
                return this.previousCenter;
            }
            internal set
            {
                this.previousCenter = value;
            }
        }

        /// <summary>
        /// Gets the previous <see cref="RadMap.ZoomLevel"/> value.
        /// </summary>
        public double PreviousZoomLevel
        {
            get
            {
                return this.previousZoomLevel;
            }
            internal set
            {
                this.previousZoomLevel = value;
            }
        }
    }
}
