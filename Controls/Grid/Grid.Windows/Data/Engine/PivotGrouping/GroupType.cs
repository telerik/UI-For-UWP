namespace Telerik.Data.Core
{
    /// <summary>
    /// Possible IGroup types.
    /// </summary>
    internal enum GroupType
    {
        /// <summary>
        /// The group has no children and usually an aggregate value is available for it.
        /// </summary>
        BottomLevel,

        /// <summary>
        /// The group has aggregated values for all other groups.
        /// </summary>
        GrandTotal,

        /// <summary>
        /// The group contains other groups. Aggregate values may or may not be available.
        /// </summary>
        Subheading,

        /// <summary>
        /// The group contains no subgroups. The aggregate values for this groups parent could be retrieved using this group.
        /// </summary>
        Subtotal
    }
}