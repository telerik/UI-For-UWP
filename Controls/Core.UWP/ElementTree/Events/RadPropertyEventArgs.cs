using System;

namespace Telerik.Core
{
    /// <summary>
    /// Encapsulates all the data associated with a change in a <see cref="PropertyBagObject"/> property store.
    /// </summary>
    public class RadPropertyEventArgs : EventArgs
    {
        private int key;
        private string propertyName;
        private object oldValue;
        private object newValue;
        private bool cancel;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadPropertyEventArgs"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        public RadPropertyEventArgs(int key, object oldValue, object newValue)
        {
            this.key = key;
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RadPropertyEventArgs"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        public RadPropertyEventArgs(string name, object oldValue, object newValue)
        {
            this.propertyName = name;
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the property change can be accepted. Valid during the Changing pass.
        /// </summary>
        public bool Cancel
        {
            get
            {
                return this.cancel;
            }
            set
            {
                this.cancel = value;
            }
        }

        /// <summary>
        /// Gets the key of the property associated with the event.
        /// </summary>
        public int Key
        {
            get
            {
                return this.key;
            }
        }

        /// <summary>
        /// Gets the name of the property that has changed. This member is not set if the property is associated with a valid key.
        /// </summary>
        public string PropertyName
        {
            get
            {
                return this.propertyName;
            }
        }

        /// <summary>
        /// Gets the old value of the associated property.
        /// </summary>
        public object OldValue
        {
            get
            {
                return this.oldValue;
            }
        }

        /// <summary>
        /// Gets or sets the new value of the associated property.
        /// </summary>
        public object NewValue
        {
            get
            {
                return this.newValue;
            }
            set
            {
                this.newValue = value;
            }
        }
    }
}
