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
    }
}
