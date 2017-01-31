namespace Telerik.Data.Core.Layouts
{
    internal class TabularGroupInfo
    {
        public TabularGroupInfo(
            object item,
            TabularGroupInfo parent,
            bool isExpanded,
            int level,
            int line,
            int index,
            int lastSubItemSlot)
        {
            this.Line = line;
            this.Item = item;
            this.Parent = parent;
            this.IsExpanded = isExpanded;
            this.Level = level;
            this.Index = index;
            this.LastSubItemSlot = lastSubItemSlot;
        }

        public int Line
        {
            get;
            set;
        }

        public TabularGroupInfo Parent
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

        internal bool IsVisible()
        {
            return this.Parent == null || (this.Parent.IsExpanded && this.Parent.IsVisible());
        }

        internal int GetLineSpan()
        {
            return this.LastSubItemSlot - this.Line + 1;
        }
    }
}