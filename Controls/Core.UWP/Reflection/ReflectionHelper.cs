using System;
using System.Reflection;

namespace Telerik.Core
{
    internal static class ReflectionHelper
    {
        public static PropertyInfo GetProperty(Type type, string propertyName)
        {
            TypeInfo typeInfo = type.GetTypeInfo();

            var property = typeInfo.GetDeclaredProperty(propertyName);
            if (property != null)
            {
                return property;
            }

            Type baseType = typeInfo.BaseType;
            if (baseType == null)
            {
                return null;
            }

            return GetProperty(baseType, propertyName);
        }

        public static MethodInfo GetMethod(Type type, string methodName)
        {
            TypeInfo typeInfo = type.GetTypeInfo();

            var method = typeInfo.GetDeclaredMethod(methodName);
            if (method != null)
            {
                return method;
            }

            Type baseType = typeInfo.BaseType;
            if (baseType == null)
            {
                return null;
            }

            return GetMethod(baseType, methodName);
        }

        public static bool TryGetPropertyValue(this object obj, string propertyName, out object value)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                value = obj;
                return true;
            }

            if (obj != null)
            {
                var info = obj.GetType().GetRuntimeProperty(propertyName);
                if (info != null)
                {
                    value = info.GetValue(obj);
                    return true;
                }
            }

            value = null;
            return false;
        }

        public static object GetPropertyValue(this object obj, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return obj;
            }

            if (obj != null)
            {
                var info = obj.GetType().GetRuntimeProperty(propertyName);
                if (info != null)
                {
                    return info.GetValue(obj);
                }
            }

            return null;
        }

        public static object GetNestedPropertyValue(object item, string nestedPropertyName)
        {
            if (item == null)
            {
                return null;
            }

            string[] splitPropertyName = nestedPropertyName.Split(new char[] { '.' });
            foreach (string propertyName in splitPropertyName)
            {
                Type itemType = item.GetType();
                PropertyInfo propertyInfo = itemType.GetProperty(propertyName);
                if (propertyInfo == null)
                {
                    return null;
                }

                item = propertyInfo.GetValue(item);
            }

            return item;
        }

        public static void SetNestedPropertyValue(object item, object newValue, string nestedPropertyName)
        {
            if (item == null)
            {
                return;
            }

            string[] splitPropertyName = nestedPropertyName.Split(new char[] { '.' });
            PropertyInfo propertyInfo = null;

            for (int i = 0; i < splitPropertyName.Length; i++)
            {
                Type itemType = item.GetType();
                string propertyName = splitPropertyName[i];
                propertyInfo = itemType.GetProperty(propertyName);
                if (propertyInfo == null)
                {
                    return;
                }

                if (splitPropertyName.Length != i + 1)
                {
                    item = propertyInfo.GetValue(item);
                }
            }

            if (propertyInfo.CanWrite)
            {
                propertyInfo.SetValue(item, newValue);
            }
        }
    }
}
