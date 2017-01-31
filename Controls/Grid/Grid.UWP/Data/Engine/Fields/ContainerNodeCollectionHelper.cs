using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Telerik.Data.Core.Fields
{
    internal class ContainerNodeCollectionHelper
    {
        public static ContainerNode GetOrCreateFolderNodes(ContainerNode node, IList<string> folderNames)
        {
            var folderNamesList = new LinkedList<string>(folderNames);

            return GetOrCreateFolderNodes(node, folderNamesList.First);
        }

        private static ContainerNode GetOrCreateFolderNodes(ContainerNode node, LinkedListNode<string> currentFolderName)
        {
            if (currentFolderName == null)
            {
                return node;
            }

            var folderName = currentFolderName.Value;
            var foundNode = FindChildNodeByUniqueName(node, folderName);

            if (foundNode == null)
            {
                foundNode = new ContainerNode(folderName, ContainerNodeRole.Folder);
                node.Children.Add(foundNode);
            }

            return GetOrCreateFolderNodes(foundNode, currentFolderName.Next);
        }
  
        private static ContainerNode FindChildNodeByUniqueName(ContainerNode node, string folderName)
        {
            foreach (var nodeItem in node.Children)
            {
                if (nodeItem.Name == folderName)
                {
                    return nodeItem;
                }
            }

            return null;
        }
    }
}
