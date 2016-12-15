using Telerik.Geospatial;

namespace Telerik.UI.Xaml.Controls.Map
{
    /// <summary>
    /// Represents the data context used by <see cref="MapShapeToolTipBehavior"/>.
    /// </summary>
    public class MapShapeToolTipContext
    {
        /// <summary>
        /// Gets or sets the default object that visually represents the associated <see cref="Shape"/> instance.
        /// </summary>
        public object DisplayContent
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the associated map shape instance.
        /// </summary>
        public IMapShape Shape
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the <see cref="MapShapeLayer"/> that the associated <see cref="Shape"/> belongs to.
        /// </summary>
        public MapShapeLayer Layer 
        { 
            get; 
            internal set; 
        }
    }
}
