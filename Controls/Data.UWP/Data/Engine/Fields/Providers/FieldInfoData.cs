using System;
using System.Collections.Generic;

namespace Telerik.Data.Core.Fields
{
    /// <summary>
    /// Provides information about properties/fields of items that are used by a <see cref="IFieldDescriptionProvider"/>.
    /// </summary>
    internal class FieldInfoData : IFieldInfoData
    {
        private Dictionary<string, IDataFieldInfo> cachedFieldDescriptions = new Dictionary<string, IDataFieldInfo>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldInfoData" /> class.
        /// </summary>
        /// <param name="root">The root.</param>
        public FieldInfoData(ContainerNode root)
        {
            if (root == null)
            {
                throw new ArgumentNullException(nameof(root));
            }

            this.RootFieldInfo = root;
            this.CreateDescriptionDictionary(this.RootFieldInfo.Children);
        }

        /// <inheritdoc />
        public ContainerNode RootFieldInfo { get; private set; }

        /// <inheritdoc />
        public IDataFieldInfo GetFieldDescriptionByMember(string name)
        {
            if (name == null)
            {
                return null;
            }
            else
            {
                return this.GetFieldDescriptionFromCache(name);
            }
        }

        public void AddFieldInfoToCache(IDataFieldInfo fieldInfo)
        {
            if (fieldInfo != null)
            {
                this.cachedFieldDescriptions[fieldInfo.Name] = fieldInfo;
            }
        }

        private IDataFieldInfo GetFieldDescriptionFromCache(string name)
        {
            if (this.cachedFieldDescriptions.ContainsKey(name))
            {
                return this.cachedFieldDescriptions[name];
            }

            return null;
        }

        private void CreateDescriptionDictionary(IEnumerable<ContainerNode> children)
        {
            if (children == null)
            {
                return;
            }

            foreach (var item in children)
            {
                this.AddToDescriptorsIfOkay(item);
                this.CreateDescriptionDictionary(item.Children);
            }
        }

        private void AddToDescriptorsIfOkay(ContainerNode node)
        {
            var descriptorNode = node as FieldInfoNode;

            if (descriptorNode != null)
            {
                this.cachedFieldDescriptions[descriptorNode.FieldInfo.Name] = descriptorNode.FieldInfo;
            }
        }
    }
}