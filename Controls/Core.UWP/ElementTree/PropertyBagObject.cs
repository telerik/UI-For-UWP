using System.ComponentModel;

namespace Telerik.Core
{
    /// <summary>
    /// Represents an object that stores its properties within a property bag.
    /// </summary>
    public abstract class PropertyBagObject
    {
        internal FastPropertyStore propertyStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBagObject"/> class.
        /// </summary>
        protected PropertyBagObject()
        {
            this.propertyStore = new FastPropertyStore();
        }

        internal bool IsLocalValue(int key)
        {
            return this.propertyStore.ContainsEntry(key);
        }

        internal object GetValue(int key)
        {
            return this.propertyStore.GetEntry(key);
        }

        internal bool SetValue(int key, object value)
        {
            return this.SetValueCore(key, value);
        }

        internal bool ClearValue(int key)
        {
            return this.ClearValueCore(key);
        }

        internal virtual bool SetValueCore(int key, object value)
        {
            this.propertyStore.SetEntry(key, value);
            return true;
        }

        internal virtual bool ClearValueCore(int key)
        {
            this.propertyStore.RemoveEntry(key);
            return true;
        }

        internal T GetTypedValue<T>(int key, T defaultValue)
        {
            object localValue = this.GetValue(key);
            if (localValue != null)
            {
                return (T)localValue;
            }

            return defaultValue;
        }
    }
}
