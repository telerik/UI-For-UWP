using System.Collections.Generic;
using System.Dynamic;

namespace Telerik.Data.Core.Fields
{
    internal class ExpandoObjectFieldDescriptionsInfoProvider : IFieldInfoExtractor
    {
        private ExpandoObject firstItem;

        public ExpandoObjectFieldDescriptionsInfoProvider(ExpandoObject item)
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

        public bool IsDynamic
        {
            get
            {
                return true;
            }
        }

        public IEnumerable<IDataFieldInfo> GetDescriptions()
        {
            List<IDataFieldInfo> infos = new List<IDataFieldInfo>();

            if (this.firstItem != null)
            {
                foreach (var pair in this.firstItem)
                {
                    var info = new ExpandoObjectFieldInfo(pair.Key);

                    // the GetValue call will update the DataType member of the filed
                    info.GetValue(this.firstItem);

                    infos.Add(info);
                }
            }

            return infos;
        }
    }
}