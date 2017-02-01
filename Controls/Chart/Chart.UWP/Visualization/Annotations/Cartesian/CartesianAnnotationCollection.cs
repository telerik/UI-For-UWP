namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a typed collection that stores <see cref="CartesianChartAnnotation"/> instances.
    /// </summary>
    public sealed class CartesianAnnotationCollection : PresenterCollection<CartesianChartAnnotation>
    {
        internal CartesianAnnotationCollection(RadCartesianChart owner)
            : base(owner)
        {
        }
    }
}
