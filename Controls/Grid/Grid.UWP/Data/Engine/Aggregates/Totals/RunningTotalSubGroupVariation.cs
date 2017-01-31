namespace Telerik.Data.Core.Totals
{
    /// <summary>
    /// Specifies which sibling values should be grouped.
    /// </summary>
    internal enum RunningTotalSubGroupVariation
    {
        /// <summary>
        /// Totals that have equal names for them and their parent groups are considered siblings.
        /// </summary>
        ParentAndSelfNames,

        /// <summary>
        /// Totals that have equal names and are generated for the same <see cref="GroupDescriptionBase"/> are considered siblings.
        /// </summary>
        GroupDescriptionAndName
    }
}
