namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Defines how the risers of a step line series will be positioned.
    /// </summary>
    public enum StepSeriesRisersPosition
    {
        /// <summary>
        /// Risers' position depends on the axis' plot mode.
        /// </summary>
        Default,

        /// <summary>
        /// Risers will be positioned where the axis' ticks are positioned.
        /// </summary>
        OnTicks,

        /// <summary>
        /// Risers will be positioned between the axis' ticks.
        /// </summary>
        BetweenTicks,
    }
}
