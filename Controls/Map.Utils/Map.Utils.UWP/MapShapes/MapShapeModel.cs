using System;
using System.Collections.Generic;

namespace Telerik.Geospatial
{
    internal abstract class MapShapeModel : IMapShape
    {
        private static int uniqueIdCounter = 0;

        private int uniqueId = -1;
        private Dictionary<string, object> attributes;

        /// <summary>
        /// Occurs when an attribute of the <see cref="MapShapeModel"/> is changed.
        /// </summary>
        public event AttributeChangedEventHandler AttributeChanged;

        public Dictionary<string, object> Attributes
        {
            get
            {
                if (this.attributes == null)
                {
                    this.attributes = new Dictionary<string, object>();
                }

                return this.attributes;
            }
        }

        /// <summary>
        /// Gets the unique identifier of the shape model; used for hashing purposes.
        /// </summary>
        internal int UniqueId 
        {
            get
            {
                if (this.uniqueId == -1)
                {
                    this.uniqueId = uniqueIdCounter++;
                }

                return this.uniqueId;
            }
        }

        object IMapShape.GetAttribute(string key)
        {
            if (this.attributes == null)
            {
                return null;
            }

            object value;
            if (this.attributes.TryGetValue(key, out value))
            {
                return value;
            }

            return null;
        }

        void IMapShape.SetAttribute(string key, object value)
        {
            if (this.attributes == null || string.IsNullOrEmpty(key))
            {
                return;
            }

            var oldValue = (this as IMapShape).GetAttribute(key);
            if (oldValue != value)
            {
                this.attributes[key] = value;
                AttributeChangedEventArgs args = new AttributeChangedEventArgs(key, oldValue, value);
                this.OnAttributeChanged(args);
            }
        }
  
        private void OnAttributeChanged(AttributeChangedEventArgs args)
        {
            AttributeChangedEventHandler handler = this.AttributeChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }
    }
}