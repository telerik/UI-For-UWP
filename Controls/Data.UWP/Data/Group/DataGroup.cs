using System;
using System.Collections.Generic;
using Telerik.Data.Core;

namespace Telerik.Data.Core
{
    internal class DataGroup : Group, IDataGroup
    {
        public DataGroup(object key)
            : base(key)
        {
        }

        int IDataGroup.Level
        {
            get
            {
                return this.Level;
            }
        }

        public object Key
        {
            get
            {
                return this.Name;
            }
        }

        public IReadOnlyList<object> ChildItems
        {
            get
            {
                return this.Items;
            }
        }

        public IDataGroup ParentGroup
        {
            get
            {
                bool isGrandTotal = this.Parent != null && Group.GrandTotalName.Equals(this.Parent.Name);
                if (isGrandTotal)
                {
                    return null;
                }

                return this.Parent as IDataGroup;
            }
        }
    }
}