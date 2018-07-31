using System;
using System.Reflection;
using Telerik.Core;

namespace Telerik.Data.Core.Fields
{
    internal class NestedPropertyInfoFieldInfo : IDataFieldInfo, IMemberAccess
    {
        private readonly string nestedPropertyName;
        private PropertyInfo propertyInfo;

        internal NestedPropertyInfoFieldInfo(PropertyInfo propertyInfo, string nestedPropertyName) 
        {
            this.propertyInfo = propertyInfo;
            this.nestedPropertyName = nestedPropertyName;
        }

        public string DisplayName
        {
            get
            {
                return this.nestedPropertyName;
            }
        }

        public string Name
        {
            get
            {
                return this.nestedPropertyName;
            }
        }

        public Type DataType
        {
            get
            {
                return this.propertyInfo.PropertyType;
            }
        }

        public FieldRole Role
        {
            get;
            set;
        }

        public object GetValue(object item)
        {
            return ReflectionHelper.GetNestedPropertyValue(item, this.nestedPropertyName);
        }

        public void SetValue(object item, object fieldValue)
        {
            ReflectionHelper.SetNestedPropertyValue(item, fieldValue, this.nestedPropertyName);
        }

        public bool Equals(IDataFieldInfo info)
        {
            var propertyField = info as NestedPropertyInfoFieldInfo;
            if (propertyField == null)
            {
                return false;
            }

            return propertyField.propertyInfo.DeclaringType.GetTypeInfo().IsAssignableFrom(this.propertyInfo.DeclaringType.GetTypeInfo());
        }
    }
}
