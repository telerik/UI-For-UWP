using Telerik.Data.Core.Layouts;

namespace Telerik.UI.Xaml.Controls.Data.ListView.Model
{
    internal class ItemGenerationContext
    {
        public ItemGenerationContext(ItemInfo info, bool isFrozen)
        {
            this.Info = info;
            this.IsFrozen = isFrozen;
        }

        public ItemInfo Info
        {
            get;
            set;
        }

        public bool IsFrozen
        {
            get;

            private set;
        }
    }
}
