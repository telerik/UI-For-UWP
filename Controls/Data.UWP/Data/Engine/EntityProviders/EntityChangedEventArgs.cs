using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.Data.Core
{
    internal class EntityChangedEventArgs : EventArgs
    {
        public EntityChangedEventArgs(Entity entity)
        {
            this.NewEntity = entity;
        }

        public Entity NewEntity { get; private set; }
    }
}
