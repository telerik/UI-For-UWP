using System;
using System.ComponentModel;

namespace Telerik.Core.Data
{
    /// <summary>
    /// Encapsulates the data, associated with a single item property change within the source collection.
    /// </summary>
    public class ItemPropertyChangedEventArgs : PropertyChangedEventArgs
    {
        private object item;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemPropertyChangedEventArgs"/> class.
        /// </summary>
        /// <param name="item">The item which property has changed.</param>
        /// <param name="propName">The name of the property.</param>
        public ItemPropertyChangedEventArgs(object item, string propName)
            : base(propName)
        {
            this.item = item;
        }

        /// <summary>
        /// Gets the raw data item instance which property has changed.
        /// </summary>
        public object Item
        {
            get
            {
                return this.item;
            }
        }
    }
}
