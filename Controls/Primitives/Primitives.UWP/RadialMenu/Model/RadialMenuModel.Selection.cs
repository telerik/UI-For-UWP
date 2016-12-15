using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu
{
    internal partial class RadialMenuModel
    {
        private bool updatingSelection;

        internal static bool CanChangeSelection(RadialMenuItem radialMenuItem, bool newValue)
        {
            if ((!radialMenuItem.IsSelected && !newValue && !radialMenuItem.Deselectable) ||
                (radialMenuItem.IsSelected && newValue && !radialMenuItem.Selectable))
            {
                return false;
            }

            return true;
        }

        internal void OnSelectionChanged(RadialMenuItem radialMenuItem)
        {
            var decorationSegment = this.decorationItemsRing.GetSegmentByItem(radialMenuItem);

            if (decorationSegment != null)
            {
                this.decorationItemsRing.UpdateItem(decorationSegment);
            }

            var itemSegment = this.contentRing.GetSegmentByItem(radialMenuItem);

            if (itemSegment != null)
            {
                this.contentRing.UpdateItem(itemSegment);
            }

            this.UpdateSelection(radialMenuItem);

            this.owner.OnSelectionChanged(radialMenuItem);
        }

        internal void UpdateSelection(RadialMenuItem radialMenuItem)
        {
            if (!this.updatingSelection && radialMenuItem != null && !string.IsNullOrEmpty(radialMenuItem.GroupName) && radialMenuItem.IsSelected)
            {
                this.updatingSelection = true;

                var items = radialMenuItem.ParentItem == null ? this.MenuItems : radialMenuItem.ParentItem.ChildItems;

                var itemsInGroup = items.Where(c => c.GroupName == radialMenuItem.GroupName && c != radialMenuItem);

                foreach (var item in itemsInGroup)
                {
                    item.IsSelected = false;
                }

                this.updatingSelection = false;
            }
        }

        internal void UpdateItemsSelection(IEnumerable<RadialMenuItem> items)
        {
            foreach (var item in items)
            {
                this.UpdateSelection(item);
                if (item.HasChildren)
                {
                    this.UpdateItemsSelection(item.ChildItems);
                }
            }
        }
    }
}
