namespace Telerik.UI.Xaml.Controls.Input
{
    /// <summary>
    /// Defines the supported placement options for the drop-down that contains the available suggestion items.
    /// </summary>
    public enum AutoCompleteBoxPlacementMode
    {
        /// <summary>
        /// The position of the drop-down is dynamically calculated based on the available screen estate.
        /// </summary>
        Auto,

        /// <summary>
        /// The drop-down is always displayed above the input field.
        /// </summary>
        Top,

        /// <summary>
        /// The drop-down is always displayed below the input field.
        /// </summary>
        Bottom,

        /// <summary>
        /// The drop-down is not displayed. In this scenario the <see cref="RadAutoCompleteBox.FilteredItems"/> property 
        /// can be bound to an external items control to display the available suggestion items.
        /// </summary>
        None
    }
}
