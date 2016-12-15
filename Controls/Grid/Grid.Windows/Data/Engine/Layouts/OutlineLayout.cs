namespace Telerik.Data.Core.Layouts
{
    internal class OutlineLayout : CompactLayout
    {
        public OutlineLayout(IHierarchyAdapter adapter, double defaultItemLength)
            : base(adapter, defaultItemLength)
        {
        }

        internal override int GetLayoutLevel(ItemInfo itemInfo, GroupInfo parentGroupInfo)
        {
            return base.GetIndent(itemInfo, parentGroupInfo);
        }

        internal override int GetIndent(ItemInfo itemInfo, GroupInfo parentGroupInfo)
        {
            return 0;
        }
    }
}