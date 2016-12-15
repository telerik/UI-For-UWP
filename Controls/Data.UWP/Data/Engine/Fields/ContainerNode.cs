using System.Collections.Generic;

namespace Telerik.Data.Core.Fields
{
    /// <summary>
    /// Represents a node in <see cref="FieldInfoNode"/> hierarchy.
    /// </summary>
    internal class ContainerNode
    {
        private List<ContainerNode> children;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerNode"/> class.
        /// </summary>
        public ContainerNode(string name, string caption, ContainerNodeRole role)
        {
            this.children = new List<ContainerNode>();
            this.Caption = caption;
            this.Role = role;
            this.Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerNode"/> class.
        /// </summary>
        public ContainerNode(string caption, ContainerNodeRole role) : this(caption, caption, role)
        {
        }

        /// <summary>
        /// Gets a string that can be used as an identifier of this instance.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string Caption { get; private set; }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public IList<ContainerNode> Children
        {
            get
            {
                return this.children;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has child nodes.
        /// </summary>
        /// <value>True, if there are child nodes.</value>
        public virtual bool HasChildren
        {
            get
            {
                return this.children.Count > 0;
            }
        }

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        /// <value>The role.</value>
        public ContainerNodeRole Role { get; protected set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Telerik.Data.Core.Fields.ContainerNode.#ctor(System.String,System.String,Telerik.Data.Core.Fields.ContainerNodeRole)", Justification = "Will fix in the future.")]
        internal static ContainerNode CreateRootNode()
        {
            var node = new ContainerNode("Root", "Root", ContainerNodeRole.None);

            return node;
        }
    }
}