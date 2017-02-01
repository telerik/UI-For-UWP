using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal class SelectionRegionInfoComparer : IComparer<SelectionRegionInfo>
    {
        public int Compare(SelectionRegionInfo x, SelectionRegionInfo y)
        {
            if (x == null && y == null)
            {
                return 0;
            }

            if (x == null)
            {
                return -1;
            }

            if (y == null)
            {
                return 1;
            }

            if (x.StartItem.RowItemInfo.LayoutInfo.Line < y.StartItem.RowItemInfo.LayoutInfo.Line)
            {
                return -1;
            }

            if (y.StartItem.RowItemInfo.LayoutInfo.Line < x.StartItem.RowItemInfo.LayoutInfo.Line)
            {
                return 1;
            }

            if (x.StartItem.RowItemInfo.LayoutInfo.Line == y.StartItem.RowItemInfo.LayoutInfo.Line)
            {
                if ((x.StartItem.Column == null && y.StartItem.Column == null) || (x.EndItem.Column == null && y.EndItem.Column == null))
                {
                    return 0;
                }

                if (x.StartItem.Column.ItemInfo.LayoutInfo.Line < y.StartItem.Column.ItemInfo.LayoutInfo.Line)
                {
                    return -1;
                }

                if (x.StartItem.Column.ItemInfo.LayoutInfo.Line > y.StartItem.Column.ItemInfo.LayoutInfo.Line)
                {
                    return 1;
                }

                if (x.EndItem.Column.ItemInfo.LayoutInfo.Line < y.EndItem.Column.ItemInfo.LayoutInfo.Line)
                {
                    return -1;
                }

                if (x.EndItem.Column.ItemInfo.LayoutInfo.Line > y.EndItem.Column.ItemInfo.LayoutInfo.Line)
                {
                    return 1;
                }

                return 0;
            }

            return 0;
        }
    }
}
