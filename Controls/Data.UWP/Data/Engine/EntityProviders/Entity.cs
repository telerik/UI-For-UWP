using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Core;

namespace Telerik.Data.Core
{
    public class Entity
    {
        public bool IsReadOnly { get; internal set; }

        private EntityPropertyCollection properties;
        public Entity()
        {
            this.properties = new EntityPropertyCollection(this);
        }

        public IList<EntityProperty> Properties
        {
            get
            {
                return this.properties;
            }
        }

        internal ISupportEntityValidation Validator { get; set; }

        public EntityProperty GetEntityProperty(string propertyName)
        {
            var entityProperty = this.properties.Where((p) => p.PropertyName == propertyName).FirstOrDefault();

            return entityProperty;
        }
    }
}
