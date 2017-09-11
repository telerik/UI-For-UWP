using System;
using System.Linq;
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