namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a typed collection that stores <see cref="PolarChartAnnotation"/> instances.
    /// </summary>
    public sealed class PolarAnnotationCollection : PresenterCollection<PolarChartAnnotation>
    {
        internal PolarAnnotationCollection(RadPolarChart owner)
            : base(owner)
        {
        }
    }
}
