using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;
using Telerik.Core;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a <see cref="DataPointBinding"/> that uses reflection to look-up the values for the generated data points.
    /// </summary>
    public class PropertyNameDataPointBinding : DataPointBinding
    {
        private string propertyName;
        private Func<object, object> getter;
        private Type getterInstanceType;
        private CallSite<Func<CallSite, object, object>> callSite;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyNameDataPointBinding"/> class.
        /// </summary>
        public PropertyNameDataPointBinding()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyNameDataPointBinding"/> class.
        /// </summary>
        /// <param name="propertyName">The name of the property which value is bound.</param>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "These virtual calls do not rely on uninitialized base state.")]
        public PropertyNameDataPointBinding(string propertyName)
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
        public override object GetValue(object instance)
        {
            object dynamicValue;
            if (this.TryGetValueFromDynamic(instance, out dynamicValue))
            {
                return dynamicValue;
            }

            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (string.IsNullOrEmpty(this.propertyName))
            {
                throw new ArgumentException("PropertyName not specified.");
            }

            Type instanceType = instance.GetType();
            if (this.getter == null || !object.ReferenceEquals(this.getterInstanceType, instanceType))
            {
                this.getter = DynamicHelper.CreatePropertyValueGetter(instanceType, this.propertyName);
                this.getterInstanceType = instanceType;
            }

            return this.getter(instance);
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
