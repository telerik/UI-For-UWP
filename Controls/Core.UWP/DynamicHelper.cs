using System;
using System.Globalization;
using System.Reflection;

namespace Telerik.Core
{
    /// <summary>
    /// Encapsulates helper methods to generate Dynamic methods using System.Reflection.Emit.
    /// </summary>
    public static class DynamicHelper
    {
        /// <summary>
        /// Generates a untyped function to allow retrieving property values for instances of the specified type without using reflection.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="propertyName">Name of the property.</param>
        public static Func<object, object> CreatePropertyValueGetter(Type type, string propertyName)
        {
            if (type == null)
            {
                throw new ArgumentException("type argument is NULL.");
            }

            if (propertyName == null)
            {
                throw new ArgumentException("propertyName argument is NULL.");
            }

            TypeInfo info = type.GetTypeInfo();
            if (info.IsNotPublic)
            {
                throw new ArgumentException("Cannot create dynamic property getter for non-public types.");
            }

            if (info.IsValueType)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Dynamic getter is not supported for value type [{0}]", info.Name));
            }

            return BindingExpressionHelper.CreateGetValueFunc(type, propertyName);
        }
    }
}