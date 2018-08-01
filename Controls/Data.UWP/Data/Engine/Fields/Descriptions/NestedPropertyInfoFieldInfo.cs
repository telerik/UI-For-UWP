using System;
using System.Reflection;

namespace Telerik.Data.Core.Fields
{
    internal class NestedPropertyInfoFieldInfo : PropertyInfoFieldInfo
    {
        private readonly string nestedPropertyName;

        internal NestedPropertyInfoFieldInfo(PropertyInfo propertyInfo, Func<object, object> propertyAccess,
            Action<object, object> propertySetter, string nestedPropertyName) 
            : base(propertyInfo, propertyAccess, propertySetter)
        {
            this.nestedPropertyName = nestedPropertyName;
        }

        public override string DisplayName
        {
            get
            {
                return this.nestedPropertyName;
            }
        }

        public override string Name
        {
            get
            {
                return this.nestedPropertyName;
            }
        }
    }
}
