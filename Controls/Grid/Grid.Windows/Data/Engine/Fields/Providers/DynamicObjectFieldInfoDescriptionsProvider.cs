using System.Collections.Generic;
using System.Dynamic;

namespace Telerik.Data.Core.Fields
{
    internal class DynamicObjectFieldInfoDescriptionsProvider : IFieldInfoExtractor
    {
        private DynamicObject firstItem;

        public DynamicObjectFieldInfoDescriptionsProvider(DynamicObject item)
        {
            this.firstItem = item;
        }

        public bool IsInitialized
        {
            get
            {
                return this.firstItem != null;
            }
        }

        public IEnumerable<IDataFieldInfo> GetDescriptions()
        {
            List<IDataFieldInfo> infos = new List<IDataFieldInfo>();

            if (this.firstItem != null)
            {
                foreach (var name in this.firstItem.GetDynamicMemberNames())
                {
                    var info = new DynamicObjectFieldInfo(name);

                    // the GetValue call will update the DataType member of the filed
                    info.GetValue(this.firstItem);

                    infos.Add(info);
                }
            }

            return infos;
        }
    }
}
