namespace Telerik.UI.Xaml.Controls.Map
{
    /// <summary>
    /// Defines a type that may be used to dynamically style <see cref="Telerik.Geospatial.IMapShape"/> instances visualized within a <see cref="MapShapeLayer"/>.
    /// </summary>
    public class MapShapeStyleSelector
    {
        /// <summary>
        /// Selects the desired style, provided the <see cref="MapShapeLayer"/> instance, containing the shape.
        /// </summary>
        /// <param name="context">The context, associated with each shape.</param>
        /// <param name="container">The <see cref="MapShapeLayer"/> instance.</param>
        public void SelectStyle(MapShapeStyleContext context, MapShapeLayer container)
        {
            if (context == null || container == null)
            {
                return;
            }

            this.SelectStyleCore(context, container);
        }

        /// <summary>
        /// Provides the core implementation of the <see cref="M:SelectStyle"/> method.
        /// </summary>
        protected virtual void SelectStyleCore(MapShapeStyleContext context, MapShapeLayer container)
        {
        }
    }
}
