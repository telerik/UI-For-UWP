namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// This is the base class for all chart annotations in a <see cref="RadPolarChart"/>.
    /// </summary>
    public abstract class PolarChartAnnotation : ChartAnnotation
    {
        /// <summary>
        /// Occurs when one of the axes of the owning <see cref="RadPolarChart"/> has been changed.
        /// </summary>
        /// <param name="oldAxis">The old axis.</param>
        /// <param name="newAxis">The new axis.</param>
        internal virtual void ChartAxisChanged(Axis oldAxis, Axis newAxis)
        {
        }

        /// <summary>
        /// Occurs when the presenter has been successfully attached to its owning <see cref="RadChartBase" /> instance.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            RadPolarChart polarChart = this.chart as RadPolarChart;
            if (polarChart.RadialAxis != null)
            {
                this.ChartAxisChanged(null, polarChart.RadialAxis);
            }

            if (polarChart.PolarAxis != null)
            {
                this.ChartAxisChanged(null, polarChart.PolarAxis);
            }
        }
    }
}
