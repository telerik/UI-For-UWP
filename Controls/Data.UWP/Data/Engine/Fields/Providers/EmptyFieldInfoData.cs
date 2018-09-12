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

        public void AddFieldInfoToCache(IDataFieldInfo fieldInfo)
        {
        }

        public IDataFieldInfo GetFieldDescriptionByMember(string name)
        {
            return null;
        }
    }
}
