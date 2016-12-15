using Telerik.Geospatial;
using Telerik.UI.Drawing;

namespace Telerik.UI.Xaml.Controls.Map
{
    /// <summary>
    /// Encapsulates the information, provided by a <see cref="MapShapeStyleSelector"/> and used by a <see cref="MapShapeLayer"/> to dynamically style each hosted <see cref="IMapShape"/>.
    /// </summary>
    public class MapShapeStyleContext
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
        /// Gets the <see cref="ShapeUIState"/> value of the <see cref="D2DShape"/> instance that visualizes the associated shape model.
        /// </summary>
        public ShapeUIState UIState
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the <see cref="D2DShapeStyle"/> value that defines the appearance of the underlying <see cref="D2DShape"/> in its <see cref="ShapeUIState.Normal"/> state.
        /// </summary>
        public D2DShapeStyle NormalStyle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="D2DShapeStyle"/> value that defines the appearance of the underlying <see cref="D2DShape"/> in its <see cref="ShapeUIState.PointerOver"/> state.
        /// </summary>
        public D2DShapeStyle PointerOverStyle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="D2DShapeStyle"/> value that defines the appearance of the underlying <see cref="D2DShape"/> in its <see cref="ShapeUIState.Selected"/> state.
        /// </summary>
        public D2DShapeStyle SelectedStyle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="D2DTextStyle"/> value that defines the appearance of the label, displayed by the underlying <see cref="D2DShape"/> instance.
        /// </summary>
        public D2DTextStyle LabelStyle
        {
            get;
            set;
        }
    }
}
