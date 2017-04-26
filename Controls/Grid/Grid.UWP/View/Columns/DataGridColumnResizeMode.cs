namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Defines the available modes that control the User Input and Experience related to the column resize operations within the <see cref="RadDataGrid"/> component.
    /// </summary>
    public enum DataGridColumnResizeHandleDisplayMode
    {
        /// <summary>
        /// The resize exposed to the user through the column header UI is disabled.
        /// </summary>
        None,

        /// <summary>
        /// The resize exposed to the user through the column header UI is through thumb at the end of the header visible always.
        /// </summary>
        Always,
        
        /// <summary>
        /// The resize exposed to the user through the column header UI is through thumb at the end of the header visible when ColumnHeaderActionMode is Flyout and  column flyout is open. 
        /// </summary>
        OnColumnHeaderActionFlyoutOpen
    }
}
