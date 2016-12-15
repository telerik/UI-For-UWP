namespace Telerik.Data.Core.Fields
{
    /// <summary>
    /// Represents a node that is associated with <see cref="IDataFieldInfo"/> instance.
    /// </summary>
    internal class FieldInfoNode : ContainerNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldInfoNode"/> class.
        /// </summary>
        /// <param name="info">The <see cref="IDataFieldInfo"/> associated with this node.</param>
        public FieldInfoNode(IDataFieldInfo info)
            : base(info.Name, info.DisplayName, ContainerNodeRole.Selectable)
        {
            this.FieldInfo = info;
        }

        /// <inheritdoc />
        public IDataFieldInfo FieldInfo
        {
            get;
            protected set;
        }
    }
}