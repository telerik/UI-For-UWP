using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu
{
    internal class MenuItemCollection<T> : ObservableCollection<T> where T : RadialMenuItem
    {
        private RadialMenuModel owner;

        public RadialMenuModel Owner
        {
            get
            {
                return this.owner;
            }
            set
            {
                this.owner = value;

                foreach (var item in this)
                {
                    item.Owner = this.owner;
                }
            }
        }

        public RadialMenuItem ParentItem { get; set; }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);

            this.SetupItem(item, index);
        }

        protected override void SetItem(int index, T item)
        {
            base.SetItem(index, item);

            this.SetupItem(item, index);
        }

        private void SetupItem(T item, int index)
        {
            item.Index = index;
            item.Owner = this.Owner;
            item.ParentItem = this.ParentItem;

            if (item.Owner != null)
            {
                item.Owner.UpdateSelection(item);
            }
        }
    }
}
