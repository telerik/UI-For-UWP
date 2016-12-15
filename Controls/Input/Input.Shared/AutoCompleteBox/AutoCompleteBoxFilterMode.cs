namespace Telerik.UI.Xaml.Controls.Input
{
    /// <summary>
    /// Defines the possible text filter modes provided by <see cref="RadAutoCompleteBox"/>.
    /// </summary>
    public enum AutoCompleteBoxFilterMode
    {
        /// <summary>
        /// In this mode <see cref="RadAutoCompleteBox"/> filters
        /// the items based on whether the starting sequence of each candidate
        /// matches the current user input.
        /// </summary>
        StartsWith,

        /// <summary>
        /// In this mode <see cref="RadAutoCompleteBox"/> filters
        /// the items based on whether the input sequence is contained in each
        /// candidate.
        /// </summary>
        Contains
    }
}
