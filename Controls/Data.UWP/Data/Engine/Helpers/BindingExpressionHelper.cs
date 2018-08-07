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
            Expression getter = BindingExpressionHelper.GenerateMemberExpression(propertyPath, parameter);

            var lambda = Expression.Lambda(getter, parameter);
            var compiled = lambda.Compile();
            var methodInfo = typeof(BindingExpressionHelper).GetTypeInfo()
                .GetDeclaredMethod("ToUntypedSingleParamFunc")
                .MakeGenericMethod(new[] { itemType, lambda.Body.Type });
            return (Func<object, object>)methodInfo.Invoke(null, new object[] { compiled });
        }

        internal static Action<object, object> CreateSetValueAction(Type itemType, string propertyPath)
        {
            var setValueFunc = BindingExpressionHelper.CreateSetValueFunc(itemType, propertyPath);
            return setValueFunc != null ? new Action<object, object>((item, propertyValue) => setValueFunc?.Invoke(item, propertyValue)) : null;
        }

        internal static Func<object, object, object> CreateSetValueFunc(Type itemType, string propertyPath)
        {
            var parameter = Expression.Parameter(itemType, "item");
            Expression getter = BindingExpressionHelper.GenerateMemberExpression(propertyPath, parameter);
            if (getter.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression memberExpression = (MemberExpression)getter;
                PropertyInfo propertyInfo = (PropertyInfo)memberExpression.Member;
                if (!propertyInfo.CanWrite)
                {
                    return null;
                }
            }

            ParameterExpression valueExp = Expression.Parameter(getter.Type, "propertyValue");
            BinaryExpression assignExp = Expression.Assign(getter, valueExp);

            var lambda = Expression.Lambda(assignExp, parameter, valueExp);
            var compiled = lambda.Compile();

            var declaredActionMethod = typeof(BindingExpressionHelper).GetTypeInfo().GetDeclaredMethod("ToUntypedDoubleParamFunc");
            var methodInfo = declaredActionMethod.MakeGenericMethod(itemType, lambda.Body.Type, lambda.Body.Type);
            return (Func<object, object, object>)methodInfo.Invoke(null, new object[] { compiled });
        }

        private static Func<object, object> ToUntypedSingleParamFunc<T, TResult>(Func<T, TResult> func)
        {
            return item => func.Invoke((T)item);
        }

        private static Func<object, object, object> ToUntypedDoubleParamFunc<T, U, TResult>(Func<T, U, TResult> func)
        {
            return (item, propertyValue) => func.Invoke((T)item, (U)propertyValue);
        }

        private static Expression GenerateMemberExpression(string propertyPath, ParameterExpression parameter)
        {
            Expression getter;
            if (string.IsNullOrEmpty(propertyPath))
            {
                getter = parameter;
            }
            else
            {
                if (propertyPath.Contains("."))
                {
                    getter = parameter;
                    foreach (var member in propertyPath.Split('.'))
                    {
                        getter = Expression.PropertyOrField(getter, member);
                    }
                }
                else
                {
                    getter = Expression.PropertyOrField(parameter, propertyPath);
                }
            }

            return getter;
        }
    }
}