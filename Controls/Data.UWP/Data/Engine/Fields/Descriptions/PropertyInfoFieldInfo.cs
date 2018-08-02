using System;
using System.Reflection;

namespace Telerik.Data.Core.Fields
{
    /// <summary>
    /// An <see cref="IDataFieldInfo"/> that uses <see cref="Func{Object, Object}"/> for property access.
    /// </summary>
    internal class PropertyInfoFieldInfo : IDataFieldInfo, IMemberAccess
    {
        private readonly string nestedPropertyName;
        private Action<object, object> propertySetter;
        private Func<object, object> propertyAccess;
        private PropertyInfo propertyInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyInfoFieldInfo"/> class.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        public PropertyInfoFieldInfo(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            this.propertyInfo = propertyInfo;
        }

        internal PropertyInfoFieldInfo(PropertyInfo propertyInfo, Func<object, object> propertyAccess, Action<object, object> propertySetter, string nestedPropertyName)
            : this(propertyInfo, propertyAccess, propertySetter)
        {
            this.nestedPropertyName = nestedPropertyName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyInfoFieldInfo" /> class.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <param name="propertyAccess">The property access.</param>
        /// <param name="propertySetter">The property setter.</param>
        internal PropertyInfoFieldInfo(PropertyInfo propertyInfo, Func<object, object> propertyAccess, Action<object, object> propertySetter) : this(propertyInfo)
        {
            if (propertyAccess == null)
            {
                throw new ArgumentNullException(nameof(propertyAccess));
            }

            this.propertyAccess = propertyAccess;

            this.propertySetter = propertySetter;
        }

        internal PropertyInfoFieldInfo(PropertyInfo propertyInfo, Func<object, object> propertyAccess)
            : this(propertyInfo, propertyAccess, null)
        {
        }

        /// <inheritdoc />
        public string DisplayName
        {
            get
            {
                // TODO:: read attributes !!!
                return this.propertyInfo.Name;
            }
        }

        /// <inheritdoc />
        public Type DataType
        {
            get
            {
                return this.propertyInfo.PropertyType;
            }
        }

        /// <inheritdoc />
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(this.nestedPropertyName))
                {
                    return this.propertyInfo.Name;
                }

                return this.nestedPropertyName;
            }
        }

        /// <inheritdoc />
        public FieldRole Role
        {
            get;
            set;
        }

        /// <inheritdoc />
        public object GetValue(object item)
        {
            if (this.propertyAccess != null)
            {
                return this.propertyAccess(item);
            }
            else
            {
                return this.propertyInfo.GetValue(item, null);
            }
        }

        /// <inheritdoc />
        public void SetValue(object item, object fieldValue)
        {
            if (this.propertySetter != null)
            {
                this.propertySetter(item, fieldValue);
            }
        }

        public bool Equals(IDataFieldInfo info)
        {
            var propertyField = info as PropertyInfoFieldInfo;
            if (propertyField == null)
            {
                return false;
            }

            return propertyField.propertyInfo.DeclaringType.GetTypeInfo().IsAssignableFrom(this.propertyInfo.DeclaringType.GetTypeInfo());
        }
    }
}