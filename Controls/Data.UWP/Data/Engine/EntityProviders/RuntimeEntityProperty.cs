using System;
using System.Collections;
using System.Reflection;
using Windows.UI.Xaml.Data;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Represents a model based on PropertyInfo of Entity with ability to extract its metadata, commit and revert data.
    /// </summary>
    /// <seealso cref="Telerik.Data.Core.EntityProperty" />
    public class RuntimeEntityProperty : EntityProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeEntityProperty"/> class.
        /// </summary>
        /// <param name="property">The property info that object is associated with.</param>
        /// <param name="item">The data item.</param>
        /// <param name="converter">The converter. Optionally you can convert your source property to a different format depending on the UI and scenario.</param>
        public RuntimeEntityProperty(PropertyInfo property, object item, IPropertyConverter converter) : base(property, item, converter)
        {
            this.SetMethod = property.SetMethod;
            this.GetMethod = property.GetMethod;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeEntityProperty"/> class.
        /// </summary>
        /// <param name="property">The property info that object is associated with.</param>
        /// <param name="item">The data item.</param>
        public RuntimeEntityProperty(PropertyInfo property, object item) : this(property, item, null)
        {
        }

        private MethodInfo SetMethod { get; set; }

        private MethodInfo GetMethod { get; set; }

        /// <summary>
        /// Gets the original value from the source object.
        /// </summary>
        public override object GetOriginalValue()
        {
            if (this.GetMethod != null)
            {
                return this.GetMethod.Invoke(this.DataItem, new object[0]);
            }

            return null;
        }

        /// <summary>
        /// Commits the pending value in property value to the data item.
        /// </summary>
        public override void Commit()
        {
            if (this.SetMethod != null)
            {
                var value = this.PropertyConverter != null ? this.PropertyConverter.ConvertBack(this.PropertyValue) : this.PropertyValue;
                var type = this.PropertyType;

                if (value != null && value.GetType() != type)
                {
                    if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        type = Nullable.GetUnderlyingType(type);
                    }

                    value = Convert.ChangeType(value, type);
                }

                this.SetMethod.Invoke(this.DataItem, new object[1] { value });
            }
        }

        /// <summary>
        /// Gets the index of the property.
        /// </summary>
        /// <param name="property">The property.</param>
        protected override int GetPropertyIndex(object property)
        {
            var propertyInfo = property as PropertyInfo;

            if (propertyInfo != null)
            {
                DisplayAttribute attribute = propertyInfo.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;
                if (attribute != null)
                {
                    return attribute.Position;
                }
            }

            return 0;
        }
        
        /// <inheritdoc />
        protected override string GetPropertyGroupKey(object property)
        {
            var propertyInfo = property as PropertyInfo;

            if (propertyInfo != null)
            {
                DisplayAttribute attribute = propertyInfo.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;
                if (attribute != null)
                {
                    string groupName = attribute.Group;
                    if (groupName != null)
                    {
                        return groupName;
                    }
                }
            }
            return null;
        }

        /// <inheritdoc />
        protected override string GetPropertyName(object property)
        {
            var propertyInfo = property as PropertyInfo;
            if (propertyInfo != null)
            {
                return propertyInfo.Name;
            }
            return null;
        }

        /// <inheritdoc />
        protected override Type GetPropertyType(object property)
        {
            var propertyInfo = property as PropertyInfo;
            if (propertyInfo != null)
            {
                return propertyInfo.PropertyType;
            }
            return null;
        }

        /// <inheritdoc />
        protected override bool GetIsReadOnly(object property)
        {
            var propertyInfo = property as PropertyInfo;
            if (propertyInfo != null)
            {
                ReadOnlyAttribute attribute = propertyInfo.GetCustomAttribute(typeof(ReadOnlyAttribute)) as ReadOnlyAttribute;
                if (attribute != null)
                {
                    return true;
                }
            }
            return false;
        }

        /// <inheritdoc />
        protected override string GetLabel(object property)
        {
            var propertyInfo = property as PropertyInfo;

            if (propertyInfo != null)
            {
                DisplayAttribute attribute = propertyInfo.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;
                if (attribute != null)
                {
                    return attribute.Header;
                }
            }
            return null;
        }

        /// <inheritdoc />
        protected override string GetWatermark(object property)
        {
            var propertyInfo = property as PropertyInfo;

            if (propertyInfo != null)
            {
                DisplayAttribute attribute = propertyInfo.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;
                if (attribute != null)
                {
                    return attribute.PlaceholderText;
                }
            }

            return null;
        }

        /// <inheritdoc />
        protected override bool GetIsRequired(object property)
        {
            var propertyInfo = property as PropertyInfo;
            if (propertyInfo != null)
            {
                RequiredAttribute attribute = propertyInfo.GetCustomAttribute(typeof(RequiredAttribute)) as RequiredAttribute;
                if (attribute != null)
                {
                    return true;
                }
            }
            return false;
        }

        /// <inheritdoc />
        protected override IList GetValueOptions(object property)
        {
            var propertyInfo = property as PropertyInfo;
            if (propertyInfo != null)
            {
                var sourceAttr = propertyInfo.GetCustomAttribute(typeof(ValueOptionsPropertyNameAttribute)) as ValueOptionsPropertyNameAttribute;
                if (sourceAttr != null)
                {
                    var sourceProperty = this.DataItem.GetType().GetRuntimeProperty(sourceAttr.PropertyName);
                    if (sourceProperty != null)
                    {
                        return sourceProperty.GetValue(this.DataItem) as IList;
                    }
                }
            }

            var attr = this.DataItem.GetType().GetTypeInfo().GetCustomAttribute(typeof(PropertyValueOptionsAttribute)) as PropertyValueOptionsAttribute;
            if (attr != null && attr.OptionsProvider != null)
            {
                return (Activator.CreateInstance(attr.OptionsProvider) as IValueOptionsProvider).GetOptions(this);
            }

            return null;
        }

        /// <inheritdoc />
        protected override INumericalRange GetValueRange(object property)
        {
            var propertyInfo = property as PropertyInfo;
            if (propertyInfo != null)
            {
                var rangeAttr = propertyInfo.GetCustomAttribute(typeof(ValueRangeAttribute)) as ValueRangeAttribute;
                if (rangeAttr != null)
                {
                    return rangeAttr.Range;
                }
            }

            return null;
        }
    }
}