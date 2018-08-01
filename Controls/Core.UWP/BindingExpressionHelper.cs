using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Telerik.Core
{
    /// <summary>
    /// Provides methods that allow getting property values without reflection.
    /// </summary>
    public sealed class BindingExpressionHelper : FrameworkElement
    {
        private static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(BindingExpressionHelper), null);

        private BindingExpressionHelper()
        {
        }

        /// <summary>
        /// Converts typed to untyped function.
        /// </summary>
        /// <typeparam name="T">The input parameter type of the function.</typeparam>
        /// <typeparam name="TResult">Return type of the function.</typeparam>
        /// <param name="func">That that will be converted.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Func<object, object> ToUntypedFunc<T, TResult>(Func<T, TResult> func)
        {
            return item => func.Invoke((T)item);
        }

        /// <summary>
        /// Returns a function that will return the value of the property, specified by the provided propertyPath.
        /// </summary>
        /// <param name="itemType">The type of the instance which property will be returned.</param>
        /// <param name="propertyPath">The path of the property which value will be returned.</param>
        public static Func<object, object> CreateGetValueFunc(Type itemType, string propertyPath)
        {
            // Strange problem in Design Mode - Missing method exceptions are thrown when we try to compile the lambda below.
            if (!itemType.GetTypeInfo().IsPublic || DesignMode.DesignModeEnabled ||
                (propertyPath != null && propertyPath.IndexOfAny(new char[] { '.', '[', ']', '(', ')', '@' }) > -1))
            {
                return (item) => BindingExpressionHelper.GetValueThroughBinding(item, propertyPath);
            }

            var parameter = Expression.Parameter(itemType, "item");
            Expression getter;
            if (string.IsNullOrEmpty(propertyPath))
            {
                getter = parameter;
            }
            else
            {
                getter = Expression.PropertyOrField(parameter, propertyPath);
            }
            var lambda = Expression.Lambda(getter, parameter);
            var compiled = lambda.Compile();
            var methodInfo = typeof(BindingExpressionHelper).GetTypeInfo()
                                                           .GetDeclaredMethod("ToUntypedFunc")
                                                           .MakeGenericMethod(new[] { itemType, lambda.Body.Type });
            return (Func<object, object>)methodInfo.Invoke(null, new object[] { compiled });
        }

        private static object GetValueThroughBinding(object item, Binding binding)
        {
            BindingExpressionHelper helper = new BindingExpressionHelper();
            try
            {
                helper.DataContext = item;
                BindingOperations.SetBinding(helper, ValueProperty, binding);
                return helper.GetValue(ValueProperty);
            }
            finally
            {
                helper.ClearValue(BindingExpressionHelper.ValueProperty);
            }
        }

        private static object GetValueThroughBinding(object item, string propertyPath)
        {
            var binding = new Binding();
            binding.Path = new PropertyPath(propertyPath ?? ".");

            return GetValueThroughBinding(item, binding);
        }
    }
}