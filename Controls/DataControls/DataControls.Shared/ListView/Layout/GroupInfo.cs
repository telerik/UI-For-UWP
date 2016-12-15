namespace Telerik.Data.Core.Layouts
{
    internal class GroupInfo
    {
        public GroupInfo(
            object item,
            GroupInfo parent,
            bool isExpanded,
            int level,
            int index,
            int lastSubItemSlot)
        {
            this.Item = item;
            this.Parent = parent;
            this.IsExpanded = isExpanded;
            this.Level = level;
            this.Index = index;
            this.LastSubItemSlot = lastSubItemSlot;
        }

        public GroupInfo Parent
        {
            get;
            private set;
        }

        public object Item
        {
            get;
            private set;
        }

        public int LastSubItemSlot
        {
            get;
            set;
        }

        public int Level
        {
            get;
            private set;
        }

        public int Index
        {
            get;
            set;
        }

        public bool IsExpanded
        {
            get;
            set;
        }

        internal int GetLineSpan()
        {
            return this.LastSubItemSlot - this.Index + 1;
        }

        internal bool IsVisible()
        {
            return this.Parent == null || (this.Parent.IsExpanded && this.Parent.IsVisible());
        }
    }
}