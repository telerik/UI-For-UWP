using Telerik.Geospatial;
using Telerik.UI.Drawing;
using Windows.Foundation;

namespace Telerik.UI.Xaml.Controls.Map
{
    /// <summary>
    /// Encapsulates the information needed by a <see cref="MapShapeLabelLayoutStrategy"/> instance to customize the labels for each shape within a <see cref="MapShapeLayer"/> instance.
    /// </summary>
    public class MapShapeLabelLayoutContext
    {
        /// <summary>
        /// Gets the associated <see cref="IMapShape"/> instance.
        /// </summary>
        public IMapShape Shape
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the label of the associated <see cref="IMapShape"/> instance.
        /// </summary>
        public string Label
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the current zoom level of the map.
        /// </summary>
        public double ZoomLevel
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the current Center of the map.
        /// </summary>
        public Location Center
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the desired visibility of the shape's label.
        /// </summary>
        public ShapeLabelVisibility Visibility
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="Location"/> at which the label will be rendered. The actual position in render units is further modified by the <see cref="RenderLocationOrigin"/> property.
        /// </summary>
        public Location? RenderLocation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="Windows.Foundation.Point(double, double)"/> value that defines how the label is position in render units over its <see cref="RenderLocation"/>.
        /// </summary>
        public Point? RenderLocationOrigin
        {
            get;
            set;
        }
    }
}
