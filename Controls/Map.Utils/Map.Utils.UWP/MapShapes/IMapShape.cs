using System;

namespace Telerik.Geospatial
{
    /// <summary>
    /// Represents the method that will handle the <see cref="IMapShape.AttributeChanged"/> event.
    /// </summary>
    public delegate void AttributeChangedEventHandler(object sender, AttributeChangedEventArgs e);

    /// <summary>
    /// Defines the abstraction of a geographical shape.
    /// </summary>
    public interface IMapShape
    {
        /// <summary>
        /// Occurs when an attribute of the <see cref="IMapShape"/> is changed.
        /// </summary>
        event AttributeChangedEventHandler AttributeChanged;

        /// <summary>
        /// Gets the attribute value associated with the provided key.
        /// </summary>
        object GetAttribute(string key);

        /// <summary>
        /// Sets the attribute value associated with the provided key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        void SetAttribute(string key, object value);
    }

    /// <summary>
    /// Provides data for the <see cref="IMapShape.AttributeChanged"/> event.
    /// </summary>
    public class AttributeChangedEventArgs : EventArgs
    {
        private string attributeName;
        private object oldValue;
        private object newValue;
        private bool cancel;

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeChangedEventArgs"/> class.
        /// </summary>
        /// <param name="attributeName">The attribute name.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        public AttributeChangedEventArgs(string attributeName, object oldValue, object newValue)
        {
            this.attributeName = attributeName;
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the attribute change can be accepted. Valid during the Changing pass.
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
        /// Gets the name of the attribute associated with the event.
        /// </summary>
        public string AttributeName
        {
            get
            {
                return this.attributeName;
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