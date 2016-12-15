namespace Telerik.Data.Core
{
    /// <summary>
    /// Interface that describe GroupDescription.
    /// </summary>
    internal interface IGroupDescription : IDescriptionBase
    {
        /// <summary>
        /// Gets the <see cref="SortOrder"/> that will be used for group sorting.
        /// </summary>
        SortOrder SortOrder { get; }

        /// <summary>
        /// Gets or sets the <see cref="GroupComparer"/> that will be used for group comparisons.
        /// </summary>
        GroupComparer GroupComparer { get; set; }
    }
}