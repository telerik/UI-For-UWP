using System.Collections.Generic;
using System.Linq;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Represents a class for providing information about an Entity model and its properties.
    /// </summary>
    public class Entity
    {
        private EntityPropertyCollection properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class.
        /// </summary>
        public Entity()
        {
            this.properties = new EntityPropertyCollection(this);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="Entity"/> is readonly or not.
        /// </summary>
        public bool IsReadOnly { get; internal set; }

        /// <summary>
        /// Gets a collection that contains all properties of the <see cref="Entity"/> class.
        /// </summary>
        public IList<EntityProperty> Properties
        {
            get
            {
                return this.properties;
            }
        }

        internal ISupportEntityValidation Validator { get; set; }

        /// <summary>
        /// Gets a specific property from the <see cref="Entity.Properties"/> collections by its name.
        /// </summary>
        /// <param name="propertyName">The specific name of the property.</param>
        public EntityProperty GetEntityProperty(string propertyName)
        {
            var entityProperty = this.properties.FirstOrDefault(p => p.PropertyName == propertyName);

            return entityProperty;
        }
    }
}
