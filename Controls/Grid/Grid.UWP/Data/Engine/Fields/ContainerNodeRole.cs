namespace Telerik.Data.Core.Fields
{
    // TODO: Document these:

    /// <summary>
    /// An <see cref="ContainerNode"/> role.
    /// </summary>
    internal enum ContainerNodeRole
    {
        /// <summary>
        /// Dimension.
        /// </summary>
        Dimension,

        /// <summary>
        /// A measure item.
        /// </summary>
        Measure,

        /// <summary>
        /// A folder in hierarchy.
        /// </summary>
        Folder,

        /// <summary>
        /// Kpi.
        /// </summary>
        Kpi,

        /// <summary>
        /// Other.
        /// </summary>
        Other,

        /// <summary>
        /// Selectable.
        /// </summary>
        Selectable,

        /// <summary>
        /// None.
        /// </summary>
        None
    }
}