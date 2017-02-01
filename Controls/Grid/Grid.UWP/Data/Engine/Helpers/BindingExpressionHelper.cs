using System;
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
            var parameter = Expression.Parameter(itemType, "item");

            Expression get;
            if (string.IsNullOrEmpty(propertyPath))
            {
                get = parameter;
            }
            else
            {
                try
                {
                    get = Expression.PropertyOrField(parameter, propertyPath);
                }
                catch (ArgumentException)
                {
                    return p => null;
                }
            }

            var lambda = Expression.Lambda(get, parameter);

            var compiled = lambda.Compile();

            var methodInfo = typeof(BindingExpressionHelper).GetTypeInfo()
                 .GetDeclaredMethod("ToUntypedFunc")
                 .MakeGenericMethod(new[] { itemType, lambda.Body.Type });

            return (Func<object, object>)methodInfo.Invoke(null, new object[] { compiled });
        }

        internal static Action<object, object> CreateSetValueAction(Type itemType, string propertyPath)
        {
            var itemInfo = itemType.GetRuntimeProperty(propertyPath);

            if (itemInfo.CanWrite)
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