namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Lists the possible display modes for <see cref="RadDataBoundListBox"/>
    /// list header or list footer.
    /// </summary>
    public enum HeaderFooterDisplayMode
    {
        /// <summary>
        /// The list header or footer is always visible no matter whether the items source is empty or not.
        /// </summary>
        AlwaysVisible,

        /// <summary>
        /// Header or footer is shown only when there are items in the source currently 
        /// attached to the <see cref="RadDataBoundListBox"/> instance.
        /// </summary>
        WithDataItems
    }
}
