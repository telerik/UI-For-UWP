namespace Telerik.Data.Core.Layouts
{
    internal struct ItemInfo
    {
        public object Item;
        public int Id;

        // Keeps the row position considering collapsed item indices.
        public int Slot;

        public int Level;
        public GroupType ItemType;

        // Expand/Collapse state        
        public bool IsCollapsible;

        public bool IsCollapsed;
        public bool IsSummaryVisible;

        public bool IsDisplayed;

        internal static readonly ItemInfo Invalid = new ItemInfo();
    }
}