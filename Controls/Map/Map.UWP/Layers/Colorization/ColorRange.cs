using Telerik.UI.Drawing;
using Windows.UI;

namespace Telerik.UI.Xaml.Controls.Map
{
    /// <summary>
    /// Represents a range within a <see cref="ChoroplethColorizer"/> instance.
    /// </summary>
    public class ColorRange
    {
        internal int ShapeCount;
        private D2DShapeStyle style;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorRange"/> class.
        /// </summary>
        public ColorRange()
        {
            this.style = new D2DShapeStyle();
        }

        /// <summary>
        /// Gets or sets the minimum of the range.
        /// </summary>
        public double Min
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum of the range.
        /// </summary>
        public double Max
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="D2DBrush"/> instance that defines the fill of all shapes that fall within the range.
        /// </summary>
        public D2DBrush Fill
        {
            get
            {
                return this.style.Fill;
            }
            set
            {
                this.style.Fill = value;
            }
        }

        /// <summary>
        /// Gets the zero-based index of the range in its owning <see cref="ChoroplethColorizer"/> instance.
        /// </summary>
        public int Index
        {
            get;
            internal set;
        }

        // TODO: We may want to expose the Style publicly to allow Stroke, StrokeThickness and Foreground customization.
        internal D2DShapeStyle Style
        {
            get
            {
                return this.style;
            }
        }
    }
}
