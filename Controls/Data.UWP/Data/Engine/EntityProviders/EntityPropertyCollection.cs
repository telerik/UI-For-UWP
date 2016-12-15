using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.Data.Core
{
    internal class EntityPropertyCollection : Collection<EntityProperty>
    {
        private Entity entity;

        public EntityPropertyCollection(Entity entity)
        {
            this.entity = entity;
        }

        protected override void InsertItem(int index, EntityProperty item)
        {
            base.InsertItem(index, item);

            item.Entity = this.entity;
        }

        protected override void RemoveItem(int index)
        {
            var item = this[index];
            if (item != null)
            {
                item.Entity = null;
            }

            base.RemoveItem(index);
        }

        protected override void SetItem(int index, EntityProperty item)
        {
            var oldItem = this[index];
            if (oldItem != null)
            {
                oldItem.Entity = null;
            }

            base.SetItem(index, item);

            item.Entity = this.entity;
        }
    }
}
