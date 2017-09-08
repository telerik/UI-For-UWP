using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;

namespace Telerik.Core.Data
{
    /// <summary>
    /// Represents a concrete <see cref="ValueLookup"/> implementation that retrieves property value given the name of the property.
    /// </summary>
    public class PropertyValueLookup : ValueLookup
    {
        private string propertyName;
        private Func<object, object> getter;
        private Type getterInstanceType;
        private CallSite<Func<CallSite, object, object>> callSite;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyValueLookup"/> class.
        /// </summary>
        public PropertyValueLookup()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyValueLookup"/> class.
        /// </summary>
        /// <param name="propertyName">The name of the property which value is bound.</param>
        public PropertyValueLookup(string propertyName)
        {
            this.PropertyName = propertyName;
        }

        /// <summary>
        /// Gets or sets the name of the property which value is bound.
        /// </summary>
        public string PropertyName
        {
            get
            {
                return this.propertyName;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException(nameof(value));
                }

                if (this.propertyName == value)
                {
                    return;
                }

                this.getter = null;
                this.callSite = null;
                this.propertyName = value;
                this.OnPropertyChanged(nameof(this.PropertyName));
            }
        }

        /// <summary>
        /// Retrieves the value for the specified object instance.
        /// </summary>
        public override object GetValueForItem(object dataItem)
        {
            object dynamicValue;
            if (this.TryGetValueFromDynamic(dataItem, out dynamicValue))
            {
                return dynamicValue;
            }

            if (dataItem == null)
            {
                throw new ArgumentNullException(nameof(dataItem));
            }

            if (string.IsNullOrEmpty(this.propertyName))
            {
                throw new ArgumentException("PropertyName not specified.");
            }

            Type instanceType = dataItem.GetType();
            if (this.getter == null || !object.ReferenceEquals(this.getterInstanceType, instanceType))
            {
                this.getter = DynamicHelper.CreatePropertyValueGetter(instanceType, this.propertyName);
                this.getterInstanceType = instanceType;
            }

            return this.getter(dataItem);
        }

        private bool TryGetValueFromDynamic(object instance, out object dynamicValue)
        {
            dynamicValue = null;

            // ExpandoObject
            IDictionary<string, object> store = instance as IDictionary<string, object>;
            if (store != null)
            {
                dynamicValue = store[this.propertyName];
                return true;
            }

            DynamicObject dynamicObject = instance as DynamicObject;
            if (dynamicObject != null)
            {
                if (this.callSite == null)
                {
                    var binder = Binder.GetMember(CSharpBinderFlags.None, this.propertyName, instance.GetType(), new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
                    this.callSite = CallSite<Func<CallSite, object, object>>.Create(binder);
                }

                dynamicValue = this.callSite.Target(this.callSite, instance);
                return true;
            }

            return false;
        }
    }
}
