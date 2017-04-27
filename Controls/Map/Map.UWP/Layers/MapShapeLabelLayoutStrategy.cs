using Telerik.Geospatial;
using Telerik.UI.Drawing;
using Windows.Foundation;

namespace Telerik.UI.Xaml.Controls.Map
{
    /// <summary>
    /// Defines a type that may be used to extend the built-in label position logic for <see cref="MapShapeLayer"/> instances.
    /// </summary>
    public class MapShapeLabelLayoutStrategy
    {
        /// <summary>
        /// Applies the custom layout logic given the provided layout context and layer.
        /// </summary>
        public void Process(MapShapeLabelLayoutContext context, MapShapeLayer container)
        {
            this.ProcessCore(context, container);
        }

        /// <summary>
        /// Performs the core logic behind the <see cref="Process"/> routine.
        /// </summary>
        protected virtual void ProcessCore(MapShapeLabelLayoutContext context, MapShapeLayer container)
        {
        }
    }
}
