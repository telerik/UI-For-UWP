namespace Telerik.UI.Xaml.Controls.Map
{
    /// <summary>
    /// Defines the available selection modes in a <see cref="MapShapeSelectionBehavior"/>.
    /// </summary>
    public enum MapShapeSelectionMode
    {
        /// <summary>
        /// No selection is allowed.
        /// </summary>
        None,

        /// <summary>
        /// Single-shape selection is allowed. A shape is selected when tapped and deselected if tapped again. If a new shape is tapped, the previous selected one is deselected.
        /// </summary>
        Single,

        /// <summary>
        /// Multiple-shape selection is allowed. A shape is selected when tapped and deselected if tapped again.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi")]
        MultiSimple,

        /// <summary>
        /// Multiple-shape selection is allowed. Multiple shapes may be selected only if the Control modifier key is pressed.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi")]
        MultiExtended
    }
}
