using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Telerik.Data.Core
{
    internal class BindingExpressionHelper
    {
        /// <summary>
        /// Returns a function that will return the value of the property, specified by the provided propertyPath.
        /// </summary>
        /// <param name="itemType">The type of the instance which property will be returned.</param>
        /// <param name="propertyPath">The path of the property which value will be returned.</param>
        public static Func<object, object> CreateGetValueFunc(Type itemType, string propertyPath)
        {
            PropertyInfo propertyInfo = itemType.GetRuntimeProperties().Where(a => a.Name.Equals(propertyPath) && !a.GetIndexParameters().Any()).FirstOrDefault();
            return item => propertyInfo?.GetValue(item);
        }

        public static Func<object, object> CreateGetNestedValueFunc(List<PropertyInfo> propertyInfos, string baseClassName)
        {
            return item =>
            {
                if (item == null)
                {
                    return null;
                }

                PropertyInfo basePropertyInfo = item.GetType().GetProperty(baseClassName);
                item = basePropertyInfo.GetValue(item);

                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    item = propertyInfo.GetValue(item);
                }

                return item;
            };
        }

        public static Action<object, object> CreateSetNestedValueFunc(List<PropertyInfo> propertyInfos, string baseClassName)
        {
            return new Action<object, object>((item, propertyValue) =>
            {
                if (item == null)
                {
                    return;
                }

                PropertyInfo basePropertyInfo = item.GetType().GetProperty(baseClassName);
                item = basePropertyInfo.GetValue(item);

                for (int i = 0; i < propertyInfos.Count - 1; i++)
                {
                    PropertyInfo info = propertyInfos[i];
                    item = info.GetValue(item);
                }

                PropertyInfo propertyInfo = propertyInfos.Last();
                propertyInfo.SetValue(item, propertyValue);
            });
        }

        internal static Action<object, object> CreateSetValueAction(Type itemType, string propertyPath)
        {
            var itemInfo = itemType.GetRuntimeProperties().Where(a => a.Name.Equals(propertyPath) && !a.GetIndexParameters().Any()).FirstOrDefault();

            if (itemInfo != null && itemInfo.CanWrite)
            {
                return new Action<object, object>((item, propertyValue) => itemInfo.SetMethod.Invoke(item, new object[] { propertyValue }));
            }

            return null;
        }

        private static Func<object, object> ToUntypedFunc<T, TResult>(Func<T, TResult> func)
        {
            return item => func.Invoke((T)item);
        }
    }
}