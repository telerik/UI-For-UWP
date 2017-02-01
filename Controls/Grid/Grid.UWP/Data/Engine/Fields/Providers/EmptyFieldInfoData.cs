using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Telerik.Data.Core.Fields
{
    internal class EmptyFieldInfoData : IFieldInfoData
    {
        private readonly ContainerNode rootFieldInfo;

        public EmptyFieldInfoData()
        {
            this.rootFieldInfo = ContainerNode.CreateRootNode();
        }

        public ContainerNode RootFieldInfo
        {
            get { return this.rootFieldInfo; }
        }

        public IDataFieldInfo GetFieldDescriptionByMember(string name)
        {
            return null;
        }
    }
}
