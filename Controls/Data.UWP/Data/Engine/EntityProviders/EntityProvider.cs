using System;
using System.Collections;
using System.ComponentModel;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Represents provider capable to create Entity model object based on the Context passed.
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public abstract class EntityProvider : INotifyPropertyChanged
    {
        private object context;
        private Entity entity;
        private PropertyIteratorMode iteratorMode = PropertyIteratorMode.Declared;

        /// <summary>
        /// Occurs when property changes its value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the source context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public object Context
        {
            get
            {
                return this.context;
            }
            set
            {
                this.context = value;
                this.OnPropertyChanged(nameof(this.Context));
            }
        }

        /// <summary>
        /// Gets or sets the entity model.
        /// </summary>
        /// <value>
        /// The entity.
        /// </value>
        public Entity Entity
        {
            get
            {
                return this.entity;
            }
            set
            {
                this.entity = value;
                this.OnPropertyChanged(nameof(Entity));
            }
        }

        /// <summary>
        /// Gets or sets the iterator property mode.
        /// </summary>
        /// <value>
        /// The iterator mode.
        /// </value>
        public PropertyIteratorMode IteratorMode
        {
            get
            {
                return this.iteratorMode;
            }
            set
            {
                this.iteratorMode = value;
            }
        }

        /// <summary>
        /// Generates the entity.
        /// </summary>
        public virtual Entity GenerateEntity()
        {
            Entity entity = new Entity();
            var properties = this.GetProperties();

            foreach (var property in properties)
            {
                if (!this.ShouldGenerateEntityProperty(property))
                {
                    continue;
                }
                EntityProperty entityProperty = this.GenerateEntityProperty(property);
                
                entity.Properties.Add(entityProperty);
            }

            entity.Validator = this.GetItemValidator(entity);
            this.Entity = entity;
            return entity;
        }

        internal void OnItemChanged(object newItem)
        {
            this.Context = newItem;
        }

        /// <summary>
        /// Generates the entity property.
        /// </summary>
        /// <param name="property">The property.</param>
        protected virtual EntityProperty GenerateEntityProperty(object property)
        {
            var entityProperty = Activator.CreateInstance(this.GetEntityPropertyType(property), new object[2] { property, this.Context }) as EntityProperty;

            entityProperty.PopulatePropertyMetadata();

            return entityProperty;
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        protected abstract IEnumerable GetProperties();

        /// <summary>
        /// Determines whether an entity property should be generated.
        /// </summary>
        /// <param name="property">The property.</param>
        protected abstract bool ShouldGenerateEntityProperty(object property);

        /// <summary>
        /// Gets the item validator.
        /// </summary>
        /// <param name="entity">The entity.</param>
        protected abstract ISupportEntityValidation GetItemValidator(Entity entity);

        /// <summary>
        /// Gets the type of the entity property.
        /// </summary>
        /// <param name="property">The property.</param>
        protected abstract Type GetEntityPropertyType(object property);

        /// <inheritdoc/>
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}