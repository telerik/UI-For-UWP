namespace Telerik.UI.Xaml.Controls.Data.ListView
{
    /// <summary>
    /// Defines the selection behavior for a data control.
    /// </summary>  
    public enum DataControlsSelectionMode
    {
        /// <summary>
        /// No selection is allowed.
        /// </summary>
        None,

        /// <summary>
        /// The user can select only one item at a time.
        /// </summary> 
        Single,

        /// <summary>
        ///  The user can select multiple items.
        /// </summary>
        Multiple,

        /// <summary>
        ///  The user can select multiple items through Checkboxes.
        /// </summary>
        MultipleWithCheckBoxes,
    }
}
