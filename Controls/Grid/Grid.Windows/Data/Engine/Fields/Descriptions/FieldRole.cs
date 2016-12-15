namespace Telerik.Data.Core.Fields
{
    /// <summary>
    /// Available roles for an <see cref="IDataFieldInfo"/>.
    /// </summary>
    internal enum FieldRole
    {
        /// <summary>
        /// This <see cref="IDataFieldInfo"/> is best use as source for aggregate.
        /// </summary>
        Value,

        /// <summary>
        /// This <see cref="IDataFieldInfo"/> is best use for grouping in rows.
        /// </summary>
        Row,

        /// <summary>
        /// This <see cref="IDataFieldInfo"/> is best use for grouping in columns.
        /// </summary>
        Column,

        /// <summary>
        /// This <see cref="IDataFieldInfo"/> is best use for filtering.
        /// </summary>
        Filter,
    }
}