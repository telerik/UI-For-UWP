namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Defines the modes for displaying empty content.
    /// </summary>
    public enum EmptyContentDisplayMode
    {
        /// <summary>
        /// Does not display empty content.
        /// </summary>
        None = 0,

        /// <summary>
        /// Displays the empty content element only when the attached source is null. 
        /// </summary>
        DataSourceNull = 1,

        /// <summary>
        /// Displays the empty content element only if the re are no items in the attached source.
        /// </summary>
        DataSourceEmpty = DataSourceNull << 1,

        /// <summary>
        /// Displays the empty content always, i.e. when there is no source or when the source is empty.
        /// </summary>
        Always = DataSourceNull | DataSourceEmpty
    }
}
