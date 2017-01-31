using System;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Defines what custom label implementation will be provided by a <see cref="ChartSeriesLabelStrategy"/> instance.
    /// </summary>
    [Flags]
    public enum LabelStrategyOptions
    {
        /// <summary>
        /// No custom implementation.
        /// </summary>
        None = 0,

        /// <summary>
        /// The strategy will provide custom visual element for each label.
        /// </summary>
        DefaultVisual = 1,

        /// <summary>
        /// The strategy will provide custom content for each label.
        /// </summary>
        Content = DefaultVisual << 1,

        /// <summary>
        /// The strategy will provide custom measurement logic for each label.
        /// </summary>
        Measure = Content << 1,

        /// <summary>
        /// The strategy will provide custom arrange logic for each label.
        /// </summary>
        Arrange = Measure << 1,

        /// <summary>
        /// All options are defined.
        /// </summary>
        All = DefaultVisual | Content | Measure | Arrange
    }
}
